class DString;
class CColorerLineSourceAdapter;

namespace gehtsoft
{
namespace xce
{
namespace colorer
{

public interface class ILineSource
{
    System::String ^GetFileName();

    int GetLinesCount();

    int GetLineLength(int lno);

    int GetLine(int line, cli::array<wchar_t> ^buffer, int lineColumnFrom, int length);
};


ref class LineSourceAdapter
{
 private:
    delegate ::SString *adapter_getline_managed(int lno);
    ::CColorerLineSourceAdapter *mNativeAdapter;
    adapter_getline_managed ^mManagedDelegate;
    ILineSource ^mSource;
    cli::array<wchar_t> ^mTemp;

    ::SString *GetLineImpl(int lno);
 internal:
    LineSourceAdapter(ILineSource ^source);
    ~LineSourceAdapter();
    !LineSourceAdapter();

    ::CColorerLineSourceAdapter *getNativeAdapter();
};

}
}
}
