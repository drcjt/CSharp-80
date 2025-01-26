using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.IL.Stubs
{
    public static class EqualityComparerHelpersIntrinsics
    {
        public static MethodIL? EmitIL(MethodDesc method)
        {
            if (!method.HasGenericParameters)
            {
                switch (method.Name)
                {
                    case "GetComparerForReferenceTypesOnly":
                        return EmitGetComparerForReferenceTypesOnly(method);

                    case "StructOnlyEquals":
                        return EmitStructOnlyEquals(method);
                }
            }

            return null;
        }

        private static MethodIL? EmitGetComparerForReferenceTypesOnly(MethodDesc method)
        {
            var elementType = method.Instantiation[0];
            if (!elementType.IsRuntimeDeterminedSubtype && !elementType.IsCanonicalSubtype(CanonicalFormKind.Any)
                && elementType.IsValueType)
            {
                var body = new MethodIL();

                body.Instructions.Add(new Instruction(ILOpcode.ldnull, 0));
                body.Instructions.Add(new Instruction(ILOpcode.ret, 1));

                return body;
            }

            return null;
        }

        private static MethodIL? EmitStructOnlyEquals(MethodDesc method)
        {
            var elementType = method.Instantiation[0];
            if (!elementType.IsRuntimeDeterminedSubtype && !elementType.IsCanonicalSubtype(CanonicalFormKind.Any)
                && elementType.IsValueType)
            {
                var methodToCall = GetMethodCall(elementType);
                if (methodToCall is null)
                    return null;

                var instantiatedMethodToCall = new InstantiatedMethod(methodToCall, method.Instantiation);

                var body = new MethodIL();
                body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
                body.Instructions.Add(new Instruction(ILOpcode.ldarg_1, 1));
                body.Instructions.Add(new Instruction(ILOpcode.call, 2, instantiatedMethodToCall));
                body.Instructions.Add(new Instruction(ILOpcode.ret, 3));

                return body;
            }

            return null;
        }

        private static MethodDesc? GetMethodCall(TypeDesc elementType)
        {
            TypeSystemContext context = elementType.Context;

            bool? equatable = ComparerIntrinsics.ImplementsIEquatable(elementType);
            if (!equatable.HasValue)
                return null;

            var helperType = context.SystemModule!.GetKnownType("System.Collections.Generic", "EqualityComparerHelpers");
            var methodCallName = equatable.Value ? "StructOnlyEqualsIEquatable" : "StructOnlyNormalEquals";
            return helperType.GetKnownMethod(methodCallName);
        }
    }
}