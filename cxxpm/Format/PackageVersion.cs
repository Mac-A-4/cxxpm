using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace cxxpm.Format {
    
    internal class PackageVersion {

        [JsonProperty("repository")]
        public PackageRepository Repository { get; set; } = new PackageRepository();

        [JsonProperty("hash")]
        public string Hash { get; set; } = string.Empty;

        public string Encode() {
            return $"{Repository.Encode()}@{Hash.ToLower()}";
        }

        public static PackageVersion Decode(string value) {
            var token = value.Split('@');
            if (token.Length != 2) {
                throw new ArgumentException("Invalid encoded string passed to PackageVersion.Decode");
            }
            return new PackageVersion() {
                Repository = PackageRepository.Decode(token[0]),
                Hash = token[1],
            };
        }

    }

}
