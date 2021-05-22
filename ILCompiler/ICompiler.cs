using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler
{
    public interface ICompiler
    {
        public bool IgnoreUnknownCil { get; set; }
        public void Compile(string inputFilePath);
    }
}
