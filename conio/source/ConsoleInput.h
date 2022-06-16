#pragma unmanaged
typedef BOOL (_stdcall *PGetConsoleKeyboardLayoutNameA)(PSTR);
#pragma managed

namespace gehtsoft
{
namespace xce
{
namespace conio
{

#undef XBUTTON1
#undef XBUTTON2

public ref class ScanCode sealed
{
 public:
    const static int LBUTTON = 0x01;               //LBUTTON
    const static int RBUTTON = 0x02;               //RBUTTON
    const static int CANCEL = 0x03;                //CANCEL
    const static int MBUTTON = 0x04;               //MBUTTON
    const static int XBUTTON1 = 0x05;              //XBUTTON1
    const static int XBUTTON2 = 0x06;              //XBUTTON2
    const static int BACK = 0x08;                  //BACK
    const static int TAB = 0x09;                   //TAB
    const static int CLEAR = 0x0C;                 //CLEAR
    const static int RETURN = 0x0D;                //RETURN
    const static int SHIFT = 0x10;                 //SHIFT
    const static int CONTROL = 0x11;               //CONTROL
    const static int MENU = 0x12;                  //MENU
    const static int PAUSE = 0x13;                 //PAUSE
    const static int CAPITAL = 0x14;               //CAPITAL
    const static int KANA = 0x15;                  //KANA
    const static int HANGEUL = 0x15;               //HANGEUL
    const static int HANGUL = 0x15;                //HANGUL
    const static int JUNJA = 0x17;                 //JUNJA
    const static int FINAL = 0x18;                 //FINAL
    const static int HANJA = 0x19;                 //HANJA
    const static int KANJI = 0x19;                 //KANJI
    const static int ESCAPE = 0x1B;                //ESCAPE
    const static int CONVERT = 0x1C;               //CONVERT
    const static int NONCONVERT = 0x1D;            //NONCONVERT
    const static int ACCEPT = 0x1E;                //ACCEPT
    const static int MODECHANGE = 0x1F;            //MODECHANGE
    const static int SPACE = 0x20;                 //SPACE
    const static int PRIOR = 0x21;                 //PRIOR
    const static int NEXT = 0x22;                  //NEXT
    const static int END = 0x23;                   //END
    const static int HOME = 0x24;                  //HOME
    const static int LEFT = 0x25;                  //LEFT
    const static int UP = 0x26;                    //UP
    const static int RIGHT = 0x27;                 //RIGHT
    const static int DOWN = 0x28;                  //DOWN
    const static int SELECT = 0x29;                //SELECT
    const static int PRINT = 0x2A;                 //PRINT
    const static int EXECUTE = 0x2B;               //EXECUTE
    const static int SNAPSHOT = 0x2C;              //SNAPSHOT
    const static int INSERT = 0x2D;                //INSERT
    const static int DEL = 0x2E;                   //DEL
    const static int HELP = 0x2F;                  //HELP
    const static int LWIN = 0x5B;                  //LWIN
    const static int RWIN = 0x5C;                  //RWIN
    const static int APPS = 0x5D;                  //APPS
    const static int SLEEP = 0x5F;                 //SLEEP
    const static int NUMPAD0 = 0x60;               //NUMPAD0
    const static int NUMPAD1 = 0x61;               //NUMPAD1
    const static int NUMPAD2 = 0x62;               //NUMPAD2
    const static int NUMPAD3 = 0x63;               //NUMPAD3
    const static int NUMPAD4 = 0x64;               //NUMPAD4
    const static int NUMPAD5 = 0x65;               //NUMPAD5
    const static int NUMPAD6 = 0x66;               //NUMPAD6
    const static int NUMPAD7 = 0x67;               //NUMPAD7
    const static int NUMPAD8 = 0x68;               //NUMPAD8
    const static int NUMPAD9 = 0x69;               //NUMPAD9
    const static int MULTIPLY = 0x6A;              //MULTIPLY
    const static int ADD = 0x6B;                   //Gray+
    const static int SEPARATOR = 0x6C;             //Gray/
    const static int SUBTRACT = 0x6D;              //Gray-
    const static int DECIMAL = 0x6E;               //Gray.
    const static int DIVIDE = 0x6F;                //Gray/
    const static int F1 = 0x70;                    //F1
    const static int F2 = 0x71;                    //F2
    const static int F3 = 0x72;                    //F3
    const static int F4 = 0x73;                    //F4
    const static int F5 = 0x74;                    //F5
    const static int F6 = 0x75;                    //F6
    const static int F7 = 0x76;                    //F7
    const static int F8 = 0x77;                    //F8
    const static int F9 = 0x78;                    //F9
    const static int F10 = 0x79;                   //F10
    const static int F11 = 0x7A;                   //F11
    const static int F12 = 0x7B;                   //F12
    const static int F13 = 0x7C;                   //F13
    const static int F14 = 0x7D;                   //F14
    const static int F15 = 0x7E;                   //F15
    const static int F16 = 0x7F;                   //F16
    const static int F17 = 0x80;                   //F17
    const static int F18 = 0x81;                   //F18
    const static int F19 = 0x82;                   //F19
    const static int F20 = 0x83;                   //F20
    const static int F21 = 0x84;                   //F21
    const static int F22 = 0x85;                   //F22
    const static int F23 = 0x86;                   //F23
    const static int F24 = 0x87;                   //F24
    const static int NUMLOCK = 0x90;               //NUMLOCK
    const static int SCROLL = 0x91;                //SCROLL
    const static int OEM_NEC_EQUAL = 0x92;         //OEM_NEC_EQUAL
    const static int OEM_FJ_JISHO = 0x92;          //OEM_FJ_JISHO
    const static int OEM_FJ_MASSHOU = 0x93;        //OEM_FJ_MASSHOU
    const static int OEM_FJ_TOUROKU = 0x94;        //OEM_FJ_TOUROKU
    const static int OEM_FJ_LOYA = 0x95;           //OEM_FJ_LOYA
    const static int OEM_FJ_ROYA = 0x96;           //OEM_FJ_ROYA
    const static int LSHIFT = 0xA0;                //LSHIFT
    const static int RSHIFT = 0xA1;                //RSHIFT
    const static int LCONTROL = 0xA2;              //LCONTROL
    const static int RCONTROL = 0xA3;              //RCONTROL
    const static int LMENU = 0xA4;                 //LMENU
    const static int RMENU = 0xA5;                 //RMENU
    const static int BROWSER_BACK = 0xA6;          //BROWSER_BACK
    const static int BROWSER_FORWARD = 0xA7;       //BROWSER_FORWARD
    const static int BROWSER_REFRESH = 0xA8;       //BROWSER_REFRESH
    const static int BROWSER_STOP = 0xA9;          //BROWSER_STOP
    const static int BROWSER_SEARCH = 0xAA;        //BROWSER_SEARCH
    const static int BROWSER_FAVORITES = 0xAB;     //BROWSER_FAVORITES
    const static int BROWSER_HOME = 0xAC;          //BROWSER_HOME
    const static int VOLUME_MUTE = 0xAD;           //VOLUME_MUTE
    const static int VOLUME_DOWN = 0xAE;           //VOLUME_DOWN
    const static int VOLUME_UP = 0xAF;             //VOLUME_UP
    const static int MEDIA_NEXT_TRACK = 0xB0;      //MEDIA_NEXT_TRACK
    const static int MEDIA_PREV_TRACK = 0xB1;      //MEDIA_PREV_TRACK
    const static int MEDIA_STOP = 0xB2;            //MEDIA_STOP
    const static int MEDIA_PLAY_PAUSE = 0xB3;      //MEDIA_PLAY_PAUSE
    const static int LAUNCH_MAIL = 0xB4;           //LAUNCH_MAIL
    const static int LAUNCH_MEDIA_SELECT = 0xB5;   //LAUNCH_MEDIA_SELECT
    const static int LAUNCH_APP1 = 0xB6;           //LAUNCH_APP1
    const static int LAUNCH_APP2 = 0xB7;           //LAUNCH_APP2
    const static int OEM_1 = 0xBA;                 //;
    const static int OEM_PLUS = 0xBB;              //+
    const static int OEM_COMMA = 0xBC;             //,
    const static int OEM_MINUS = 0xBD;             //-
    const static int OEM_PERIOD = 0xBE;            //.
    const static int OEM_2 = 0xBF;                 ///
    const static int OEM_3 = 0xC0;                 //~
    const static int OEM_4 = 0xDB;                 //[
    const static int OEM_5 = 0xDC;                 //\ back slash
    const static int OEM_6 = 0xDD;                 //]
    const static int OEM_7 = 0xDE;                 //'
    const static int OEM_8 = 0xDF;                 //OEM_8
    const static int OEM_AX = 0xE1;                //OEM_AX
    const static int OEM_102 = 0xE2;               //OEM_102
    const static int ICO_HELP = 0xE3;              //ICO_HELP
    const static int ICO_00 = 0xE4;                //ICO_00
    const static int ZERO = 0x30;                  //0
    const static int ONE = 0x31;                   //1
    const static int TWO = 0x32;                   //2
    const static int FREE = 0x33;                  //3
    const static int FOUR = 0x34;                  //4
    const static int FIVE = 0x35;                  //5
    const static int SIX = 0x36;                   //6
    const static int SEVEN = 0x37;                 //7
    const static int EIGHT = 0x38;                 //8
    const static int NINE = 0x39;                  //9
    const static int A = (int)'A';                 //A
    const static int B = (int)'B';                 //B
    const static int C = (int)'C';                 //C
    const static int D = (int)'D';                 //D
    const static int E = (int)'E';                 //E
    const static int F = (int)'F';                 //F
    const static int G = (int)'G';                 //G
    const static int H = (int)'H';                 //H
    const static int I = (int)'I';                 //I
    const static int J = (int)'J';                 //J
    const static int K = (int)'K';                 //K
    const static int L = (int)'L';                 //L
    const static int M = (int)'M';                 //M
    const static int N = (int)'N';                 //N
    const static int O = (int)'O';                 //O
    const static int P = (int)'P';                 //P
    const static int Q = (int)'Q';                 //Q
    const static int R = (int)'R';                 //R
    const static int S = (int)'S';                 //S
    const static int T = (int)'T';                 //T
    const static int U = (int)'U';                 //U
    const static int V = (int)'V';                 //V
    const static int W = (int)'W';                 //W
    const static int X = (int)'X';                 //X
    const static int Y = (int)'Y';                 //Y
    const static int Z = (int)'Z';                 //Z
 private:
    ScanCode();
};


public interface class ConsoleInputListener
{
    void onKeyPressed(int scanCode, wchar_t character, bool shift, bool ctrl, bool alt);
    void onKeyReleased(int scanCode, wchar_t character, bool shift, bool ctrl, bool alt);
    void onMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool lb, bool rb);
    void onMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt);
    void onMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt);
    void onMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt);
    void onMouseRButtonUp(int row, int column, bool shift, bool ctrl, bool alt);
    void onMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt);
    void onMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt);
    void onGetFocus(bool shift, bool ctrl, bool alt);
    void onScreenBufferChanged(int rows, int columns);
};

public ref class ConsoleInput
{
 private:
    bool mLeftButtonPressed;
    bool mRightButtonPressed;
    int mMouseRow, mMouseColumn;
    ::PGetConsoleKeyboardLayoutNameA GetConsoleKeyboardLayoutNameA;
 public:
    ConsoleInput();
    ~ConsoleInput();
    !ConsoleInput();

    static int keyNameToCode(String ^name);
    static String ^keyCodeToName(int code);

    bool read(ConsoleInputListener ^listener, int timeout);

    property int CurrentLayout
    {
        int get();
    };
};

}
}
}
