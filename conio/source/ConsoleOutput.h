namespace gehtsoft
{
namespace xce
{
namespace conio
{

ref class Canvas;

/** Output modes for the console */
public enum class ConsoleOutputMode
{
    Win32,      //pure win32 mode
    ConEmu,     //con emu true color mode
    VT,         //VT terminal
    VTTC,       //VT terminal with true color support
};


public ref class ConsoleOutput
{
 private:
    int mRows;
    int mColumns;

    bool mRestoreRes;
    int mOrgRows;
    int mOrgColumns;

    int mCursorSize;
    int mCursorRow;
    int mCursorColumn;
    bool mCursorVisible;

    bool mRestoreContent;
    CHAR_INFO *mOrgContent;
    AnnotationInfo *mOrgInfo;

    UINT mOldCP;

    HANDLE hSharedMem;
    AnnotationHeader *mRGBHeader;
    AnnotationInfo *mRGBInfo;

    ConsoleOutputMode mOutputMode;

    void init(bool save, bool changeResolution, int rows, int column, ConsoleOutputMode outputMode);
    void paintWin32(Canvas ^canvas, bool fast);
    void paintVT(Canvas ^canvas, bool fast);
    void EscapeCode(ConsoleColor^ color, wchar_t *sequence);
    void WriteVtSequence(wchar_t *buffer);

    Canvas ^mOldScreen;
 public:
    ConsoleOutput(bool save);
    ConsoleOutput(bool save, ConsoleOutputMode outputMode);
    ConsoleOutput(bool save, int rows, int columns);
    ConsoleOutput(bool save, int rows, int columns, ConsoleOutputMode outputMode);
    ~ConsoleOutput();
    !ConsoleOutput();

    property int Rows
    {
        int get();
    };

    property int Columns
    {
        int get();
    };

    void paint(Canvas ^canvas, bool fast);

    void setCursorType(int size, bool visible);
    void getCursorType([Out]int %size, [Out]bool %visible);
    void setCursorPos(int row, int column);
    void getCursorPos([Out]int %row, [Out]int %column);

    void updateSize();

};

}
}
}
