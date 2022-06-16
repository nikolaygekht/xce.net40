namespace gehtsoft
{
namespace xce
{
namespace conio
{

public ref class ConsoleColor
{
 private:
    short mPalColor;
    int mRGBFg;
    int mRGBBg;
    int mStyle;
 public:
    ConsoleColor(short palColor);
    ConsoleColor(short palColor, int RGBFg, int RGBBg);
    ConsoleColor(short palColor, int RGBFg, int RGBBg, int style);

    property short PaletteColor
    {
        short get();
        void set(short);
    };

    property bool RGBValid
    {
        bool get();
    }

    property int RGBForeground
    {
        int get();
        void set(int);
    };

    property int RGBBackground
    {
        int get();
        void set(int);
    };

    property int Style
    {
        int get();
        void set(int);
    }

    static int rgb(int red, int green, int blue);
};

public ref class BoxBorder
{
 private:
    wchar_t mTopLeft;
    wchar_t mTop;
    wchar_t mTopRight;
    wchar_t mLeft;
    wchar_t mRight;
    wchar_t mBottomLeft;
    wchar_t mBottom;
    wchar_t mBottomRight;

    static BoxBorder ^mSingleBorder = nullptr;
    static BoxBorder ^mDoubleBorder = nullptr;

 public:
    BoxBorder(wchar_t topLeft, wchar_t top, wchar_t topRight, wchar_t left, wchar_t right, wchar_t bottomLeft, wchar_t bottom, wchar_t bottomRight);

    property wchar_t TopLeft
    {
        wchar_t get();
    };
    property wchar_t Top
    {
        wchar_t get();
    };
    property wchar_t TopRight
    {
        wchar_t get();
    };
    property wchar_t BottomLeft
    {
        wchar_t get();
    };
    property wchar_t Bottom
    {
        wchar_t get();
    };
    property wchar_t BottomRight
    {
        wchar_t get();
    };
    property wchar_t Left
    {
        wchar_t get();
    };
    property wchar_t Right
    {
        wchar_t get();
    };

    static property BoxBorder ^SingleBorderBox
    {
        BoxBorder ^get();
    }

    static property BoxBorder ^DoubleBorderBox
    {
        BoxBorder ^get();
    }

};

public ref class Canvas
{
 private:
     int mRows;
     int mColumns;
     CHAR_INFO *mData;
     int *mForegroundColor;
     int *mBackgroundColor;
     int *mStyle;

 internal:
    property CHAR_INFO *Data
    {
        CHAR_INFO *get();
    };

    property int *ForegroundColor
    {
        int *get();
    };

    property int *BackgroundColor
    {
        int *get();
    };

    property int *Style
    {
        int *get();
    };
 public:
    Canvas(int rows,  int columns);
    ~Canvas();
    !Canvas();

    property int Rows
    {
         int get();
    };

    property int Columns
    {
         int get();
    };

    void write(int row, int column, wchar_t chr);
    void write(int row, int column, ConsoleColor ^color);
    void write(int row, int column, wchar_t chr, ConsoleColor ^color);
    void write(int row, int column, String ^text);
    void write(int row, int column, String ^text, ConsoleColor ^color);
    void write(int row, int column, String ^text, int offset, int length);
    void write(int row, int column, array<wchar_t> ^text, int offset, int length);
    void write(int row, int column, System::Text::StringBuilder ^text, int offset, int length);
    void write(int row, int column, String ^text, int offset, int length, ConsoleColor ^color);
    void write(int row, int column, System::Text::StringBuilder ^text, int offset, int length, ConsoleColor ^color);
    void fill(int row, int column, int rows, int columns, wchar_t chr);
    void fill(int row, int column, int rows, int columns, ConsoleColor ^color);
    void fillFg(int row, int column, int rows, int columns, ConsoleColor ^color);
    void fillBg(int row, int column, int rows, int columns, ConsoleColor ^color);
    void fill(int row, int column, int rows, int columns, wchar_t chr, ConsoleColor ^color);
    void box(int row, int column, int rows, int columns, BoxBorder ^border, ConsoleColor ^color);
    void box(int row, int column, int rows, int columns, BoxBorder ^border, ConsoleColor ^color, wchar_t fillchar);
    void paint(int row, int column, Canvas ^canvas);
    bool get(int row, int column, [Out]wchar_t %chr, ConsoleColor ^color);
};

}
}
}
