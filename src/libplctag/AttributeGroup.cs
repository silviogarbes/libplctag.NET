using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libplctag
{
    public class AttributeGroup
    {

        public string Name { get; set; }
        public Protocol? Protocol { get; set; }
        public string Gateway { get; set; }
        public PlcType? PlcType { get; set; }
        public string Path { get; set; }
        public int? ElementSize { get; set; }
        public int? ElementCount { get; set; }
        public bool? UseConnectedMessaging { get; set; }
        public int? ReadCacheMillisecondDuration { get; set; }

        public string GetAttributeString()
        {

            var attributes = new Dictionary<string, string>();

            attributes.Add("protocol", this.Protocol.ToString());
            attributes.Add("gateway", this.Gateway);
            attributes.Add("path", Path);
            attributes.Add("plc", PlcType.ToString().ToLower());
            attributes.Add("elem_size", ElementSize?.ToString());
            attributes.Add("elem_count", ElementCount?.ToString());
            attributes.Add("name", Name);
            attributes.Add("read_cache_ms", ReadCacheMillisecondDuration?.ToString());
            if (UseConnectedMessaging.HasValue)
                attributes.Add("use_connected_msg", UseConnectedMessaging.Value ? "1" : "0");

            string separator = "&";
            return string.Join(separator, attributes.Where(attr => attr.Value != null).Select(attr => $"{attr.Key}={attr.Value}"));

        }

    }
}
