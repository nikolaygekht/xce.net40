#pragma unmanaged
#include <windows.h>
#include <ole2.h>
#include <rpc.h>
#include <comdef.h>
#include "win32\rpcsal.h"
#include "win32\activscp.h"
#include "win32\activdbg.h"
#include <vector>
#include "native/engine.h"
#pragma managed

#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::Serialization;
using namespace System::Security::Permissions;
#include "engine1.h"

namespace gehtsoft
{
namespace xce
{
namespace script
{
namespace engine
{

ScriptEngineException::ScriptEngineException(int hr)
{
    mHr = hr;
    mHrDescription = Marshal::PtrToStringUni(IntPtr((void *)CActiveScriptEngine::getErrorMsg((HRESULT)hr)));

    mSourceName = L"";
    mLine = 0;
    mColumn = 0;
    mDescription = L"";
    mMessage = nullptr;

}

ScriptEngineException::ScriptEngineException(int hr, String^ source, int line, int column, String^ description)
{
    mHr = hr;
    mHrDescription = Marshal::PtrToStringUni(IntPtr((void *)CActiveScriptEngine::getErrorMsg((HRESULT)hr)));

    mSourceName = source;
    mLine = line;
    mColumn = column;
    mDescription = description;
}

ScriptEngineException::ScriptEngineException(SerializationInfo ^info, StreamingContext context) : Exception(info, context)
{
    try
    {
        mHr = info->GetInt32("hr");
        mHrDescription = Marshal::PtrToStringUni(IntPtr((void *)CActiveScriptEngine::getErrorMsg((HRESULT)mHr)));
    }
    catch (Exception ^)
    {
        mHr = 0;
        mHrDescription = L"";
    }

    try
    {
        mSourceName = info->GetString("source");
    }
    catch (Exception ^)
    {
        mSourceName = L"";
    }

    try
    {
        mMessage = info->GetString("description");
    }
    catch (Exception ^)
    {
        mDescription = "";
    }

    try
    {
        mLine = info->GetInt32("line");
    }
    catch (Exception ^)
    {
        mLine = 0;
    }


    try
    {
        mColumn = info->GetInt32("column");
    }
    catch (Exception ^)
    {
        mColumn = 0;
    }

    mMessage = nullptr;
}

void ScriptEngineException::GetObjectData(SerializationInfo ^info, StreamingContext context)
{
    Exception::GetObjectData(info, context);
    info->AddValue("hr", mHr);
    info->AddValue("line", mLine);
    info->AddValue("column", mColumn);
    info->AddValue("source", mSourceName);
    info->AddValue("description", mDescription);
}

String ^ScriptEngineException::Message::get()
{
    if (mMessage == nullptr)
    {
        if (mLine > 0 && mSourceName->Length > 0)
            mMessage = String::Format("{0:x} {1}\n{2} ({3},{4}) - {5}", mHr, mHrDescription, mSourceName, mLine, mColumn, mDescription);
        else
            mMessage = String::Format("{0:x} {1}", mHr, mHrDescription);
    }
    return mMessage;
};

int ScriptEngineException::HResult::get()
{
    return mHr;
};

int ScriptEngineException::Line::get()
{
    return mLine;
};


int ScriptEngineException::Column::get()
{
    return mColumn;
};


String ^ScriptEngineException::HResultDescription::get()
{
    return mHrDescription;
};

String^ ScriptEngineException::Description::get()
{
    return mDescription;
};

String^ ScriptEngineException::SourceName::get()
{
   return mSourceName;
};

ScriptEngine::ScriptEngine()
{
    mEngine = 0;
}


ScriptEngine::~ScriptEngine()
{
    this->!ScriptEngine();
}

ScriptEngine::!ScriptEngine()
{
    if (mEngine != 0)
    {
        delete mEngine;
        mEngine = 0;
    }

}

void ScriptEngine::initialize(String ^engine)
{
    if (engine == nullptr)
        throw gcnew ArgumentNullException("engine");

    IntPtr _engine = Marshal::StringToCoTaskMemUni(engine);

    try
    {
        if (mEngine != 0)
        {
            delete mEngine;
            mEngine = 0;
        }
        mEngine = new CActiveScriptEngine();
        HRESULT hr = mEngine->init((const wchar_t *)_engine.ToPointer());
        if (FAILED(hr))
        {
            delete mEngine;
            mEngine = 0;

            throw gcnew COMException(String::Format("{0:x} {1}", hr, gcnew String(mEngine->getErrorMsg(hr))), hr);
        }
    }
    finally
    {
        Marshal::FreeCoTaskMem(_engine);
    }
}

void ScriptEngine::load(String ^source, String ^name)
{
    if (source == nullptr)
        throw gcnew ArgumentNullException("source");
    if (name == nullptr)
        throw gcnew ArgumentNullException("name");
    if (mEngine == 0)
        throw gcnew InvalidOperationException();

    IntPtr _source = Marshal::StringToCoTaskMemUni(source);
    IntPtr _name = Marshal::StringToCoTaskMemUni(name);

    try
    {
        HRESULT hr = mEngine->load((const wchar_t *)_source.ToPointer(), (const wchar_t *)_name.ToPointer());
        if (FAILED(hr))
        {
            throw gcnew ScriptEngineException(hr, Marshal::PtrToStringUni(IntPtr((void *)mEngine->getLastErrorContext())),
                                                  mEngine->getLastErrorLine() + 1,
                                                  mEngine->getLastErrorPos() + 1,
                                                  Marshal::PtrToStringUni(IntPtr((void *)mEngine->getErrorDescription())));
        }
    }
    finally
    {
        Marshal::FreeCoTaskMem(_source);
        Marshal::FreeCoTaskMem(_name);
    }
}

void ScriptEngine::addObject(String ^name, Object ^object, bool global)
{
    if (name == nullptr)
        throw gcnew ArgumentNullException("name");
    if (object == nullptr)
        throw gcnew ArgumentNullException("object");
    if (mEngine == 0)
        throw gcnew InvalidOperationException();

    IntPtr _name = Marshal::StringToCoTaskMemUni(name);
    IntPtr _object = Marshal::GetIUnknownForObject(object);

    try
    {
        HRESULT hr = mEngine->addObject((const wchar_t *)_name.ToPointer(), (IUnknown *)_object.ToPointer(), global);
        if (FAILED(hr))
            throw gcnew COMException(String::Format("{0:x} {1}", hr, gcnew String(mEngine->getErrorMsg(hr))), hr);
    }
    finally
    {
        Marshal::Release(_object);
        Marshal::FreeCoTaskMem(_name);
    }

}

void ScriptEngine::connect()
{
    HRESULT hr = mEngine->connect();
    if (FAILED(hr))
        throw gcnew COMException(String::Format("{0:x} {1}", hr, gcnew String(mEngine->getErrorMsg(hr))), hr);
}

void ScriptEngine::invoke(String ^name)
{
    if (name == nullptr)
        throw gcnew ArgumentNullException("name");
    if (mEngine == 0)
        throw gcnew InvalidOperationException();

    IntPtr _name = Marshal::StringToCoTaskMemUni(name);
    VARIANT r;
    VariantInit(&r);

    try
    {
        HRESULT hr = mEngine->invoke((const wchar_t *)_name.ToPointer(), 0, 0, &r);
        if (FAILED(hr))
        {
            if (mEngine->getLastErrorLine() >= 0)
            {
                throw gcnew ScriptEngineException(hr, Marshal::PtrToStringUni(IntPtr((void *)mEngine->getLastErrorContext())),
                                                      mEngine->getLastErrorLine() + 1,
                                                      mEngine->getLastErrorPos() + 1,
                                                      Marshal::PtrToStringUni(IntPtr((void *)mEngine->getErrorDescription())));
            }
            else
            {
                throw gcnew COMException(String::Format("{0:x} {1}", hr, gcnew String(mEngine->getErrorMsg(hr))), hr);
            }
        }
    }
    finally
    {
        VariantClear(&r);
        Marshal::FreeCoTaskMem(_name);

    }
}

void ScriptEngine::invoke(String ^name, cli::array<Object^> ^args, [Out]Object^% ret)
{
    if (name == nullptr)
        throw gcnew ArgumentNullException("name");
    if (mEngine == 0)
        throw gcnew InvalidOperationException();

    IntPtr _name = Marshal::StringToCoTaskMemUni(name);
    VARIANT r;
    VariantInit(&r);
    VARIANT *_args = 0;
    int _argc = 0;

    if (args != nullptr && args->Length > 0)
    {
        _args = new VARIANT[args->Length];
        for (int i = 0; i < args->Length; i++)
            VariantInit(_args + i);
        _argc = args->Length;
    }

    try
    {
        if (_argc > 0)
        {
            _args = new VARIANT[args->Length];
            for (int i = 0; i < args->Length; i++)
            {
                if (args[i] != nullptr)
                    Marshal::GetNativeVariantForObject(args[i], IntPtr((void *)(_args + args->Length - (i + 1))));
            }
        }
        HRESULT hr = mEngine->invoke((const wchar_t *)_name.ToPointer(), _argc, _args, &r);
        if (FAILED(hr))
        {
            if (mEngine->getLastErrorLine() >= 0)
            {
                throw gcnew ScriptEngineException(hr, Marshal::PtrToStringUni(IntPtr((void *)mEngine->getLastErrorContext())),
                                                      mEngine->getLastErrorLine() + 1,
                                                      mEngine->getLastErrorPos() + 1,
                                                      Marshal::PtrToStringUni(IntPtr((void *)mEngine->getErrorDescription())));
            }
            else
            {
                throw gcnew COMException(String::Format("{0:x} {1}", hr, gcnew String(mEngine->getErrorMsg(hr))), hr);
            }
        }

        if (r.vt != VT_EMPTY && r.vt != VT_NULL)
        {
            ret = Marshal::GetObjectForNativeVariant(IntPtr((void *)&r));
        }
        else
        {
            ret = nullptr;
        }
    }
    finally
    {
        if (_args != 0 && _argc > 0)
        {
            for (int i = 0; i < _argc; i++)
                VariantClear(_args + i);
            delete _args;
        }
        VariantClear(&r);
        Marshal::FreeCoTaskMem(_name);

    }
}


}
}
}
}

