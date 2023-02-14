using cxxpm.CMake;
using cxxpm.Format;
using cxxpm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Manager {

    internal class CMakeManager {

        private readonly ConsoleUtil log;

        private readonly DataManager dataManager;

        public CMakeManager(DataManager dataManager) {
            log = new ConsoleUtil();
            this.dataManager = dataManager;
        }

        public void Generate() {
            log.i($"Generating CMake files");
            dataManager.UsingWorkspace(workspace => {
                foreach (var package in workspace.Packages.Values) {
                    GeneratePackage(package);
                }
                GenerateRoot(workspace);
            });
            log.s($"Successfully generated all CMake files");
        }

        private void GenerateRoot(Workspace workspace) {
            dataManager.UsingPackage(package => {
                GenerateRootImpl(workspace, package);
            });
        }

        private void GeneratePackage(WorkspacePackage workspacePackage) {
            var package = dataManager.LoadPackage(workspacePackage.Directory);
            GeneratePackageImpl(workspacePackage, package);
        }

        private void GenerateRootImpl(Workspace workspace, Package package) {
            var builder = new CMakeBuilder() {
                Type = package.Type,
                Name = package.Name,
                Source = package.Source,
                Include = package.Include,
                Project = package.Name,
                Dependencies = (from e in package.Dependencies.Values select GetVersionName(e.Version)).ToList(),
                Subdirectories = (from e in workspace.Packages.Values select dataManager.GetPackagePath(e.Directory)).ToList(),
            };
            var path = dataManager.GetPackageCMakeFilePath();
            log.i($"Generating \"{path}\"");
            File.WriteAllText(path, builder.Build());
        }

        private void GeneratePackageImpl(WorkspacePackage workspacePackage, Package package) {
            var builder = new CMakeBuilder() {
                Type = package.Type,
                Name = GetVersionName(workspacePackage.Version),
                Source = package.Source,
                Include = package.Include,
                Dependencies = (from e in package.Dependencies.Values select GetVersionName(e.Version)).ToList(),
            };
            var path = dataManager.GetPackageCMakeFilePath(workspacePackage.Directory);
            log.i($"Generating \"{path}\"");
            File.WriteAllText(path, builder.Build());
        }

        private string GetVersionName(PackageVersion version) {
            return version.Encode().Replace(':', '-').Replace('@', '-');
        }

    }

}
