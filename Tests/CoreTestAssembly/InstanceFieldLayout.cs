#pragma warning disable CS0169 // Field 'x' is never used

using System.Runtime.InteropServices;

namespace CoreTestAssembly
{
    [StructLayout(LayoutKind.Sequential)]
    class Class1
    {
        int MyInt;
        bool MyBool;
        char MyChar;
        string MyString;
        byte[] MyByteArray;
        Class1 MyClass1SelfRef;
    }

    [StructLayout(LayoutKind.Sequential)]
    class Class2 : Class1
    {
        int MyInt2;
    }


    // [StructLayout(LayoutKind.Sequential)] is applied by default by the C# compiler
    public struct Struct0
    {
        bool b1;
        bool b2;
        bool b3;
        int i1;
        string s1;
    }

    // [StructLayout(LayoutKind.Sequential)] is applied by default by the C# compiler
    struct Struct1
    {
        Struct0 MyStruct0;
        bool MyBool;
    }

    unsafe struct Struct2
    {
        bool b1;
        fixed char fixedBuffer[25];
        int i1;
    }
}
