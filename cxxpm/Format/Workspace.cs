using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Format {

    internal class Workspace : FormatUtil<Workspace> {

        [JsonProperty("packages")]
        public Dictionary<string, WorkspacePackage> Packages { get; set; } = new Dictionary<string, WorkspacePackage>();

    }

}
