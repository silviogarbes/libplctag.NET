using libplctag.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace libplctag
{
    public class Plc
    {
        public string Gateway { get; set; }
        public string Path { get; set; }
        public PlcType? Type { get; set; }
        public bool? UseConnectedMessaging { get; set; }

        /// <summary>
        /// A factory for Tag objects that injects properties related to a Plc connection into the Tag.
        /// </summary>
        public Tag CreateTag(string name, int elementSize, int elementCount = 1)
        {
            return new Tag()
            {
                Name = name,
                ElementSize = elementSize,
                ElementCount = elementCount,
                Gateway = Gateway,
                PlcType = Type,
                UseConnectedMessaging = UseConnectedMessaging,
            };
        }

        /// <summary>
        /// A factory for GenericTag objects that injects properties related to a Plc connection into the Tag.
        /// </summary>
        public GenericTag<TPlcType, TDotNetType> CreateGenericTag<TPlcType, TDotNetType>(string name, int elementCount = 1)
            where TPlcType : IPlcType<TDotNetType>, new()
        {
            return new GenericTag<TPlcType, TDotNetType>()
            {
                Name = name,
                ElementCount = elementCount,
                Gateway = Gateway,
                PlcType = Type,
                UseConnectedMessaging = UseConnectedMessaging
            };
        }

    }
}
