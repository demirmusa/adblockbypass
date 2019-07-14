using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ABB.Settings;
using Microsoft.Extensions.Options;
using ABB.KeyProvider;
namespace ABB
{
    public class StringReplacer
    {
        private readonly IUniqueKeyProvider _keyProvider;

        public StringReplacer(IUniqueKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        /// <summary>
        /// this is simply replace given bypass keys with generated guid value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string SafeReplace(string input, string val)
        {
            var keyInformation = _keyProvider.GetUniqueKey(val);

            string textToFind = $@"\b{val}\b";
            input = Regex.Replace(input, textToFind, keyInformation.MatchedGuid);

            textToFind = $@"\b.{val}\b";//class prefix
            input = Regex.Replace(input, textToFind, keyInformation.MatchedGuid);

            textToFind = $@"\b#{val}\b";//id prefix
            input = Regex.Replace(input, textToFind, keyInformation.MatchedGuid);

            return input;
        }
        public string SafeReplace(string input, List<string> values)
        {
            if (values == null) return input;

            foreach (var key in values)
                input = SafeReplace(input, key);

            return input;
        }

    }
}
