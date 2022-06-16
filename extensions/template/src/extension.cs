using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.extension.template_impl;

namespace gehtsoft.xce.extension
{
    public class template : IEditorExtension, IDisposable
    {
        TemplateFileTypeCollection mTypes = new TemplateFileTypeCollection();
    
        public bool Initialize(Application application)
        {
            mTypes = TemplateReader.read(application);
            application.Commands.AddCommand(new InsertTemplateCommand(mTypes));
            return true;
        }
        
        ~template()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (mTypes != null)
                mTypes.Dispose();
            mTypes = null;
            if (disposing)
                GC.SuppressFinalize(this);
        }
    };
}

