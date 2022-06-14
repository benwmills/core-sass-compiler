using Microsoft.Extensions.Primitives;

namespace MillsSoftware.CoreSassCompiler
{
    // Used to store an optional (but recommended) change token that watches for runtime changes in the SASS source files.
    public class SassWatchToken
    {
        public IChangeToken? ChangeToken { get; set; }
    }
}
