using System;
using System.Collections.Generic;
using System.Text;

namespace ABB.Settings
{
    public class ABBPage
    {
        internal string PagePath { get; private set; }
        internal List<string> JsFiles { get; private set; }
        internal List<string> CssFiles { get; private set; }
        internal List<string> AnotherKeysToReplace { get; private set; }
        public ABBPage(string pagePath)
        {
            this.PagePath = pagePath;
        }
        public ABBPage AddJsFiles(params string[] fileNames)
        {
            if (fileNames == null) return this;

            if (JsFiles == null)
                JsFiles = new List<string>();

            foreach (var fileName in fileNames)
                JsFiles.Add(fileName);
            return this;
        }
        public ABBPage AddCssFiles(params string[] fileNames)
        {
            if (fileNames == null) return this;

            if (CssFiles == null)
                CssFiles = new List<string>();

            foreach (var fileName in fileNames)
                CssFiles.Add(fileName);

            return this;
        }
        public ABBPage AddAnotherKeysToBypass(params string[] keys)
        {
            if (keys == null) return this;

            if (AnotherKeysToReplace == null)
                AnotherKeysToReplace = new List<string>();

            foreach (var key in keys)
                AnotherKeysToReplace.Add(key);

            return this;
        }
    }
}
