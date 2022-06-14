namespace MillsSoftware.CoreSassCompiler
{
    // Results of a SASS compilation.  Generally stored in the cache for quick access.
    public class SassCompilation
    {
        public bool IsSuccess { get; set; }
        public string? SassResult { get; set; }
        public string? Hash { get; set; }
    }
}
