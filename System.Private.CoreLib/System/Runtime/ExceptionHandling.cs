namespace System.Runtime
{
    internal static class ExceptionHandling
    {
        public static void ThrowException(object exception)
        {
            exception ??= new NullReferenceException();

            DispatchException(exception);
        }

        private static void DispatchException(object exception)
        {
            // TODO: Exception dispatch goes here
            // search for appropriate handler
            // then call the handler if found

            // Treat everything that gets here as unhandled exceptions
            UnhandledExceptionFailFast(exception);
        }

        private static void UnhandledExceptionFailFast(object unhandledException)
        {
            if (unhandledException is Exception exceptionObject)
            {
                Console.Write("Unhandled exception. ");
                
                // TODO: this isn't working need to work out why
                //Console.WriteLine(exceptionObject.Message ?? "");

                if (exceptionObject.Message == null)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(exceptionObject.Message);
                }
            }

            Environment.Exit(-1);
        }
    }
}
