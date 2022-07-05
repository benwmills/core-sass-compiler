using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MillsSoftware.CoreSassCompiler
{
    // Tag helper to inject CSS (compiled SASS) into a Razor view.  Adds a browser cache busting hash based on the compilation.
    public class SassTagHelper : TagHelper
    {
        private readonly SassCache _Cache;

        public SassTagHelper(SassCache cache)
        {
            _Cache = cache;
        }

        public string? InputPath { get; set; }
        public string? OutputPath { get; set; }
        public bool? Minify { get; set; }
        public int? CacheMinutes { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "link";
            output.Attributes.Add("rel", "stylesheet");

            bool hrefExists = output.Attributes.TryGetAttribute("href", out var hrefAttribute);

            if (hrefExists && !string.IsNullOrWhiteSpace(this.InputPath) && !string.IsNullOrWhiteSpace(this.OutputPath))
            {
                var sassCompilation = _Cache.GetSassCompilation(this.InputPath, this.OutputPath, this.Minify ?? true, this.CacheMinutes ?? 1440);
                if (sassCompilation.IsSuccess)
                {
                    var newHref = $"{hrefAttribute.Value}?{sassCompilation.Hash}";
                    output.Attributes.SetAttribute("href", newHref);
                }
            }

            base.Process(context, output);
        }
    }
}
