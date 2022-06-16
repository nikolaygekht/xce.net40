#pragma unmanaged
#include "stdhead.h"

#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Runtime::InteropServices;

#include "TextClipboard.h"


namespace gehtsoft
{
namespace xce
{
namespace conio
{

bool TextClipboard::IsTextAvailable(TextClipboardFormat format)
{
    if (!::OpenClipboard(0))
        return false;
    bool rc = ::IsClipboardFormatAvailable((UINT)format) != 0;
    ::CloseClipboard();
    return rc;
}

String ^TextClipboard::GetText(TextClipboardFormat format)
{
    if (!::OpenClipboard(0))
        return nullptr;

    try
    {
        HANDLE hdata = ::GetClipboardData((UINT)format);
        if (!hdata)
        {
            ::CloseClipboard();
            return nullptr;
        }

        const void *text = (const char *)::GlobalLock(hdata);
        String ^ret = nullptr;

        switch (format)
        {
        case    TextClipboardFormat::Text:
                ret = Marshal::PtrToStringAnsi(IntPtr(const_cast<void *>(text)));
                break;
        case    TextClipboardFormat::UnicodeText:
                ret = Marshal::PtrToStringUni(IntPtr(const_cast<void *>(text)));
                break;
        }
        ::GlobalUnlock(hdata);
        return ret;
    }
    finally
    {
        ::CloseClipboard();
    }
}

void TextClipboard::SetText(String ^text, TextClipboardFormat format)
{
    if (!::OpenClipboard(0))
        return ;
    try
    {
        ::EmptyClipboard();
        IntPtr buff;
        int buffLengthInBytes;


        switch (format)
        {
        case    TextClipboardFormat::Text:
                buff = Marshal::StringToCoTaskMemAnsi(text);
                buffLengthInBytes = ::strlen((const char *)buff.ToPointer()) + 1;
                break;
        case    TextClipboardFormat::UnicodeText:
                buff = Marshal::StringToCoTaskMemUni(text);
                buffLengthInBytes = (::wcslen((const wchar_t *)buff.ToPointer()) + 1) * 2;
                break;
        default:
                break;

        }
        HANDLE hdata = ::GlobalAlloc(GMEM_MOVEABLE | GMEM_DDESHARE, buffLengthInBytes);
        void *pdata = reinterpret_cast<void *>(::GlobalLock(hdata));
        memcpy_s(pdata, buffLengthInBytes, buff.ToPointer(), buffLengthInBytes);
        ::GlobalUnlock(hdata);
        Marshal::FreeCoTaskMem(buff);
        ::SetClipboardData((UINT)format, hdata);
    }
    finally
    {
        ::CloseClipboard();
    }
}

String ^TextClipboard::GetText()
{
    String ^text = nullptr;
    if (IsTextAvailable(TextClipboardFormat::UnicodeText))
        text = GetText(TextClipboardFormat::UnicodeText);
    if (text == nullptr && IsTextAvailable(TextClipboardFormat::Text))
        text = GetText(TextClipboardFormat::Text);
    return text;
}

void TextClipboard::SetText(String ^text)
{
    SetText(text, TextClipboardFormat::UnicodeText);
}


TextClipboard::TextClipboard()
{
}


}
}
}
