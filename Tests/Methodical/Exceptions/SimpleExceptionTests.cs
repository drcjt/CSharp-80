using System;

namespace Exceptions
{
    internal class VirtualMethodExceptionBase
    {
        public virtual void Test() { }
    }

    internal class VirtualMethodExceptionDerivedThrows : VirtualMethodExceptionBase
    {
        public override void Test()
        {
            throw new Exception();
        }
    }

    internal class VirtualMethodExceptionDerivedDoesNotThrow : VirtualMethodExceptionBase
    {
        public override void Test()
        {
        }
    }

    interface InterfaceWithManyParameters
    {
        int ManyParameters(int a, int b, int c, int d, int e, int f, int g, int h);
    }

    class ClassWithManyParameters : InterfaceWithManyParameters
    {
        public int ManyParameters(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            if (a > 0)
                throw new Exception();
            else
                return 42;
        }
    }

    internal static class SimpleExceptionTests
    {
        private static int Try_VirtualMethodCallNoThrow()
        {
            var testClass = new VirtualMethodExceptionDerivedDoesNotThrow();

            int result = 0;

            try
            {
                testClass.Test();
            }
            catch
            {
                result = 1;
            }

            return result;
        }

        private static int TryCatch_VirtualMethodCallThrows_IsCaught()
        {
            var testClass = new VirtualMethodExceptionDerivedThrows();

            int result = 1;

            try
            {
                testClass.Test();
            }
            catch
            {
                result = 0;
            }

            return result;
        }

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

        private static void Throw(int parameter)
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
                return 0;
            }

            return result;
        }

        public unsafe static int TryCatch_WithThrowingMethodHavingAParameter_StackIsRestoredCorrectly()
        {
            int result = 1;
            int count = 0;
            ushort[] stackAllocedVariables = new ushort[2];
            while (count < 2)
            {
                try
                {
                    var stackTest = stackalloc int[1];
                    stackAllocedVariables[count] = (ushort)stackTest;

                    Throw(count);
                }
                catch
                {
                }
                count++;
            }

            result = stackAllocedVariables[0] - stackAllocedVariables[1] != 4 ? 1 : 0;
            return result;
        }

        public static int TryCatch_ThrowBeforeTry_IsNotCaughtByInnerTry()
        {
            int result = 2;
            try
            {
                Throw();
                try
                {
                }
                catch
                {
                    result = 1;
                }
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        private class TestException : Exception
        {
        }

        public static void TryCatch_WithCatchType_CatchTypeIsAddedAsDependency()
        {
            try
            {

            }
            catch (TestException)
            {
            }
        }

        public static int TryCatch_WithInterfaceMethodWithManyParameters_UnwindsCorrectly()
        {
            int numExceptions = 0;

            InterfaceWithManyParameters testInstance = new ClassWithManyParameters();
            int a = -1;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Console.WriteLine(a);
                    testInstance.ManyParameters(a, 0, 0, 0, 0, 0, 0, 0);
                }
                catch
                {
                    numExceptions++;
                }
                a = -a;
            }

            Console.WriteLine("Number of exceptions");
            Console.Write(numExceptions);

            return numExceptions == 1 ? 0 : 1;
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
            result = TryCatch_WithThrowingMethodHavingAParameter_StackIsRestoredCorrectly(); if (result != 0) return result;
            result = TryCatch_ThrowBeforeTry_IsNotCaughtByInnerTry(); if (result != 0) return result;
            result = Try_VirtualMethodCallNoThrow(); if (result != 0) return result;
            result = TryCatch_VirtualMethodCallThrows_IsCaught(); if (result != 0) return result;
            result = TryCatch_WithInterfaceMethodWithManyParameters_UnwindsCorrectly(); if (result != 0) return result;

            TryCatch_WithCatchType_CatchTypeIsAddedAsDependency();

            return result;
        }
    }
}
