﻿using ILCompiler.Compiler.EvaluationStack;
using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler.Lowerings
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowerings(this IServiceCollection services)
        {
            services.AddSingleton<ILowering<BinaryOperator>, BinaryOperatorLowering>();
            services.AddSingleton<ILowering<JumpTrueEntry>, JumpTrueLowering>();
            services.AddSingleton<ILowering<StoreLocalVariableEntry>, StoreLocalVariableLowering>();
            services.AddSingleton<ILowering<IndirectEntry>, IndirectLowering>();
            services.AddSingleton<ILowering<ArrayLengthEntry>, ArrayLengthLowering>();
            return services;
        }
    }
}
