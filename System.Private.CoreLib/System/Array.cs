using System.Runtime.CompilerServices;

namespace System
{
    public class Array
    {
        public extern int Length
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }
    }
}
