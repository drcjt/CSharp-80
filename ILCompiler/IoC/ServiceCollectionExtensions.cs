using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFactory<T>(this IServiceCollection services) where T : class
        {
            services.AddSingleton<Func<T>>(x => () => x.GetRequiredService<T>());
            services.AddSingleton<Factory<T>>();
        }
    }
}
