namespace GenericFields
{
    internal static class TestRunner
    {
        public static int Main()
        {
            int result = InstanceAssignmentClass.RunTests(); if (result != 0) return result;
            result = InstanceAssignmentStruct.RunTests(); if (result != 0) return result;

            // TODO: Requires boxing
            //result = InstanceEqualNullClass.RunTests(); if (result != 0) return result;
            //result = InstanceEqualNullStruct.RunTests(); if (result != 0) return result;

            // TODO: Fails - statics in generic types not yet implemented
            //int result = StaticAssignmentClass.RunTests(); if (result != 0) return result;            
            //int result = StaticAssignmentStruct.RunTests(); if (result != 0) return result;

            return result;
        }
    }
}
