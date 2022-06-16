#pragma unmanaged
#include "cregexp/cregexp1.h"
#include "unicode/String.h"
#include "unicode/EString.h"

#pragma managed
#using <mscorlib.dll>
using namespace System::Runtime::InteropServices;
#include "Regex.h"

namespace gehtsoft
{
namespace xce
{
namespace colorer
{

Regex::Regex(System::String ^regularExpression)
{
    mValue = 0;
    mMatches = 0;
    mRegex = 0;

     if (regularExpression == nullptr)
        throw gcnew System::ArgumentNullException(L"regularExpression");
     if (regularExpression->Length == 0)
        throw gcnew System::ArgumentException(L"An empty string cannot be a regular expression", L"regularExpression");

    System::IntPtr _regularExpression;
    _regularExpression = Marshal::StringToCoTaskMemUni(regularExpression);
    ::SString *__regularExpression = new ::SString((const wchar *)_regularExpression.ToPointer(), 0, regularExpression->Length);

    try
    {
        mRegex = new ::CRegExp1(__regularExpression);
        if (!mRegex->isOk())
        {
            EError1 error = mRegex->getError();
            delete mRegex;
            mRegex = 0;

            switch (error)
            {
            case    ESYNTAX:
                    throw gcnew System::ArgumentException(L"The syntax error in the regular expression", L"regularExpression");
            case    EBRACKETS:
                    throw gcnew System::ArgumentException(L"The brackets aren't balanced", L"regularExpression");
            case    EENUM:
                    throw gcnew System::ArgumentException(L"The invalid character class is used", L"regularExpression");
            case    EOP:
                    throw gcnew System::ArgumentException(L"The invalid operation is used", L"regularExpression");
            case    EError:
            default:
                    throw gcnew System::ArgumentException(L"The regular expression is malformed", L"regularExpression");

            }
        }
        mRegex->setPositionMoves(true);
    }
    finally
    {
        delete __regularExpression;
        Marshal::FreeCoTaskMem(_regularExpression);
    }
}

Regex::~Regex()
{
    this->!Regex();
}

Regex::!Regex()
{
    if (mRegex != 0)
    {
        delete mRegex;
        mRegex = 0;
    }
    if (mMatches != 0)
    {
        delete mMatches;
        mMatches = 0;
    }
    if (mValue != 0)
    {
        delete mValue;
        mValue = 0;
    }
}

bool Regex::Match(System::String ^value)
{
    return Match(value, 0, value->Length);
}

bool Regex::Match(System::String ^value, int startIndex, int length)
{
    if (length == 0)
       return false;

    if (mRegex == 0)
        throw gcnew System::InvalidOperationException();

    if (value == nullptr)
        throw gcnew System::ArgumentNullException("value");

    if (startIndex < 0 || startIndex >= value->Length)
        throw gcnew System::ArgumentOutOfRangeException("startIndex");

    if (length < 1 || startIndex + length > value->Length)
        throw gcnew System::ArgumentOutOfRangeException("length");

    mStartIndex = startIndex;

    if (mValue != 0)
    {
        delete mValue;
        mValue = 0;
    }
     System::IntPtr _value;
    _value = Marshal::StringToCoTaskMemUni(value);
    mValue = new ::SString(((const wchar *)(_value.ToPointer())) + startIndex, 0, length);
    Marshal::FreeCoTaskMem(_value);

    return Match(0);
}

bool Regex::Match(cli::array<wchar_t> ^value, int startIndex, int length)
{
    if (mRegex == 0)
        throw gcnew System::InvalidOperationException();

    if (value == nullptr)
        throw gcnew System::ArgumentNullException("value");

    if (startIndex < 0 || startIndex >= value->Length)
        throw gcnew System::ArgumentOutOfRangeException("startIndex");

    if (length < 1 || startIndex + length > value->Length)
        throw gcnew System::ArgumentOutOfRangeException("length");

    mStartIndex = startIndex;

    if (mValue != 0)
    {
        delete mValue;
        mValue = 0;
    }

    GCHandle ^pinned = GCHandle::Alloc(value, GCHandleType::Pinned);
    System::IntPtr src =  Marshal::UnsafeAddrOfPinnedArrayElement(value, startIndex);
    mValue = new ::SString((const wchar *)src.ToPointer(), 0, length);
    pinned->Free();
    return Match(0);
}

bool Regex::Match(System::IntPtr _data, System::IntPtr _lengthAccessor, System::IntPtr _charAccessor, int startIndex, int length)
{
    void *data = (void *)_data.ToPointer();
    ::estring_length_ptr lengthAccessor = (::estring_length_ptr)_lengthAccessor.ToPointer();
    ::estring_char_ptr charAccessor = (::estring_char_ptr)_charAccessor.ToPointer();
    mStartIndex = 0;
    mValue = new ::EString(data, lengthAccessor, charAccessor);
    return Match(startIndex);
}

bool Regex::NextMatch()
{
    return Match(mLastMatch);
}

bool Regex::Match(int from)
{
    if (mRegex == 0 || mValue == 0)
        throw gcnew System::InvalidOperationException();
    if (mMatches == 0)
        mMatches = new SMatches1;
    memset(mMatches, 0, sizeof(SMatches1));
    bool rc = mRegex->parse(mValue, from, mValue->length(), mMatches);
    if (rc)
        mLastMatch = mMatches->e[0];
    else
        mLastMatch = from;
    return rc;
}

int Regex::MatchesCount::get()
{
    if (mMatches == 0)
        return 0;
    return mMatches->cMatch;
}

int Regex::Start(int group)
{
    if (mMatches == 0 || group < 0 || group >= mMatches->cMatch)
        throw gcnew System::ArgumentOutOfRangeException(L"group");
    return mStartIndex + mMatches->s[group];
}

int Regex::End(int group)
{
    if (mMatches == 0 || group < 0 || group >= mMatches->cMatch)
        throw gcnew System::ArgumentOutOfRangeException(L"group");
    return mStartIndex + mMatches->e[group];
}

int Regex::Length(int group)
{
    if (mMatches == 0 || group < 0 || group >= mMatches->cMatch)
        throw gcnew System::ArgumentOutOfRangeException(L"group");
    return mMatches->e[group] - mMatches->s[group];
}

System::String ^Regex::Value(int group)
{
    if (mValue == 0)
        throw gcnew System::InvalidOperationException();
    if (mMatches == 0 || group < 0 || group >= mMatches->cMatch)
        throw gcnew System::ArgumentOutOfRangeException(L"group");

    int offset = mMatches->s[group] + mStartIndex;
    int length = mMatches->e[group] - offset;

    if (length < 1)
        return L"";



    if (length > 0)
    {
        wchar *t = new wchar[length];
        for (int i = 0; i < length; i++)
            t[i] = mValue->operator[](offset + i);
        System::String ^s = Marshal::PtrToStringUni(System::IntPtr(t), length);
        delete t;
        return s;
    }
    else
        return L"";
}

System::String ^Regex::Name(int group)
{
    if (mRegex == 0)
        throw gcnew System::InvalidOperationException();
    if (mMatches == 0 || group < 0 || group >= mMatches->cMatch)
        throw gcnew System::ArgumentOutOfRangeException(L"group");
    ::String *name = mRegex->getBracketName(group);
    if (name == 0)
        return nullptr;
    wchar *t = const_cast<wchar *>(name->getWChars());

    if (t == 0)
        return nullptr;
    return Marshal::PtrToStringUni(System::IntPtr(t), name->length());
}

int Regex::IndexOf(System::String ^group)
{
    if (group == nullptr)
        throw gcnew System::ArgumentNullException(L"group");
    if (mRegex == 0)
        throw gcnew System::InvalidOperationException();
    if (group->Length == 0)
        return -1;

     System::IntPtr _group;
    _group = Marshal::StringToCoTaskMemUni(group);
    ::SString *__group = new ::SString((const wchar *)_group.ToPointer(), 0, group->Length);
    try
    {
        return mRegex->getBracketNo(__group);
    }
    finally
    {
        delete __group;
        Marshal::FreeCoTaskMem(_group);
    }
}

}
}
}
