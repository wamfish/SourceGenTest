/*
    With the StatusCode field included: 
        TestApp under Dependencies/Analysers/TestCodeGen/TestCodeGen.CodeGen we get: This generator is not generating files.
        TestLib works as expected.
    Comment out StatusCode:
        TestApp works as expected.
        TestLib works as expected.
*/
namespace WFLib;
public abstract class RecData
{
    public byte StatusCode;
    public abstract void Clear();
    public abstract void Init();
}
