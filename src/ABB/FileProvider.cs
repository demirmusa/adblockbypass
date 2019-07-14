using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ABB.Caching;
using ABB.KeyProvider;
using ABB.KeyProvider.Dto;
using ABB.Settings;
using Microsoft.Extensions.Options;

namespace ABB
{
    public class FileProvider
    {
        private readonly ABBOptions _options;
        private readonly IUniqueKeyProvider _keyProvider;
        private readonly StringReplacer _stringReplacer;
        private readonly ICacheManager _cacheManager;

        private const string CachedFilePrefix = "AdBlockBypass.FileProvider.CachedFiles.";

        public FileProvider(
            IOptions<ABBOptions> options,
            IUniqueKeyProvider keyProvider,
            StringReplacer stringReplacer,
            ICacheManager cacheManager
            )
        {
            _options = options.Value;
            _keyProvider = keyProvider;
            _stringReplacer = stringReplacer;
            _cacheManager = cacheManager;
        }
        public string ProvideCssJsFileString(string fileKey)
        {
            var keyInformation = _keyProvider.GetValueFromMatchedGuid(fileKey);//get file name with given key (find which file does user want)

            if (keyInformation == null) return null;//check whether file exist


            string fileString = GetFileStringFromCache(keyInformation);//get from cache

            if (!string.IsNullOrEmpty(fileString))
                return fileString;

            ABBFile file;
            string filePath = "";
            if (Path.GetExtension(keyInformation.Value) == ".css" && _options.CSSFiles.ContainsKey(keyInformation.Value))
            {
                file = _options.CSSFiles[keyInformation.Value];
                filePath = GetCssFilePath(file);
            }
            else if (Path.GetExtension(keyInformation.Value) == ".js" && _options.JSFiles.ContainsKey(keyInformation.Value))
            {
                file = _options.JSFiles[keyInformation.Value];
                filePath = GetJsFilePath(file);
            }
            else
            {
                file = null;
            }

            if (file != null)
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File not found at the given path. {filePath}");

                //read file and replace all keys
                fileString = File.ReadAllText(filePath);
                fileString = _stringReplacer.SafeReplace(fileString, file.KeysToReplace);
                SetFileStringCache(keyInformation, fileString);
            }
            return fileString;
        }

        private string GetCssFilePath(ABBFile file)
        {
            return string.IsNullOrEmpty(file.FilePath)//generate file path (if file FilePath property use it. else use DefaultFilePath property)
                ? $"{_options.DefaultFilePath}/css/{file.FileName}"
                : $"{file.FilePath}/{file.FileName}";
        }

        private string GetJsFilePath(ABBFile file)
        {
            return string.IsNullOrEmpty(file.FilePath)//generate file path (if file FilePath property use it. else use DefaultFilePath property)
                 ? $"{_options.DefaultFilePath}/js/{file.FileName}"
                 : $"{file.FilePath}/{file.FileName}";
        }
        private string GetFileStringFromCache(KeyInformation keyInformation)
        {
            string result = "";
            if (keyInformation.ExpireTime > DateTime.Now)
            {
                result = _cacheManager.Get<string>(CachedFilePrefix + keyInformation.MatchedGuid);
            }
            return result;
        }

        private void SetFileStringCache(KeyInformation keyInformation, string fileString)
        {
            if (keyInformation.ExpireTime > DateTime.Now)//if key is valid
            {
                _cacheManager.Set(CachedFilePrefix + keyInformation.MatchedGuid, fileString,
                    keyInformation.ExpireTime.Subtract(DateTime.Now));
            }
        }
    }
}
