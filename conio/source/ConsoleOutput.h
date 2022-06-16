namespace gehtsoft
{
namespace xce
{
namespace conio
{

ref class Canvas;



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
    void init(bool save, bool changeResolution, int rows, int columns);
 public:
    ConsoleOutput(bool save);
    ConsoleOutput(bool save, int rows, int columns);
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
