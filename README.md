# Core Sass Compiler

## The Basic Idea

.NET Core module to add SASS compilation and minification to any ASP.NET Core project.

## How to Install

1. Install nuget package via GitHub Packages.  Note that this package is NOT on the main nuget feed yet.

2. Add to services via dependency injection (here added to Program.cs in a .NET 6 project):

```
using MillsSoftware.CoreSassCompiler;

builder.Services.AddCoreSassCompiler(
    new SassProfile()
    {
        Name = "main-css",
        Url = "/css/main",
        InputFile = Path.Combine(environment.WebRootPath, "scss", "index.scss"),
        Minify = true,
        CacheMinutes = 1440,
        ChangeToken = environment.WebRootFileProvider.Watch("scss/*.scss")
    }
);

```

This sets up a profile that defines what SASS should be compiled (using LibSassHost) and whether it should be minified (using NUglify).  The compiled SASS will be cached for the number of minutes specified.  The cached compilation has a dependency on the `scss/*.scss files`, so editing any of these files results in a new compilation.  Note that multiple profiles can be added if necessary.

3. Link to the compiled SASS via a tag helper (here added in the _Layout.cshtml):

```
    <sass name="main-css" />
```

Note that this won't work until you add the following directive to `_ViewImports.cshtml` that allows your views to use the new tag helper:

```
@addTagHelper *, MillsSoftware.CoreSassCompiler
```

The end result in your rendered layout is a standard link tag with a hash based on the file content:

```
    <link rel="stylesheet" href="/css/main?UeBVvswRn5/Iod8IXrQ61A==" />
```

4. Add a controller to render the CSS:

```
using Microsoft.AspNetCore.Mvc;
using MillsSoftware.CoreSassCompiler;

namespace MySite.Controllers
{
    [Route("css")]
    public class CssController : Controller
    {
        private readonly SassCompiler _compiler;
        private readonly IWebHostEnvironment _environment;

        public CssController(SassCompiler compiler, IWebHostEnvironment environment)
        {
            _compiler = compiler;
            _environment = environment;
        }

        [HttpGet("main")]
        public IActionResult Main()
        {
            var compilation = _compiler.GetCompilation("main-css");
            if (compilation == null) return NotFound();

            if (!compilation.IsSuccess  && _environment.IsDevelopment())
            {
                return Content($"{compilation.SassErrors} {compilation.MinifierErrors}");
            }

            return Content(compilation.SassResult ?? "", "text/css");
        }
    }
}

```

## Future Changes

1. Add to the main nuget package feed.

2. Set up an automatic controller to return the CSS.  This would eliminate step 4 above, but would potential not give you as much control over the output (e.g. what to do when the compilation fails).

3. Add HTTP cache headers to the CSS output.