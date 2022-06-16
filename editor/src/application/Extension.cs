using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.colorer;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.application
{
    public delegate void AfterOpenWindowHook(TextWindow window);
    public delegate void BeforeCloseWindowHook(TextWindow window);
    public delegate void BeforeSaveWindowHook(TextWindow window);
    public delegate void AfterPaintWindowHook(TextWindow window, Canvas canvas);
    public delegate void TimerHook();
    public delegate void KeyPressedHook(int scanCode, char character, bool shift, bool ctrl, bool alt, ref bool handled);
    public delegate void IdleHook();

    public interface IEditorExtension
    {
        bool Initialize(Application application);
    };
}
