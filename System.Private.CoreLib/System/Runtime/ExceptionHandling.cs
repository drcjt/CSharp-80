namespace System.Runtime
{
    public struct ExInfo
    {
        public ushort framePointer;
        public ushort instructionPointer;
    }

    internal static class ExceptionHandling
    {
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

            // Treat everything that gets here as unhandled exceptions
            UnhandledExceptionFailFast(exception, exceptionInfo);
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
