namespace GenericFields
{
    internal static class TestRunner
    {
        public static int Main()
        {
            bool result = true;

            result = result && InstanceAssignmentClass.RunTests();
            result = result && InstanceAssignmentStruct.RunTests();

            result = result && InstanceEqualNullClass.RunTests();
            result = result && InstanceEqualNullStruct.RunTests();

            result = result && StaticAssignmentClass.RunTests();
            result = result && StaticAssignmentStruct.RunTests();

            result = result && StaticEqualNullClass.RunTests();
            result = result && StaticEqualNullStruct.RunTests();

            return result ? 0 : 1;
        }
    }
}
