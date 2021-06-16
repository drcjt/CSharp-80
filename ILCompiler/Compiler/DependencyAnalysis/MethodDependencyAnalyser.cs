using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class MethodDependencyAnalyser
    {
        private readonly MethodDef _method;

        public MethodDependencyAnalyser(MethodDef method)
        {
            _method = method;
        }

        public IList<MethodDef> FindCallTargets()
        {
            var dependsOnMethods = new List<MethodDef>();
            var currentIndex = 0;
            var currentOffset = 0;

            if (_method.Body != null)
            {
                while (currentIndex < _method.Body.Instructions.Count)
                {
                    var currentInstruction = _method.Body.Instructions[currentIndex];

                    switch (currentInstruction.OpCode.Code)
                    {
                        case Code.Call:
                            var methodDefOrRef = currentInstruction.Operand as IMethodDefOrRef;
                            var methodDef = methodDefOrRef.ResolveMethodDef();
                            if (methodDef != null)
                            {
                                dependsOnMethods.Add(methodDef);
                            }
                            break;
                    }
                    currentOffset += currentInstruction.GetSize();
                    currentIndex++;
                }
            }

            return dependsOnMethods;
        }
    }
}
