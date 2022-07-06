namespace MillsSoftware.CoreSassCompiler
{
    // Results of a SASS compilation.  Stored in the cache for quick access.
    public class SassCompilation
    {
        public bool IsSuccess { get; set; }
        public string? SassResult { get; set; }
        public string? SassErrors { get; set; }
        public string? MinifierErrors { get; set; }
        public string? Url { get; set; }
        public string? Hash { get; set; }
    }
}
