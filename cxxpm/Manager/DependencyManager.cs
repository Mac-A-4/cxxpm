using cxxpm.Format;
using cxxpm.Git;
using cxxpm.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Manager {

    internal class DependencyManager {

        private readonly ConsoleUtil log;

        private readonly DataManager dataManager;

        public DependencyManager(DataManager dataManager) {
            log = new ConsoleUtil();
            this.dataManager = dataManager;
        }

        public PackageVersion AddDependency(PackageRepository repository) {
            log.i($"Adding dependency {repository.Encode()}");
            return dataManager.UsingWorkspace(workspace => {
                return AddDependencyImpl(workspace, repository, null);
            });
        }

        public PackageVersion AddDependency(PackageRepository repository, string branch) {
            log.i($"Adding dependency {repository.Encode()} branch {branch}");
            return dataManager.UsingWorkspace(workspace => {
                return AddDependencyImpl(workspace, repository, branch);
            });
        }

        public PackageVersion AddDependency(PackageVersion version) {
            log.i($"Adding dependency {version.Encode()}");
            return dataManager.UsingWorkspace(workspace => {
                if (workspace.Packages.TryGetValue(version.Encode(), out var package)) {
                    log.s($"Successfully added {version.Encode()} from workspace");
                    return package.Version;
                } else {
                    return AddDependencyImpl(workspace, version.Repository, version.Hash);
                }
            });
        }

        public bool ContainsDependency(PackageVersion version) {
            return dataManager.UsingWorkspace(workspace => {
                return workspace.Packages.ContainsKey(version.Encode());
            });
        }

        public void CleanDependencies() {
            log.i($"Cleaning dependencies");
            dataManager.UsingWorkspace(workspace => {
                CleanDependenciesImpl(workspace);
            });
        }

        private void CleanDependenciesImpl(Workspace workspace) {
            var reachable = GetReachableDependencies(workspace);
            var unused = (from package in workspace.Packages.Values
                          where !reachable.Contains(package.Version.Encode())
                          select package).ToList();
            log.i($"{reachable.Count} reachable, {unused.Count} unused");
            foreach (var package in unused) {
                var packageId = package.Version.Encode();
                log.i($"Removing dependency {packageId}");
                workspace.Packages.Remove(packageId);
                dataManager.DeletePackage(package.Directory);
            }
            log.s($"Successfully cleaned dependencies");
        }

        private HashSet<string> GetReachableDependencies(Workspace workspace) {
            var reachable = new HashSet<string>();
            var traversal = new Stack<string>(GetDependencies());
            while (traversal.Count > 0) {
                var top = traversal.Pop();
                reachable.Add(top);
                if (workspace.Packages.TryGetValue(top, out var package)) {
                    foreach (var dependency in GetDependencies(package.Directory)) {
                        if (!reachable.Contains(dependency)) {
                            traversal.Push(dependency);
                        }
                    }
                }
            }
            return reachable;
        }

        private List<string> GetDependencies() {
            return GetDependencies(dataManager.LoadPackage());
        }

        private List<string> GetDependencies(string directory) {
            return GetDependencies(dataManager.LoadPackage(directory));
        }

        private List<string> GetDependencies(Package package) {
            return (from dependency in package.Dependencies.Values select dependency.Version.Encode()).ToList();
        }

        private PackageVersion AddDependencyImpl(Workspace workspace, PackageRepository repository, string? target) {
            var directory = Guid.NewGuid().ToString();
            var packagePath = dataManager.GetPackagePath(directory);
            log.i($"Destination directory \"{packagePath}\"");
            var gitRepo = new GitRepository(packagePath, repository);
            log.i($"Cloning repository from {repository.Host}");
            gitRepo.Clone();
            if (target != null) {
                log.i($"Checkout target \"{target}\"");
                gitRepo.Checkout(target);
            }
            var version = new PackageVersion() {
                Repository = repository,
                Hash = gitRepo.GetHash(),
            };
            var versionId = version.Encode();
            log.i($"Resolved package {versionId}");
            if (workspace.Packages.TryGetValue(versionId, out var existing)) {
                log.s($"Successfully added {versionId} from workspace");
                dataManager.DeletePackage(directory);
                return existing.Version;
            }
            var package = new WorkspacePackage() {
                Version = version,
                Directory = directory,
            };
            workspace.Packages.Add(versionId, package);
            log.i($"Preparing package {versionId}");
            try {
                PrepareDependencyImpl(workspace, directory);
            } catch (Exception) {
                dataManager.DeletePackage(directory);
                throw;
            }
            log.s($"Successfully added {versionId}");
            return version;
        }

        private void PrepareDependencyImpl(Workspace workspace, string directory) {
            var package = dataManager.LoadPackage(directory);
            if (package.Type == PackageType.Application) {
                throw new InvalidOperationException("Application packages cannot be added as dependencies");
            }
            foreach (var dependency in package.Dependencies) {
                var packageId = dependency.Key;
                var version = dependency.Value.Version;
                if (!workspace.Packages.ContainsKey(packageId)) {
                    AddDependencyImpl(workspace, version.Repository, version.Hash);
                }
            }
        }

    }

}
