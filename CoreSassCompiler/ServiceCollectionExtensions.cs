using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace MillsSoftware.CoreSassCompiler
{
    public static class ServiceCollectionExtensions
    {
        // Sets up dependency injection for the library.
        public static void AddCoreSassCompiler(this IServiceCollection services, List<SassProfile> profiles)
        {
            services.AddSingleton<SassCompiler>();
            services.AddSingleton(profiles);
        }

        public static void AddCoreSassCompiler(this IServiceCollection services, SassProfile profile)
        {
            services.AddSingleton<SassCompiler>();
            services.AddSingleton(new List<SassProfile>() { profile });
        }
    }
}
