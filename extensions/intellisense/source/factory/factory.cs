using System;
using System.Collections.Generic;
using gehtsoft.xce.intellisense.common;
using gehtsoft.intellisense.cs;
using gehtsoft.intellisense.docsource;
using gehtsoft.intellisense.lua;
using gehtsoft.intellisense.vbscript;
using gehtsoft.intellisense.xml;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class IntellisenseFactory
    {
        internal static List<IIntellisenseProvider> CreateProviders(Application application)
        {

            List<IIntellisenseProvider> providers = new List<IIntellisenseProvider>();
            IIntellisenseProvider p;

            p = new CsIntellisenseProvider();
            if (p.Initialize(application))
                providers.Add(p);

            p = new DocSourceIntellisenseProvider();
            if (p.Initialize(application))
                providers.Add(p);

            p = new LuaIntellisenseProvider();
            if (p.Initialize(application))
                providers.Add(p);

            p = new VbIntellisenseProvider();
            if (p.Initialize(application))
                providers.Add(p);

            p = new XmlIntellisenseProvider();
            if (p.Initialize(application))
                providers.Add(p);
            return providers;
        }
    }
}

