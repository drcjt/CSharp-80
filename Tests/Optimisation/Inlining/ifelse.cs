using System;
using System.Runtime.CompilerServices;

namespace Inlining
{
    public class A
    {
        private int _prop;
        public int Prop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_prop != 100) ? _prop : 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value == 1)
                {
                    Console.WriteLine("Setting Prop to 1");
                    _prop = value + 99;
                }
                else if (value == 2)
                {
                    Console.WriteLine("Setting Prop to 2");
                    _prop = value + 98;
                }
                else
                {
                    Console.WriteLine("Setting Prop to default value");
                    _prop = value;
                }
            }
        }
    }

    public static class IfElse
    {
        public static int Main()
        {
            A a = new A();
            a.Prop = 1;
            int retval = a.Prop;

            return retval;
        }
    }
}