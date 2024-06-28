namespace GenericLocals
{
    internal static class TestRunner
    {
        public static int Main()
        {
            int result = InstanceAssignmentClass.RunTests(); if (result != 0) return result;
            result = InstanceAssignmentStruct.RunTests(); if (result != 0) return result;

            result = InstanceEqualNullClass.RunTests(); if (result != 0) return result;
            result = InstanceEqualNullStruct.RunTests(); if (result != 0) return result;

            result = StaticAssignmentClass.RunTests(); if (result != 0) return result;            
            result = StaticAssignmentStruct.RunTests(); if (result != 0) return result;

            result = StaticEqualNullClass.RunTests(); if (result != 0) return result;
            result = StaticEqualNullStruct.RunTests(); if (result != 0) return result;

            return result;
        }
    }
}