using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class DependencyAnalyser
    {
        private readonly MethodDesc _method;

        private readonly IList<IDependencyNode> _dependencies = new List<IDependencyNode>();

        private IDictionary<string, Z80MethodCodeNode> _codeNodesByFullMethodName;

        public DependencyAnalyser(MethodDesc method, IDictionary<string, Z80MethodCodeNode> codeNodesByFullMethodName)
        {
            _method = method;
            _codeNodesByFullMethodName = codeNodesByFullMethodName;
        }

        public IList<IDependencyNode> FindDependencies()
        {
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
                            ImportCall(currentInstruction);
                            break;


                        case Code.Ldsfld:
                        case Code.Ldsflda:
                            ImportLoadField(currentInstruction, true);
                            break;

                        case Code.Stsfld:
                            ImportStoreField(currentInstruction, true);
                            break;
                            
                    }
                    currentOffset += currentInstruction.GetSize();
                    currentIndex++;
                }
            }

            return _dependencies;
        }

        private void ImportStoreField(Instruction instuction, bool isStatic)
        {
            ImportFieldAccess(instuction, isStatic);
        }

        private void ImportLoadField(Instruction instruction, bool isStatic)
        {
            ImportFieldAccess(instruction, isStatic);
        }

        private void ImportFieldAccess(Instruction instruction, bool isStatic)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            if (isStatic || fieldDef.IsStatic)
            {
                if (isStatic && !fieldDef.IsStatic)
                {
                    throw new InvalidProgramException();
                }

                if (fieldDef.HasFieldRVA)
                {
                    return;
                }

                var declaringType = fieldDef.DeclaringType;

                if (declaringType != null)
                {
                    _dependencies.Add(new EEType(declaringType));
                }
            }
        }

        private void ImportCall(Instruction instruction)
        {
            var method = instruction.Operand as IMethod;
            if (method != null)
            {
                if (method.IsMethodSpec)
                {
                    var methodSpec = (MethodSpec)method;
                    IList<TypeSig> genericArguments = methodSpec.GenericInstMethodSig.GenericArguments;
                    var methodDef = methodSpec.Method.ResolveMethodDefThrow();

                    if (!_codeNodesByFullMethodName.TryGetValue(methodSpec.FullName, out var methodNode))
                    {
                        methodNode = new Z80MethodCodeNode(new InstantiatedMethod(methodDef, genericArguments, methodSpec.FullName));
                        _codeNodesByFullMethodName[methodSpec.FullName] = methodNode;
                    }

                    _dependencies.Add(methodNode);
                }
                else
                {
                    var methodDef = method.ResolveMethodDef();
                    if (methodDef != null)
                    {
                        if (!_codeNodesByFullMethodName.TryGetValue(methodDef.FullName, out var methodNode))
                        {
                            methodNode = new Z80MethodCodeNode(new MethodDesc(methodDef));
                            _codeNodesByFullMethodName[methodDef.FullName] = methodNode;
                        }

                        _dependencies.Add(methodNode);
                    }
                }
            }
        }
    }
}
