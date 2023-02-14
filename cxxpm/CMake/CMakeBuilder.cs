using cxxpm.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.CMake {

    internal class CMakeBuilder {

        public static readonly string CMAKE_VERSION = "3.10";

        public PackageType Type { get; set; }

        public string? Project { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Include { get; set; } = string.Empty;

        public List<string> Dependencies { get; set; } = new List<string>();

        public List<string> Subdirectories { get; set; } = new List<string>();

        public string Build() {
            var builder = new StringBuilder();
            BuildHeader(builder);
            BuildTarget(builder);
            BuildDependencies(builder);
            BuildSubdirectories(builder);
            return builder.ToString();
        }

        private void BuildHeader(StringBuilder builder) {
            builder.AppendLine($"cmake_minimum_required(VERSION {CMAKE_VERSION})");
            if (Project != null) {
                builder.AppendLine($"project(\"{Project}\")");
            }
        }

        private void BuildTarget(StringBuilder builder) {
            builder.AppendLine($"file(GLOB_RECURSE TARGET_SRC {CreateRelativePath($"{Source}/*.cpp")})");
            if (Type == PackageType.Application) {
                builder.AppendLine($"add_executable(\"{Name}\" ${{TARGET_SRC}})");
            } else {
                builder.AppendLine($"add_library(\"{Name}\" STATIC ${{TARGET_SRC}})");
            }
            builder.AppendLine($"target_include_directories(\"{Name}\" PUBLIC {CreateRelativePath(Include)})");
        }

        private void BuildDependencies(StringBuilder builder) {
            foreach (var e in Dependencies) {
                builder.AppendLine($"target_link_libraries(\"{Name}\" PRIVATE \"{e}\")");
            }
        }

        private void BuildSubdirectories(StringBuilder builder) {
            foreach (var e in Subdirectories) {
                builder.AppendLine($"add_subdirectory({CreateRelativePath(e)})");
            }
        }

        private string CreateRelativePath(string path) {
            return $"\"${{CMAKE_CURRENT_SOURCE_DIR}}/{path.Trim().Replace("\\", "/")}\"";
        }

    }

}
