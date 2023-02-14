using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Format {

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum PackageType {

        [EnumMember(Value = "application")]
        Application,

        [EnumMember(Value = "library")]
        Library,

    }

}
