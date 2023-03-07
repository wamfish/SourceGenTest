# SourceGenTest

The SourceGenTest solution: This solution is not meant to be run. I created this solution to figure out an issue I was having
with a souce code generator I have been working on. 

This solution contains 3 projects:

TestApp:<BR><BR>
    The AppTestDerived class is a partial class that has a base class of RecData that is defined in TestLib.
    TestCodeGen generates source for this class and compiles with 0 erros, but under 
    Dependencies/Analysers/TestCodeGen/TestCodeGen.CodeGen we get: This generator is not generating files.<BR><BR>
TestLib:<BR><BR>
    The LibTestDerived class is a partial class that has a base class of RecData that is defined in TestLib.
    TestCodeGen generates source for this class and compiles with 0 erros and 
    Dependencies/Analysers/TestCodeGen/TestCodeGen.CodeGen works as expected.<BR><BR>
TestCodeGen:<BR><BR>
    This is the source code generator project that both TestLib and Testapp reference.

I have noticed if I comment out the StatusCode field in RecData, so that only the abstract methods remain, TestApp
will also work as expected. I do not know if this scenario is unsported by Source code generators, or if this a bug?
