﻿using System.Runtime.CompilerServices;

namespace System
{
    public static partial class Environment
    {
        [Intrinsic]
        private static void _Exit(int exitCode)
        {
            throw new Exception();
        }

        public static void Exit(int exitCode) => _Exit(exitCode);
    }
}
