﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.z80
{
    public class Condition
    {
        private readonly string _name;
        public Condition(string name)
        {
            _name = name;
        }

        public static readonly Condition Zero = new("Z");
        public static readonly Condition NonZero = new("NZ");
        public static readonly Condition NC = new("NC");

        public override string ToString()
        {
            return _name;
        }
    }
}
