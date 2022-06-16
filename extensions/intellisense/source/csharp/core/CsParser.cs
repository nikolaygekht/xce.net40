using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace gehtsoft.intellisense.cs
{
    public class CsParser
    {
        private CsProject mProject;
        private Thread mThread;
        private object mStopEvent;
        private object mStoppedEvent;
        private object mParserMutex;
        private bool mStopRequest, mStopped;
        private int mTimeout;
        private string mLogFile;

        static readonly LanguageProperties CurrentLanguageProperties = LanguageProperties.CSharp;
        private ProjectContentRegistry mParserRegistry;
        private DefaultProjectContent mParserProjectContent;
        private CsParserFileCollection mProjectFiles = new CsParserFileCollection();

        public CsProject Project
        {
            get
            {
                return mProject;
            }
        }

        public CsParser(CsProject project, bool createCache, int timeout)
        {
            mProject = project;
            mStopEvent = new object();
            mStoppedEvent = new object();
            mThread = new Thread(this.ParsingThread);
            mThread.IsBackground = true;
            mStopRequest = false;
            mStopped = true;
            mTimeout = timeout;
            mParserMutex = new object();

            mParserRegistry = new ProjectContentRegistry(); // Default .NET 2.0 registry
            if (createCache && !mProject.IsDefault)
            {
                FileInfo fi = new FileInfo(mProject.Name);
                string ispath = Path.Combine(fi.Directory.FullName, "intellisense\\" + fi.Name);
                if (!Directory.Exists(ispath))
                    Directory.CreateDirectory(ispath);
                mLogFile = Path.Combine(ispath, "intellisense-error.log");
                mParserRegistry.ActivatePersistence(ispath);
            }
            else
            {
                mLogFile = ".\\intellisense-error.log";
            }

            mParserProjectContent = new DefaultProjectContent();
            mParserProjectContent.Language = CurrentLanguageProperties;
        }

        private void log(String message)
        {
            using (StreamWriter w = new StreamWriter(mLogFile, true))
            {
                w.WriteLine("{0}", message);
                w.Close();
            }
        }

        private void ParsingThread()
        {
            try
            {
                mStopped = false;

                try
                {
                    mParserProjectContent.AddReferencedContent(mParserRegistry.Mscorlib);
                }
                catch (Exception e)
                {
                    log(string.Format("Can't add mscorlib content:\n{0}", e.ToString()));
                    return;
                }
                //load the assemblies first
                foreach (CsProjectReference r in mProject.References)
                {
                    try
                    {
                        string assemblyNameCopy = r.Name; // copy for anonymous method
                        string assemblyPath = r.Path;
                        if (assemblyPath == null)
                            assemblyPath = assemblyNameCopy;

                        IProjectContent referenceProjectContent = mParserRegistry.GetProjectContentForReference(assemblyNameCopy, assemblyPath);
                        mParserProjectContent.AddReferencedContent(referenceProjectContent);
                        if (referenceProjectContent is ReflectionProjectContent)
                            (referenceProjectContent as ReflectionProjectContent).InitializeReferences();
                    }
                    catch (Exception e)
                    {
                        log(string.Format("Can't add a reference {0} content:\n{1}", r.Name, e.ToString()));
                        return;
                    }
                }

                //load all project files first
                foreach (CsProjectSource s in mProject.Sources)
                {
                    if (mProjectFiles.IndexOf(s.Path) < 0)
                        AddFileToProject(s.Path);
                }

                //background job
                while (!mStopRequest)
                {
                    try
                    {
                        for (int i = 0; i < mProjectFiles.Count; i++)
                        {
                            if (mProjectFiles[i].Source.Changed)
                            {
                                ParseAndUpdateFile(mProjectFiles[i]);
                                mProjectFiles[i].Source.Changed = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log(string.Format("Parsing the file failed", e.ToString()));
                    }
                    lock (mStopEvent)
                        Monitor.Wait(mStopEvent, mTimeout);
                    if (mStopRequest)
                        break;
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
            finally
            {
                mStopped = true;
                lock (mStoppedEvent)
                    Monitor.PulseAll(mStoppedEvent);
            }
        }

        internal CsParserFile FindFileInProject(string path)
        {
            int index = mProjectFiles.IndexOf(path);
            if (index >= 0)
                return mProjectFiles[index];
            else
                return null;
        }

        public bool Works
        {
            get
            {
                return mThread.IsAlive && !mStopped;
            }
        }

        public CsParserFile NewFileInEditor(string path, ICsParserFileSource source)
        {
            int index = mProjectFiles.IndexOf(path);
            if (index >= 0)
                RemoveFileFromProject(mProjectFiles[index]);
            return AddFileToProject(path, source);
        }

        public void CloseFileInEditor(CsParserFile file)
        {
            int index = mProjectFiles.IndexOf(file);
            if (index >= 0)
                RemoveFileFromProject(mProjectFiles[index]);
            if (mProject.Sources.IndexOf(file.Name) != -1)
                AddFileToProject(file.Name);
        }

        public bool HasEditorFiles()
        {
            foreach (CsParserFile file in mProjectFiles)
            {
                if (file.Source.GetType() != typeof(CsParserFileSource))
                    return true;
            }
            return false;
        }

        public void UpdateFileImmediatelly(CsParserFile file)
        {
            int index = mProjectFiles.IndexOf(file);
            if (index >= 0)
                ParseAndUpdateFile(file);
        }

        private void RemoveFileFromProject(CsParserFile file)
        {
            int index = mProjectFiles.IndexOf(file);
            if (index >= 0)
            {
                if (file.CompilationUnit != null)
                    mParserProjectContent.RemoveCompilationUnit(file.CompilationUnit);
                mProjectFiles.Remove(index);
            }
        }

        private CsParserFile AddFileToProject(string path)
        {
            return AddFileToProject(path, new CsParserFileSource(path));
        }

        private CsParserFile AddFileToProject(string path, ICsParserFileSource source)
        {
            CsParserFile file = new CsParserFile(this, path, source);
            if (ParseAndUpdateFile(file))
                mProjectFiles.Add(file);
            return file;
        }

        private bool ParseAndUpdateFile(CsParserFile file)
        {
            lock (mParserMutex)
            {
                TextReader textReader = null;
                try
                {
                    textReader = file.Source.CreateReader();
                    ICompilationUnit newCompilationUnit;
                    SupportedLanguage supportedLanguage;
                    supportedLanguage = SupportedLanguage.CSharp;
                    using (IParser p = ParserFactory.CreateParser(supportedLanguage, textReader))
                    {
                        //if (file.Source is CsParserFileSource)
                            p.ParseMethodBodies = false;
                        //else
                        //    p.ParseMethodBodies = true;
                        p.Parse();
                        newCompilationUnit = ConvertCompilationUnit(p.CompilationUnit);
                    }
                    mParserProjectContent.UpdateCompilationUnit(file.CompilationUnit, newCompilationUnit, file.Name);
                    file.CompilationUnit = newCompilationUnit;
                    return true;
                }
                catch (Exception e)
                {
                    log(string.Format("Can't add a file {0} content:\n{1}", file.Name, e.ToString()));
                    return false;
                }
                finally
                {
                    if (textReader != null)
                        textReader.Close();
                    textReader = null;
                }
            }
        }


        private ICompilationUnit ConvertCompilationUnit(CompilationUnit cu)
        {
            NRefactoryASTConvertVisitor converter;
            converter = new NRefactoryASTConvertVisitor(mParserProjectContent);
            cu.AcceptVisitor(converter, null);
            return converter.Cu;
        }


        public void Start()
        {
            mStopRequest = false;
            mThread.Start();
        }

        public void Stop()
        {
            mStopRequest = true;
            lock (mStopEvent)
                Monitor.PulseAll(mStopEvent);
            int max = 0;
            while (!mStopped && max < 1000)
            {
                lock (mStoppedEvent)
                    Monitor.Wait(mStoppedEvent, 50);
                max += 50;
            }
        }

        public bool Parsed
        {
            get
            {
                if (mStopped)
                    return false;
                if (mProjectFiles.Count < mProject.Sources.Count)
                    return false;

                for (int i = 0; i < mProjectFiles.Count; i++)
                    if (mProjectFiles[i].Source.Changed)
                        return false;

                return true;
            }
        }

        public CsParserFileCollection Files
        {
            get
            {
                return mProjectFiles;
            }
        }

        public CsParserExpression FindExpression(CsParserFile file, int line, int column, bool to, bool full)
        {
            int index = mProjectFiles.IndexOf(file);

            if (index >= 0)
            {
                lock (mParserMutex)
                {
                    ParseAndUpdateFile(file);

                    string t;
                    int pos;
                    if (to)
                    {
                        t = file.Source.ReadTo(line, column);
                        pos = t.Length;
                    }
                    else
                    {
                        t = file.Source.ReadTo(-1);
                        pos = file.Source.LineToPosition(line, column);
                        if (pos > t.Length)
                            pos = t.Length;
                    }

                    return new CsParserExpression(FindExpression(file, t, pos, full));
                }
            }
            else
                return null;
        }

        private ExpressionResult FindExpression(CsParserFile file, string content, int pos, bool full)
        {
            IExpressionFinder finder = new CSharpExpressionFinder(file.ParseInformaiton);
            ExpressionResult expression;
            if (full)
                expression = finder.FindFullExpression(content, pos);
            else
                expression = finder.FindExpression(content, pos);
            return expression;
        }

        public CsCodeCompletionItemCollection GetCompletionData(CsParserFile file, int line, int column, bool allowComplete)
        {
            return GetCompletionData(file, line, column, allowComplete, false);
        }

        public CsCodeCompletionItemCollection GetCompletionData(CsParserFile file, int line, int column, bool allowComplete, bool forceDot)
        {
            CsCodeCompletionItemCollection collection = new CsCodeCompletionItemCollection();
            int index = mProjectFiles.IndexOf(file);
            string preSelection = "";

            if (index >= 0)
            {
                lock (mParserMutex)
                {
                    ParseAndUpdateFile(file);
                    string content = file.Source.ReadTo(-1);
                    NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
                    ArrayList l;

                    if (!allowComplete)
                    {
                        l = resolver.CtrlSpace(line, column, file.ParseInformaiton, content, ExpressionContext.Default);
                        AddResolveResults(collection, l, ExpressionContext.Default);
                        return collection;
                    }

                    int position = file.Source.LineToPosition(line, column);
                    string content1;
                    if (position < content.Length)
                        content1 = content.Substring(0, position);
                    else
                        content1 = content;
                    ExpressionResult expressionResult = FindExpression(file, content1, position, false);
                    string expression = expressionResult.Expression;
                    if (expression == null || expression.Length == 0)
                    {
                        if (forceDot)
                            return collection;
                        l = resolver.CtrlSpace(line, column, file.ParseInformaiton, content, expressionResult.Context);
                        AddResolveResults(collection, l, expressionResult.Context);
                        return collection;
                    }


                    int idx;

                    if (forceDot)
                        idx = expression.Length;
                    else
                        idx = expression.LastIndexOf('.');
                    if (idx > 0)
                    {
                        //GenerateCompletionData(textArea, expressionResult);
                        expressionResult.Expression = expression.Substring(0, idx);
                        if (idx + 1 < expression.Length)
                            preSelection = expression.Substring(idx + 1);
                        ResolveResult rr = resolver.Resolve(expressionResult, file.ParseInformaiton, content);
                        if (rr != null)
                            AddResolveResults(collection, rr.GetCompletionData(mParserProjectContent), expressionResult.Context);
                    }
                    else
                    {
                        if (forceDot)
                            return collection;
                        preSelection = expression;
                        l = resolver.CtrlSpace(line, column, file.ParseInformaiton, content, expressionResult.Context);
                        AddResolveResults(collection, l, expressionResult.Context);
                    }
                    if (preSelection.Length > 0)
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            string s1 = collection[i].Name;
                            if (s1.Length >= preSelection.Length &&
                                s1.IndexOf(preSelection) == 0)
                                {
                                    collection.DefaultIndex_Internal = i;
                                    collection.Preselection_Internal = preSelection;
                                    break;
                                }
                        }
                    }
                }
            }
            return collection;
        }

        internal List<CsInsightData> GetListOfInsightMethods(CsParserFile file, int line, int column)
        {
            List<CsInsightData> list = new List<CsInsightData>();
            int index = mProjectFiles.IndexOf(file);
            IAmbience a = new CSharpAmbience();
            a.ConversionFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowAccessibility | ConversionFlags.ShowParameterList | ConversionFlags.ShowTypeParameterList;

            if (index >= 0)
            {
                lock (mParserMutex)
                {
                    ParseAndUpdateFile(file);
                    NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
                    int position = file.Source.LineToPosition(line, column);
                    string content, content1;
                    content = file.Source.ReadTo(-1);

                    if (position < content.Length)
                        content1 = content.Substring(0, position);
                    else
                        content1 = content;

                    ExpressionResult expressionResult = FindExpression(file, content1, position, false);
                    string expression = null;
                    if (expressionResult.Expression != null)
                        expression = expressionResult.Expression.Trim();
                    if (expression == null || expression.Length == 0)
                        return list;

                    bool constructorInsight = false;
                    if (expressionResult.Context == ExpressionContext.Attribute)
                        constructorInsight = true;
                    else if (expressionResult.Context.IsObjectCreation)
                    {
                        constructorInsight = true;
                        expressionResult.Context = ExpressionContext.Type;
                    }
                    else if (expressionResult.Context == ExpressionContext.BaseConstructorCall)
                        constructorInsight = true;

                    ResolveResult results = resolver.Resolve(expressionResult, file.ParseInformaiton, content);

                    if (results == null)
                        return list;

                    LanguageProperties language = LanguageProperties.CSharp;
                    TypeResolveResult trr = results as TypeResolveResult;

                    if (trr == null && language.AllowObjectConstructionOutsideContext)
                    {
                        if (results is MixedResolveResult)
                            trr = (results as MixedResolveResult).TypeResult;
                    }

                    if (trr != null && !constructorInsight)
                    {
                        if (language.AllowObjectConstructionOutsideContext)
                            constructorInsight = true;
                    }

                    if (constructorInsight)
                    {
                        if (trr != null || expressionResult.Context == ExpressionContext.BaseConstructorCall)
                        {
                            if (results.ResolvedType != null)
                            {
                                foreach (IMethod m in results.ResolvedType.GetMethods())
                                    if (m.IsConstructor)
                                        list.Add(new CsInsightData(a.Convert(m), m));
                            }
                        }
                    }
                    else
                    {
                        MethodGroupResolveResult result = results as MethodGroupResolveResult;
                        if (result == null)
                            return list;
                        bool classIsInInheritanceTree = false;
                        if (result.CallingClass != null)
                            classIsInInheritanceTree = result.CallingClass.IsTypeInInheritanceTree(result.ContainingType.GetUnderlyingClass());

                        foreach (IMethod method in result.ContainingType.GetMethods())
                        {
                            if (language.NameComparer.Equals(method.Name, result.Name))
                            {
                                if (method.IsAccessible(result.CallingClass, classIsInInheritanceTree))
                                    list.Add(new CsInsightData(a.Convert(method), method));
                            }
                        }
                        if (list.Count == 0 && result.CallingClass != null && language.SupportsExtensionMethods)
                        {
                            ArrayList list1 = new ArrayList();
                            ResolveResult.AddExtensions(language, list1, result.CallingClass, result.ContainingType);
                            foreach (IMethodOrProperty mp in list1)
                            {
                                if (language.NameComparer.Equals(mp.Name, result.Name) && mp is IMethod)
                                {
                                    DefaultMethod m = (DefaultMethod)mp.CreateSpecializedMember();
                                    // for the insight window, remove first parameter and mark the
                                    // method as normal - this is required to show the list of
                                    // parameters the method expects.
                                    m.IsExtensionMethod = false;
                                    m.Parameters.RemoveAt(0);
                                    list.Add(new CsInsightData(a.Convert(m), m));
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        public CsCodeCompletionItemCollection GetConstructorData(CsParserFile file, int line, int column)
        {
            CsCodeCompletionItemCollection coll = new CsCodeCompletionItemCollection();
            IClass c = file.ParseInformaiton.MostRecentCompilationUnit.GetInnermostClass(line, column);

            if (c == null)
                return coll;

            CSharpAmbience a = new CSharpAmbience();

            string n, t;

            foreach (IMethod meth in c.BaseType.GetMethods())
            {
                if (meth.IsConstructor)
                {
                    a.ConversionFlags = ConversionFlags.ShowParameterList;
                    n = a.Convert(meth).Replace("#ctor", meth.DeclaringType.Name);
                    a.ConversionFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
                    t = a.Convert(meth).Replace(meth.DeclaringType.Name, meth.DeclaringType == c ? "this" : "base");
                    coll.Add(new CsCodeCompletionItem(n, t));
                }
            }
            coll.Sort();
            return coll;
        }

        public CsCodeCompletionItemCollection GetOverloadData(CsParserFile file, int line, int column)
        {
            CsCodeCompletionItemCollection coll = new CsCodeCompletionItemCollection();

            IClass c = file.ParseInformaiton.MostRecentCompilationUnit.GetInnermostClass(line, column);

            if (c == null)
                return coll;

            CSharpAmbience a = new CSharpAmbience();

            string n, t;

            foreach (IProperty prop in c.BaseType.GetProperties())
            {
                if (prop.IsOverridable && !prop.IsOverride)
                {
                    a.ConversionFlags = ConversionFlags.ShowParameterList;
                    n = "p:" + a.Convert(prop);
                    a.ConversionFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowAccessibility;
                    t = "override " + a.Convert(prop);
                    coll.Add(new CsCodeCompletionItem(n, t));
                }
            }

            foreach (IMethod meth in c.BaseType.GetMethods())
            {
                if (meth.IsOverridable && ! meth.IsOverride)
                {
                    a.ConversionFlags = ConversionFlags.ShowParameterList;
                    n = "m:" + meth.DeclaringType.Name + "." + a.Convert(meth);
                    a.ConversionFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowAccessibility;
                    t = "override " + a.Convert(meth);
                    coll.Add(new CsCodeCompletionItem(n, t));

                }
            }
            coll.Sort();
            return coll;
        }



        private void AddResolveResults(CsCodeCompletionItemCollection coll, ICollection list, ExpressionContext context)
        {
            if (list == null)
                return;
            coll.Capacity += list.Count;
            ICsCodeCompletionItem suggestedData = null;
            foreach (object o in list)
            {
                if (context != null && !context.ShowEntry(o))
                    continue;
                ICsCodeCompletionItem ccd = CreateItem(coll, o, context);
                if (object.Equals(o, context.SuggestedItem))
                    suggestedData = ccd;
                if (ccd != null)
                    coll.Add(ccd);
            }

            coll.Sort();

            if (context.SuggestedItem != null)
            {
                if (suggestedData == null)
                {
                    suggestedData = CreateItem(coll, context.SuggestedItem, context);
                    if (suggestedData != null)
                    {
                        coll.Add(suggestedData);
                        coll.Sort();
                    }
                }
                if (suggestedData != null)
                {
                    coll.DefaultIndex_Internal = coll.IndexOf(suggestedData);
                }
            }
        }

        private ICsCodeCompletionItem CreateItem(CsCodeCompletionItemCollection coll, object item, ExpressionContext context)
        {
            if (item is string)
            {
                return new CsCodeCompletionItem(item.ToString());
            }
            else if (item is IClass)
            {
                return new CsCodeCompletionItem((IClass)item);
            }
            else if (item is IProperty)
            {
                IProperty property = (IProperty)item;
                if (property.Name != null)
                {
                    ICsCodeCompletionItem p = coll.Find(property.Name);
                    if (p != null)
                    {
                        p.Overloads.Add(new CsCodeCompletionItem(property));
                        return null;
                    }
                    else
                        return new CsCodeCompletionItem(property);
                }
            }
            else if (item is IMethod)
            {
                IMethod method = (IMethod)item;
                if (method.Name != null)
                {
                    ICsCodeCompletionItem p = coll.Find(method.Name);
                    if (p != null)
                    {
                        p.Overloads.Add(new CsCodeCompletionItem(method));
                        return null;
                    }
                    else
                        return new CsCodeCompletionItem(method);
                }
            }
            else if (item is IField)
            {
                return new CsCodeCompletionItem((IField)item);
            }
            else if (item is IEvent)
            {
                IEvent ev = (IEvent)item;
                if (ev.Name != null)
                {
                    ICsCodeCompletionItem p = coll.Find(ev.Name);
                    if (p != null)
                    {
                        p.Overloads.Add(new CsCodeCompletionItem(ev));
                        return null;
                    }
                    else
                        return new CsCodeCompletionItem(ev);
                }
            }
            return null;
        }
    }
}

