using Microsoft.Extensions.Caching.Memory;

namespace MillsSoftware.CoreSassCompiler
{
    // Store a SASS compilation in the cache.  Allows for time expiration and watching for SASS source file changes.
    public class CustomCache
    {
        private readonly IMemoryCache _cache;
        private readonly SassWatchToken _sassWatchToken;
        private readonly SassCompiler _sassCompiler;
        private const string CacheKey = "MillsSoftware.CoreSassCompiler";

        public CustomCache(IMemoryCache cache, SassWatchToken sassWatchToken, SassCompiler sassCompiler)
        {
            _cache = cache;
            _sassWatchToken = sassWatchToken;
            _sassCompiler = sassCompiler;
        }

        public SassCompilation GetSassCompilation(string inputPath, string outputPath, bool minify, int cacheMinutes)
        {
            return _cache.GetOrCreate<SassCompilation>(CacheKey, entry =>
            {
                // Cache entry for defined number of minutes.
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheMinutes);

                // Watch for changes in the SASS files.
                if (_sassWatchToken.ChangeToken != null)
                {
                    entry.ExpirationTokens.Add(_sassWatchToken.ChangeToken);
                }

                return _sassCompiler.Compile(inputPath, outputPath, minify);
            });
        }
    }
}
