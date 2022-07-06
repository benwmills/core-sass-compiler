using Microsoft.Extensions.Primitives;

namespace MillsSoftware.CoreSassCompiler
{
    // Sass profile defines the SASS file that should be compiled and where it will be available.
    public class SassProfile
    {
        public SassProfile()
        {
            this.Minify = true;
            this.CacheMinutes = 1440;   // 24 hour default.
        }

        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? InputFile { get; set; }
        public bool Minify { get; set; }
        public int CacheMinutes { get; set; }
        public IChangeToken? ChangeToken { get; set; }      // Used to store an optional (but recommended) change token that watches for runtime changes in the SASS source files.
    }
}
