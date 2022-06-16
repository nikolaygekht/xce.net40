class CRegExp1;
class SString;
struct SMatches1;

namespace gehtsoft
{
namespace xce
{
namespace colorer
{

public ref class Regex
{
 internal:
    ::CRegExp1 *mRegex;
    ::String *mValue;
    ::SMatches1 *mMatches;
    int mStartIndex;
    int mLastMatch;
    bool Match(int from);

 public:
    Regex(System::String ^regularExpression);
    ~Regex();
    !Regex();

    bool Match(System::String ^value);
    bool Match(System::String ^value, int startIndex, int length);
    bool Match(cli::array<wchar_t> ^value, int startIndex, int length);
    bool Match(System::IntPtr data, System::IntPtr lengthAccessor, System::IntPtr charAccessor, int startIndex, int length);
    bool NextMatch();

    property int MatchesCount
    {
        int get();
    };

    int Start(int group);
    int End(int group);
    int Length(int group);
    System::String ^Value(int group);
    System::String ^Name(int group);
    int IndexOf(System::String ^group);
};

}
}
}
