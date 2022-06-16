class CActiveScriptEngine;

namespace gehtsoft
{
namespace xce
{
namespace script
{
namespace engine
{

public ref class ScriptEngineException : public Exception
{
 private:
    String ^mMessage;
    int mHr;
    String ^mSourceName;
    int mLine;
    int mColumn;
    String ^mDescription;
    String ^mHrDescription;
 internal:
    ScriptEngineException(int hr);
    ScriptEngineException(int hr, String^ source, int line, int column, String^ description);
 public:
    ScriptEngineException(SerializationInfo ^info, StreamingContext context);

    [SecurityPermissionAttribute(SecurityAction::Demand, SerializationFormatter=true)]
    virtual void GetObjectData(SerializationInfo ^info, StreamingContext context) override;

    property String ^Message
    {
        virtual String ^get() override;
    };

    property int HResult
    {
       virtual int get();
    };

    property int Line
    {
       virtual int get();
    };

    property int Column
    {
       virtual int get();
    };

    property String^ HResultDescription
    {
       virtual String ^get();
    };

    property String^ Description
    {
       virtual String ^get();
    };

    property String^ SourceName
    {
       virtual String ^get();
    };

};

public ref class ScriptEngine
{
 private:
    ::CActiveScriptEngine *mEngine;
 public:
    ScriptEngine();
    ~ScriptEngine();
    !ScriptEngine();

    void initialize(String ^engine);
    void load(String ^source, String ^name);
    void addObject(String ^name, Object ^object, bool global);
    void connect();
    void invoke(String ^name);
    void invoke(String ^name, cli::array<Object^> ^args, [Out]Object^% ret);
};


}
}
}
}
