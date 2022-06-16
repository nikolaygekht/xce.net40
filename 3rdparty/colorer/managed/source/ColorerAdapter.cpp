#pragma unmanaged
#include "colorer/parserFactory.h"
#include "colorer/editor/baseEditor.h"
#include "ColorerAdapterNative.h"

/** Constructor.

    @param getLinePtr The callback for getting the line
  */
CColorerLineSourceAdapter::CColorerLineSourceAdapter(adapter_getline_ptr getLinePtr)
{
    mGetLinePtr = getLinePtr;
    mLastString = 0;
}

/** Destructor. */
CColorerLineSourceAdapter::~CColorerLineSourceAdapter()
{
    if (mLastString != 0)
        delete mLastString;
    mLastString = 0;
}

/** Overridable: Job started.

    @param lno      First line number.
  */
void CColorerLineSourceAdapter::startJob(int lno)
{
}

/** Overridable: Job finished.

    @param lno      Last line number.
  */
void CColorerLineSourceAdapter::endJob(int lno)
{
    if (mLastString != 0)
        delete mLastString;
    mLastString = 0;
}

/** Overridable: Get contents of specified string

    @param lno      Line number to get

    @return         String object, which should be valid
                    until next call of getLine() or
                    endJob() If requested line can't be returned,
                    fe there is no line with the passed index, method
                    must return null.
  */
String *CColorerLineSourceAdapter::getLine(int lno)
{
    if (mLastString != 0)
        delete mLastString;
    mLastString = 0;
    mLastString = mGetLinePtr(lno);
    return mLastString;
}

#pragma managed
#using <mscorlib.dll>
using namespace System::Runtime::InteropServices;
#include "ColorerAdapter.h"

namespace gehtsoft
{
namespace xce
{
namespace colorer
{


::SString *LineSourceAdapter::GetLineImpl(int lno)
{
    if (lno < 0 || lno >= mSource->GetLinesCount())
        return 0;
    int length = mSource->GetLineLength(lno);
    if (length == 0)
        return new ::SString("");

    if (length >= mTemp->Length)
        mTemp = gcnew cli::array<wchar_t>((length / 1024 + 1) * 1024);
    mSource->GetLine(lno, mTemp, 0, length);
    GCHandle ^pinned = GCHandle::Alloc(mTemp, GCHandleType::Pinned);
    System::IntPtr src =  Marshal::UnsafeAddrOfPinnedArrayElement(mTemp, 0);
    ::SString *ret = new ::SString((const wchar *)src.ToPointer(), 0, length);
    pinned->Free();
    return ret;
}

LineSourceAdapter::LineSourceAdapter(ILineSource ^source)
{
    mSource = source;
    mManagedDelegate = gcnew adapter_getline_managed(this, &LineSourceAdapter::GetLineImpl);
    mNativeAdapter = new ::CColorerLineSourceAdapter((::adapter_getline_ptr)Marshal::GetFunctionPointerForDelegate(mManagedDelegate).ToPointer());
    mTemp = gcnew cli::array<wchar_t>(1024);
}

LineSourceAdapter::~LineSourceAdapter()
{
    this->!LineSourceAdapter();
}

LineSourceAdapter::!LineSourceAdapter()
{
    if (mNativeAdapter != 0)
        delete mNativeAdapter;
    mNativeAdapter = 0;
}

::CColorerLineSourceAdapter *LineSourceAdapter::getNativeAdapter()
{
    return mNativeAdapter;
}

}
}
}



