using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Format {

    internal class WorkspacePackage {

        [JsonProperty("version")]
        public PackageVersion Version { get; set; } = new PackageVersion();

        [JsonProperty("directory")]
        public string Directory { get; set; } = string.Empty;

    }

}
