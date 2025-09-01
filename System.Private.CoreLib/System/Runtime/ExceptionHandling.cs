using Internal.Runtime;
using System.Diagnostics;

namespace System.Runtime
{
    public struct ExInfo
    {
        internal StackFrameIterator _frameIter;
        internal object _exception;
    }

    internal static unsafe class ExceptionHandling
    {
        private struct EHClause
        {
            internal ushort _tryStartOffset;
            internal ushort _tryEndOffset;
            internal byte* _handlerAddress;
            internal void* _pTargetType;
        }

        [RuntimeExport("ThrowException")]
        public static void ThrowException(ref ExInfo exceptionInfo)
        {
            exceptionInfo._exception ??= new NullReferenceException();

            DispatchException(ref exceptionInfo);
        }

        [RuntimeExport("ThrowDivideByZeroException")]
        public static void ThrowDivideByZeroException()
        {
            throw new DivideByZeroException();
        }

        private static void DispatchException(ref ExInfo exceptionInfo)
        {
            object exceptionObj = exceptionInfo._exception;

            byte* pCatchHandler = null;
            // enumerate frames
            bool isValid = true;
            for (; isValid; isValid = InternalCalls.SFINext(ref exceptionInfo._frameIter))
            {
                byte* pHandler;
                if (FindFirstPassHandler(exceptionObj, ref exceptionInfo._frameIter, out pHandler))
                {
                    // Found a handler
                    pCatchHandler = pHandler;
                    break;
                }
            }

            if (pCatchHandler == null)
            {
                // Treat everything that gets here as unhandled exceptions
                UnhandledExceptionFailFast(ref exceptionInfo);
            }

            InternalCalls.CallCatchHandler(exceptionObj, pCatchHandler, ref exceptionInfo._frameIter);
        }

        private static bool FindFirstPassHandler(object exception, ref StackFrameIterator frameIter, out byte* pHandler)
        {
            pHandler = null;

            EHClause nextClause;
            byte* ehEnum;

            InternalCalls.EHEnumInit(&ehEnum);

            for (int idx = 0; InternalCalls.EHEnumNext(&ehEnum, &nextClause); idx++)
            {
                var catchEETypePtr = (EEType*)(nextClause._pTargetType);

                if (frameIter.InstructionPointer >= nextClause._tryStartOffset && frameIter.InstructionPointer < nextClause._tryEndOffset &&
                    TypeCast.IsInstanceOfClass(catchEETypePtr, exception) != null)
                {
                    pHandler = nextClause._handlerAddress;
                    return true;
                }
            }

            return false;
        }

        private static void UnhandledExceptionFailFast(ref ExInfo exceptionInfo)
        {
            if (exceptionInfo._exception is Exception exceptionObject)
            {
                Console.Write("Unhandled exception. ");

                Console.WriteLine(exceptionObject.Message ?? "");

                Console.Write("IP="); 
                Console.Write(exceptionInfo._frameIter.InstructionPointer);
                Console.Write(",FP=");
                Console.WriteLine(exceptionInfo._frameIter.FramePointer);
            }

            Environment.Exit(-1);
        }
    }
}
