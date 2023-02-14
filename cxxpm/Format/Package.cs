using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace cxxpm.Format {

    internal class Package : FormatUtil<Package> {

        [JsonProperty("type")]
        public PackageType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("src")]
        public string Source { get; set; } = string.Empty;

        [JsonProperty("include")]
        public string Include { get; set; } = string.Empty;

        [JsonProperty("dependencies")]
        public Dictionary<string, PackageDependency> Dependencies { get; set; } = new Dictionary<string, PackageDependency>();

    }

}
