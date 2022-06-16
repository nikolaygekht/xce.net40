namespace gehtsoft
{
namespace xce
{
namespace conio
{

public enum class TextClipboardFormat 
{
    Text = CF_TEXT,
    UnicodeText = CF_UNICODETEXT,
};

public ref class TextClipboard sealed
{
 public:
    static bool IsTextAvailable(TextClipboardFormat format);
    static String ^GetText(TextClipboardFormat format);
    static void SetText(String ^text, TextClipboardFormat format);
    static String ^GetText();
    static void SetText(String ^text);
 private:
    TextClipboard();
};

}
}
}
