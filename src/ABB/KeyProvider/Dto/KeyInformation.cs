using System;
using System.Collections.Generic;
using System.Text;

namespace ABB.KeyProvider.Dto
{
    public class KeyInformation
    {
        public string Value { get; set; }
        public string MatchedGuid { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
