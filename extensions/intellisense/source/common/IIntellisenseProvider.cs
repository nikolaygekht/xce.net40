using System;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.intellisense.common
{
    public interface IIntellisenseProvider
    {
        bool AcceptFile(Application application, TextWindow w);
        bool Initialize(Application application);
        bool Start(Application application, TextWindow w);
        void Stop(Application application, TextWindow w);

        bool CanGetCodeCompletionCollection(Application application, TextWindow w);
        ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow w, out int wline, out int wcolumn, out int wlength);

        bool IsShowOnTheFlyDataCharacter(char character);
        bool IsHideOnTheFlyDataCharacter(char character);
        bool ForwardEnterAtOneTheFlyEnd();
        bool IsOnTheFlyDataCharacter(char character);
        bool CanGetOnTheFlyCompletionData(Application application, TextWindow w);
        ICodeCompletionItemCollection GetOnTheFlyCompletionData(Application application, TextWindow w, char pressed);
        void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length, ICodeCompletionItem item);

        bool IsShowInsightDataCharacter(char character);
        bool CanGetInsightDataProvider(Application application, TextWindow w);
        IInsightDataProvider GetInsightDataProvider(Application application, TextWindow w, char pressed);

        bool CanGetProjectBrowserItemCollection(Application application, TextWindow w);
        IProjectBrowserItemCollection GetProjectBrowserItemCollection(Application application, TextWindow w, out IProjectBrowserItem curSel);
        IProjectBrowserItemCollection FindProjectBrowserItem(Application application, TextWindow w, out IProjectBrowserItem curSel);
    }
}