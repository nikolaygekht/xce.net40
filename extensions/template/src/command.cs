using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.template_impl
{
    internal class InsertTemplateCommand : IEditorCommand
    {
        TemplateFileTypeCollection mTypes;
        IEditorCommand mEnter = null;

        internal InsertTemplateCommand(TemplateFileTypeCollection types)
        {
            mTypes = types;
        }

        public string Name
        {
            get
            {
                return "InsertTemplate";
            }
        }

        public bool IsEnabled(Application application, string parameter)
        {
            return true;
        }

        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return;
            if (mEnter == null)
                mEnter = application.Commands["Enter"];

            Template t = null;

            if (parameter == null || parameter.Length == 0)
            {
                ChooseTemplateDialog dlg = new ChooseTemplateDialog(application, application.ActiveWindow.Text.FileName, mTypes);
                if (dlg.HasTemplates)
                {
                    if (dlg.DoModal() == XceDialog.DialogResultOK)
                    {
                        t = dlg.Template;
                    }
                }
                else
                    application.ShowMessage("No appropriate templates have been found", "Templates");
            }
            else
            {
                foreach (TemplateFileType t1 in mTypes)
                {
                    foreach (Template t2 in t1.Templates)
                    {
                        if (t2.Name == parameter)
                        {
                            t = t2;
                            break;
                        }
                    }
                    if (t != null)
                        break;
                }
                if (t == null)
                    application.ShowMessage("Specified template has not been found", "Templates");
            }

            if (t != null)
            {
                List<string> parameters = new List<string>();
                if (t.Params.Count > 0)
                {
                    TemplateParametersDialog dlg1 = new TemplateParametersDialog(application, t);
                    if (dlg1.DoModal() == XceDialog.DialogResultOK)
                    {
                        for (int i = 0; i < t.Params.Count; i++)
                            parameters.Add(dlg1.GetParam(i));
                    }
                    else
                        return;
                }
                if (t.Body.Count > 0)
                {
                    FileInfo fi = new FileInfo(application.ActiveWindow.Text.FileName);
                    TextWindow tw = application.ActiveWindow;
                    bool oldInsertMode = tw.InsertMode;
                    tw.InsertMode = true;
                    tw.Text.BeginUndoTransaction();
                    int forceColumn = -1;

                    bool setForce = false;
                    if (tw.CursorRow >= tw.Text.LinesCount)
                        setForce = true;
                    else
                    {
                        int s = tw.Text.LineStart(tw.CursorRow);
                        int l = tw.Text.LineLength(tw.CursorRow);
                        setForce = true;
                        for (int i = 0; i < l && i < tw.CursorColumn; i++)
                            if (!char.IsWhiteSpace(tw.Text[s + i]))
                            {
                                setForce = false;
                                break;
                            }
                    }

                    if (setForce)
                        forceColumn = tw.CursorColumn;


                    for (int line = 0; line < t.Body.Count; line++)
                    {
                        string s = t.Body[line];
                        for (int i = 0; i < t.Params.Count; i++)
                            s = s.Replace("__" + t.Params[i] + "__", parameters[i]);
                        s = s.Replace("__filename__", tw.Text.FileName);
                        s = s.Replace("__onlyname__", fi.Name);
                        s = s.Replace("__guid__", System.Guid.NewGuid().ToString());
                        s = s.Replace("__date__", DateTime.Now.ToString("d"));
                        s = s.Replace("__time__", DateTime.Now.ToString("t"));
                        s = s.Replace("__xmldate__", DateTime.Now.ToString("yyyy-MM-dd"));
                        if (s.Length > 0)
                            tw.Stroke(s, 0, s.Length);
                        if (line < t.Body.Count - 1)
                        {
                            mEnter.Execute(application, null);
                            if (forceColumn >= 0)
                                tw.CursorColumn = forceColumn;
                        }
                    }
                    tw.Text.EndUndoTransaction();
                    tw.InsertMode = oldInsertMode;
                }
            }
        }
    }
}
