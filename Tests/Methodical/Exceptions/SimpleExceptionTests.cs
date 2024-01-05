using System;

namespace Exceptions
{
    internal static class SimpleExceptionTests
    {
        private static int Try_NoThrow()
        {
            int result;

            try
            {
                result = 0;
            }
            catch
            {
                result = 1;
            }

            return result;
        }

        private static int TryCatch_WithThrow_IsCaught()
        {
            int result = 1;
            try
            {
                throw new Exception();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        private static int NestedTryCatch_WithThrows_AreCaught()
        {
            int result = 1;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch 
                {
                }
                throw new Exception();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        public static int RunTests()
        {
            int result = Try_NoThrow(); if (result != 0) return result;
            result = TryCatch_WithThrow_IsCaught(); if (result != 0) return result;
            result = NestedTryCatch_WithThrows_AreCaught(); if (result != 0) return result;

            return result;
        }
    }
}
