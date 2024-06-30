namespace GenericLocals
{
    internal static class TestRunner
    {
        public static int Main()
        {
            var result = InstanceAssignmentClass.RunTests();
            result &= InstanceAssignmentStruct.RunTests();

            result &= InstanceEqualNullClass.RunTests();
            result &= InstanceEqualNullStruct.RunTests();

            result &= StaticAssignmentClass.RunTests();
            result &= StaticAssignmentStruct.RunTests();

            result &= StaticEqualNullClass.RunTests();
            result &= StaticEqualNullStruct.RunTests();

            return result ? 0 : 1;
        }
    }
}