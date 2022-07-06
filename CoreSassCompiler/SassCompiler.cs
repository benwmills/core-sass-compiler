using LibSassHost;
using Microsoft.Extensions.Logging;
using NUglify;
using NUglify.Css;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace MillsSoftware.CoreSassCompiler
{
    // The main SASS compiler.  Optionally minifies results.  Outputs any errors to the log.
    public class SassCompiler
    {
        private readonly ILogger<SassCompiler> _logger;
        private readonly IMemoryCache _cache;
        private readonly List<SassProfile> _profiles;

        public SassCompiler(ILogger<SassCompiler> logger, IMemoryCache cache, List<SassProfile> profiles)
        {
            _logger = logger;
            _cache = cache;
            _profiles = profiles;
        }

        public SassCompilation? GetCompilation(string profileName)
        {
            var profile = this._profiles.FirstOrDefault(x => x.Name == profileName);
            if (profile == null || string.IsNullOrWhiteSpace(profile.InputFile)) return null;

            string cacheKey = $"MillsSoftware.CoreSassCompiler.{profile.Name}";

            return _cache.GetOrCreate<SassCompilation>(cacheKey, entry =>
            {
                // Cache entry for defined number of minutes.
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(profile.CacheMinutes);

                // Watch for changes in the SASS files.
                if (profile.ChangeToken != null)
                {
                    entry.ExpirationTokens.Add(profile.ChangeToken);
                }

                var sassCompilation = this.Compile(profile.InputFile, profile.Minify);
                sassCompilation.Url = profile.Url;

                return sassCompilation;
            });
        }

        private SassCompilation Compile(string inputPath, bool minify)
        {
            var result = new SassCompilation();

            try
            {
                // Compile using LibSass.
                var options = new CompilationOptions();
                CompilationResult libSass = LibSassHost.SassCompiler.CompileFile(inputPath, options: options);

                // Minify the result.
                if (minify)
                {
                    var cssSettings = new CssSettings();
                    var minifiedResult = Uglify.Css(libSass.CompiledContent, cssSettings);

                    if (minifiedResult.HasErrors)
                    {
                        result.MinifierErrors = string.Join(",", minifiedResult.Errors.Select(x => x.Message));
                        _logger.LogError(result.MinifierErrors);
                    }

                    result.SassResult = minifiedResult.Code;
                }
                else
                {
                    result.SassResult = libSass.CompiledContent;
                }

                // Calculate the hash of the file.
                var sassBytes = Encoding.UTF8.GetBytes(result.SassResult);
                using var hasher = MD5.Create();
                var hashBytes = hasher.ComputeHash(sassBytes);
                var hashString = Convert.ToBase64String(hashBytes);

                result.Hash = hashString;

                // Indicate success.
                result.IsSuccess = true;
            }
            catch (SassCompilationException e)
            {
                result.IsSuccess = false;
                result.SassErrors = LibSassHost.Helpers.SassErrorHelpers.GenerateErrorDetails(e);
                _logger.LogError(result.SassErrors);
            }

            return result;
        }
    }
}
