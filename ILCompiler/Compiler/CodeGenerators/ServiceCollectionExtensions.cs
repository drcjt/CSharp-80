using ILCompiler.Compiler.EvaluationStack;
using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler.CodeGenerators
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCodeGenerators(this IServiceCollection services)
        {
            services.AddSingleton<ICodeGenerator<BinaryOperator>, BinaryOperatorCodeGenerator>();
            services.AddSingleton<ICodeGenerator<CallEntry>, CallCodeGenerator>();
            services.AddSingleton<ICodeGenerator<CastEntry>, CastCodeGenerator>();
            services.AddSingleton<ICodeGenerator<FieldAddressEntry>, FieldAddressCodeGenerator>();
            services.AddSingleton<ICodeGenerator<Int32ConstantEntry>, Int32ConstantCodeGenerator>();
            services.AddSingleton<ICodeGenerator<NativeIntConstantEntry>, NativeIntConstantCodeGenerator>();
            services.AddSingleton<ICodeGenerator<IntrinsicEntry>, IntrinsicCodeGenerator>();
            services.AddSingleton<ICodeGenerator<JumpEntry>, JumpCodeGenerator>();
            services.AddSingleton<ICodeGenerator<JumpTrueEntry>, JumpTrueCodeGenerator>();
            services.AddSingleton<ICodeGenerator<LocalVariableAddressEntry>, LocalVariableAddressCodeGenerator>();
            services.AddSingleton<ICodeGenerator<StringConstantEntry>, StringConstantCodeGenerator>();
            services.AddSingleton<ICodeGenerator<SwitchEntry>, SwitchCodeGenerator>();
            services.AddSingleton<ICodeGenerator<UnaryOperator>, UnaryOperatorCodeGenerator>();
            services.AddSingleton<ICodeGenerator<StoreIndEntry>, StoreIndCodeGenerator>();
            services.AddSingleton<ICodeGenerator<IndirectEntry>, IndirectCodeGenerator>();
            services.AddSingleton<ICodeGenerator<LocalVariableEntry>, LocalVariableCodeGenerator>();
            services.AddSingleton<ICodeGenerator<StoreLocalVariableEntry>, StoreLocalVariableCodeGenerator>();
            services.AddSingleton<ICodeGenerator<ReturnEntry>, ReturnCodeGenerator>();
            services.AddSingleton<ICodeGenerator<AllocObjEntry>, AllocObjCodeGenerator>();
            services.AddSingleton<ICodeGenerator<LocalHeapEntry>, LocalHeapCodeGenerator>();
            services.AddSingleton<ICodeGenerator<PutArgTypeEntry>, PutArgTypeCodeGenerator>();
            services.AddSingleton<ICodeGenerator<StaticFieldEntry>,  StaticFieldCodeGenerator>();
            services.AddSingleton<ICodeGenerator<BoundsCheck>, BoundsCheckCodeGenerator>();
            return services;
        }
    }
}
