namespace System
{
    internal class Buffer
    {
        internal static unsafe void Memmove(byte* dest, byte* src, int len)
        {
            for (int i = 0; i < len; i++) 
            {
                *dest = *src;
                dest++;
                src++;
            }
        }
    }
}
