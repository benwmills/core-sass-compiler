using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace MillsSoftware.CoreSassCompiler
{
    public static class ServiceCollectionExtensions
    {
        // Sets up dependency injection for the library.
        public static void CoreSassCompiler(this IServiceCollection services, IChangeToken? changeToken = null)
        {
            services.AddSingleton<SassCompilation>();
            services.AddSingleton<SassCache>();
            services.AddSingleton<SassWatchToken>(new SassWatchToken() { ChangeToken = changeToken});
        }
    }
}
