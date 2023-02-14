using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cxxpm.Format {

    internal class PackageRepository {

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("host")]
        public string Host { get; set; } = string.Empty;

        [JsonProperty("user")]
        public string User { get; set; } = string.Empty;

        public string Encode() {
            return $"{Host.ToLower()}:{User}:{Name}";
        }

        public static PackageRepository Decode(string value) {
            var token = value.Split(':');
            if (token.Length != 3) {
                throw new ArgumentException("Invalid encoded string passed to PackageRepository.Decode");
            }
            return new PackageRepository() {
                Host = token[0].ToLower(),
                User = token[1],
                Name = token[2],
            };
        }

    }

}
