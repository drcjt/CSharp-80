using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class MethodDependencyAnalyser
    {
        private readonly MethodDesc _method;

        public MethodDependencyAnalyser(MethodDesc method)
        {
            _method = method;
        }

        public IList<IMethod> FindCallTargets()
        {
            var dependsOnMethods = new List<IMethod>();
            var currentIndex = 0;
            var currentOffset = 0;

            if (_method.Body != null)
            {
                while (currentIndex < _method.Body.Instructions.Count)
                {
                    var currentInstruction = _method.Body.Instructions[currentIndex];

                    switch (currentInstruction.OpCode.Code)
                    {
                        case Code.Newobj:
                        case Code.Call:
                        case Code.Callvirt:
                            var method = currentInstruction.Operand as IMethod;
                            if (method != null)
                            {
                                if (method.IsMethodSpec)
                                {
                                    dependsOnMethods.Add(method);
                                }
                                else
                                {
                                    var methodDef = method.ResolveMethodDef();
                                    if (methodDef != null)
                                    {
                                        dependsOnMethods.Add(methodDef);
                                    }
                                }
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
