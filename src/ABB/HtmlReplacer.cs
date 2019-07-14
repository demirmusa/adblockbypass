using System;
using System.Collections.Generic;
using System.Text;
using ABB.KeyProvider;
using ABB.Settings;
using Microsoft.Extensions.Options;

namespace ABB
{
    public class HtmlReplacer
    {
        private readonly ABBOptions _options;
        private readonly StringReplacer _stringReplacer;

        public HtmlReplacer(IOptions<ABBOptions> options, StringReplacer stringReplacer)
        {
            _options = options.Value;
            _stringReplacer = stringReplacer;
        }

        public string ReplaceBypassKeysInHtml(string pagePath, string pageString)
        {
            if (!_options.Pages.ContainsKey(pagePath)) return pageString;

            var page = _options.Pages[pagePath];

            if (page.JsFiles != null && page.JsFiles.Count > 0)//page has js files
            {
                foreach (var pageJsFileName in page.JsFiles)
                {
                    if (_options.JSFiles.ContainsKey(pageJsFileName))//options contains js file
                    {
                        var jsFile = _options.JSFiles[pageJsFileName];
                        pageString = _stringReplacer.SafeReplace(pageString, jsFile.FileName);//replace file names with unique key so that url blocking cant catch your js file
                        pageString = _stringReplacer.SafeReplace(pageString, jsFile.KeysToReplace);
                    }
                }
            }

            if (page.CssFiles != null && page.JsFiles.Count > 0)
            {
                foreach (var pageCssFileName in page.CssFiles)
                {
                    if (_options.CSSFiles.ContainsKey(pageCssFileName))
                    {
                        var cssFile = _options.CSSFiles[pageCssFileName];
                        pageString = _stringReplacer.SafeReplace(pageString, cssFile.FileName);//replace file names with unique key so that url blocking cant catch your
                        pageString = _stringReplacer.SafeReplace(pageString, cssFile.KeysToReplace);
                    }
                }
            }

            if (page.AnotherKeysToReplace != null && page.AnotherKeysToReplace.Count > 0)
            {
                pageString = _stringReplacer.SafeReplace(pageString, page.AnotherKeysToReplace);
            }
            return pageString;
        }
    }
}
