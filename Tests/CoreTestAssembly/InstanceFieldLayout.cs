#pragma warning disable CS0169 // Field 'x' is never used

namespace CoreTestAssembly
{
    class Class1
    {
        int MyInt;
        bool MyBool;
        char MyChar;
        string MyString;
        byte[] MyByteArray;
        Class1 MyClass1SelfRef;
    }

    public struct Struct0
    {
        bool b1;
        bool b2;
        bool b3;
        int i1;
        string s1;
    }

    struct Struct1
    {
        Struct0 MyStruct0;
        bool MyBool;
    }
}
