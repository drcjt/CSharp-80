using dnlib.DotNet;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler
{
    public class MethodCompilerFactory : IMethodCompilerFactory
    {
        private IZ80Assembly _assembly;
        private IRomRoutines _romRoutines;
        private IConfiguration _configuration;
        private ILogger<MethodCompiler> _logger;
        public MethodCompilerFactory(IZ80Assembly assembly, IRomRoutines romRoutines, IConfiguration configuration, ILogger<MethodCompiler> logger)
        {
            _assembly = assembly;
            _romRoutines = romRoutines;
            _configuration = configuration;
            _logger = logger;
        }

        public IMethodCompiler Create(MethodDef method)
        {
            return new MethodCompiler(_assembly, _romRoutines, _configuration, _logger, method);
        }
    }
}
