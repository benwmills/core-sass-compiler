using LibSassHost;
using Microsoft.Extensions.Logging;
using NUglify;
using NUglify.Css;
using System.Security.Cryptography;
using System.Text;

namespace MillsSoftware.CoreSassCompiler
{
    // The main SASS compiler.  Optionally minifies results.  Outputs any errors to the log.
    public class SassCompiler
    {
        private readonly ILogger<SassCompiler> _logger;

        public SassCompiler(ILogger<SassCompiler> logger)
        {
            _logger = logger;
        }

        public SassCompilation Compile(string inputPath, string outputPath, bool minify)
        {
            var result = new SassCompilation();

            try
            {
                // Compile using LibSass.
                var options = new CompilationOptions();
                CompilationResult libSass = LibSassHost.SassCompiler.CompileFile(inputPath, outputPath, options: options);

                // Minify the result.
                if (minify)
                {
                    var cssSettings = new CssSettings();
                    var minifiedResult = Uglify.Css(libSass.CompiledContent, cssSettings);

                    if (minifiedResult.HasErrors)
                    {
                        _logger.LogError(string.Join(",", minifiedResult.Errors.Select(x => x.Message)));
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

                result.IsSuccess = true;
            }
            catch (SassCompilationException e)
            {
                string error = LibSassHost.Helpers.SassErrorHelpers.GenerateErrorDetails(e);
                _logger.LogError(error);
                throw new ApplicationException(error, e);
            }

            return result;
        }
    }
}
