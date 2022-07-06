using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MillsSoftware.CoreSassCompiler
{
    // Tag helper to inject CSS (compiled SASS) into a Razor view.  Adds a browser cache busting hash based on the compilation.
    public class SassTagHelper : TagHelper
    {
        private readonly SassCompiler _sassCompiler;

        public SassTagHelper(SassCompiler sassCompiler)
        {
            _sassCompiler = sassCompiler;
        }

        public string? Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "link";
            output.Attributes.Add("rel", "stylesheet");

            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                var sassCompilation = _sassCompiler.GetCompilation(this.Name);
                if (sassCompilation != null && sassCompilation.IsSuccess)
                {
                    var href = $"{sassCompilation.Url}?{sassCompilation.Hash}";
                    output.Attributes.Add("href", href);
                }
            }

            base.Process(context, output);
        }
    }
}
