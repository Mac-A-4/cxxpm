using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Format {

    internal class PackageDependency {

        [JsonProperty("version")]
        public PackageVersion Version { get; set; } = new PackageVersion();

        [JsonProperty("branch")]
        public string? Branch { get; set; }

    }

}
