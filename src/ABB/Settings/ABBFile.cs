using System;
using System.Collections.Generic;
using System.Text;

namespace ABB.Settings
{
    public class ABBFile
    {
        public string FileName { get; set; }
        /// <summary>
        /// AdBlockBypass will search file with {FilePath}/{FileName}
        /// <para>(overrides DefaultFilePath)</para>
        /// </summary>
        public string FilePath { get; set; }
        public List<string> KeysToReplace { get; set; }
    }
}
