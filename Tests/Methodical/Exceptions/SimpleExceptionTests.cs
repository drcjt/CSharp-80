﻿using System;

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

        private static void Throw()
        {
            throw new Exception();
        }

        private static void ThrowIndirect()
        {
            Throw();
        }

        private static int TryCatch_WithThrowInSeparateMethod_IsCaught()
        {
            int result = 1;
            try
            {
                Throw();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        private static int TryCatch_WithThrowInNestedSeparateMethod_IsCaught()
        {
            int result = 1;
            try
            {
                ThrowIndirect();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        private static int TryCatch_WithSpecificExceptionTypes()
        {
            int result = 1;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (NullReferenceException ex)
                {
                    result = 2;
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }

        public static int TryCatchOfNRE_WhenNREThrown_IsCaught()
        {
            int result = 1;
            try
            {
                string s = null;
                Console.WriteLine(s.Length);
            }
            catch (NullReferenceException ex)
            {
                // TODO: return 0 from here caused exception in SSA
                result = 0;
            }

            return result;
        }

        public static int RunTests()
        {
            int result = Try_NoThrow(); if (result != 0) return result;
            result = TryCatch_WithThrow_IsCaught(); if (result != 0) return result;
            result = NestedTryCatch_WithThrows_AreCaught(); if (result != 0) return result;
            result = TryCatch_WithThrowInSeparateMethod_IsCaught(); if (result != 0) return result;
            result = TryCatch_WithThrowInNestedSeparateMethod_IsCaught(); if (result != 0) return result;
            result = TryCatch_WithSpecificExceptionTypes(); if (result != 0) return result;
            result = TryCatchOfNRE_WhenNREThrown_IsCaught(); if (result != 0) return result;

            return result;
        }
    }
}
