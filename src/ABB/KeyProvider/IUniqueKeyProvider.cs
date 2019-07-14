using System;
using System.Collections.Generic;
using System.Text;
using ABB.KeyProvider.Dto;

namespace ABB.KeyProvider
{
    public interface IUniqueKeyProvider
    {
        KeyInformation GetUniqueKey(string value);
        KeyInformation GetValueFromMatchedGuid(string key);
    }
}
