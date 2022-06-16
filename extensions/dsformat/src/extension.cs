using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.dsformat_impl;

namespace gehtsoft.xce.extension
{
    public class dsformat : IEditorExtension
    {
        public bool Initialize(Application application)
        {
            application.Commands.AddCommand(new FormatDocSourceCommand());
            application.Commands.AddCommand(new BreakLineByWidthCommand());
            application.Commands.AddCommand(new FormatParaByWidth());
            return true;
        }
    };
}

