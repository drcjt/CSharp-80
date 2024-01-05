namespace System.Runtime
{
    public struct ExInfo
    {
        internal ushort stackPointer;
        internal ushort framePointer;
        internal ushort instructionPointer;
    }

    internal static unsafe class ExceptionHandling
    {
        private struct EHClause
        {
            internal ushort _tryStartOffset;
            internal ushort _tryEndOffset;
            internal byte* _handlerAddress;
        }

        [RuntimeExport("ThrowException")]
        public static void ThrowException(object exception, ExInfo exceptionInfo)
        {
            exception ??= new NullReferenceException();

            DispatchException(exception, exceptionInfo);
        }

        private static void DispatchException(object exception, ExInfo exceptionInfo)
        {
            // TODO: Exception dispatch goes here
            // search for appropriate handler
            // then call the handler if found

            byte* pCatchHandler = null;
            // enumerate frames
            // for (; isValid; isValid = frameIter.Next))
            // {
            byte* pHandler;
            if (FindFirstPassHandler(exception, exceptionInfo, out pHandler))
            {
                // Found a handler
                //break;
                pCatchHandler = pHandler;
            }
            // }

            if (pCatchHandler == null)
            {
                // Treat everything that gets here as unhandled exceptions
                UnhandledExceptionFailFast(exception, exceptionInfo);
            }

            InternalCalls.CallCatchHandler(exception, pCatchHandler, ref exceptionInfo);
        }

        private static bool FindFirstPassHandler(object exception, ExInfo exceptionInfo, out byte* pHandler)
        {
            pHandler = null;

            EHClause nextClause;
            byte* ehEnum;

            InternalCalls.EHEnumInit(&ehEnum);

            for (int idx = 0; InternalCalls.EHEnumNext(&ehEnum, &nextClause); idx++)
            {
                if (exceptionInfo.instructionPointer >= nextClause._tryStartOffset && exceptionInfo.instructionPointer < nextClause._tryEndOffset)
                {
                    pHandler = nextClause._handlerAddress;
                    return true;
                }
            }

            return false;
        }

        private static void UnhandledExceptionFailFast(object unhandledException, ExInfo exceptionInfo)
        {
            if (unhandledException is Exception exceptionObject)
            {
                Console.Write("Unhandled exception. ");
           
                Console.WriteLine(exceptionObject.Message ?? "");

                Console.Write("IP="); 
                Console.Write(exceptionInfo.instructionPointer);
                Console.Write(",FP=");
                Console.WriteLine(exceptionInfo.framePointer);
            }

            Environment.Exit(-1);
        }
    }
}
