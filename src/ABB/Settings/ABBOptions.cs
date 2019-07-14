using System;
using System.Collections.Generic;
using System.Text;

namespace ABB.Settings
{
    public class ABBOptions
    {
        public double CacheExpireTimeSec { get; set; }
        /// <summary>
        /// default file path for your js and css files.AdBlockBypass will search file with ({DefaultFilePath}/{FileType}/{FileName})
        /// <para>example: {DefaultFilePath}/css/myAdd.css</para>
        /// <para>example: {DefaultFilePath}/js/myAdd.js</para>
        /// </summary>
        public string DefaultFilePath { get; set; }


        internal Dictionary<string, ABBPage> Pages { get; private set; } = new Dictionary<string, ABBPage>();
        internal Dictionary<string, ABBFile> JSFiles { get; private set; } = new Dictionary<string, ABBFile>();
        internal Dictionary<string, ABBFile> CSSFiles { get; private set; } = new Dictionary<string, ABBFile>();


        public void AddPages(params ABBPage[] pages)
        {
            if (pages == null) return;

            foreach (var page in pages)
            {
                Pages.Add(page.PagePath, page);
            }
        }

        public void AddJSFiles(params ABBFile[] jsFiles)
        {
            if (jsFiles == null) return;

            foreach (var file in jsFiles)
                JSFiles.Add(file.FileName, file);
        }
        public void AddCSSFiles(params ABBFile[] cssFiles)
        {
            if (cssFiles == null) return;

            foreach (var file in cssFiles)
                CSSFiles.Add(file.FileName, file);
        }
    }
}
