using cxxpm.CMake;
using cxxpm.Format;
using cxxpm.Git;
using cxxpm.Utility;
using System;
using System.Security;
using System.Text;

namespace cxxpm.Manager {

    internal class WorkspaceManager {

        private readonly ConsoleUtil log;

        private readonly DataManager dataManager;

        private readonly DependencyManager dependencyManager;

        private readonly CMakeManager cmakeManager;

        public WorkspaceManager(DataManager dataManager, DependencyManager dependencyManager, CMakeManager cmakeManager) {
            log = new ConsoleUtil();
            this.dataManager = dataManager;
            this.dependencyManager = dependencyManager;
            this.cmakeManager = cmakeManager;
        }

        public List<PackageDependency> ListDependencies() {
            return dataManager.UsingPackage(package => {
                return package.Dependencies.Values.ToList();
            });
        }

        public void AddDependency(PackageRepository repository) {
            dataManager.UsingPackage(package => {
                var dependencyId = repository.Encode();
                if (package.Dependencies.ContainsKey(dependencyId)) {
                    log.e($"Dependency {dependencyId} is already added");
                    return;
                }
                var version = dependencyManager.AddDependency(repository);
                AddVersionToPackage(package, version, null);
            });
        }

        public void AddDependencyFromHash(PackageRepository repository, string hash) {
            dataManager.UsingPackage(package => {
                var dependencyId = repository.Encode();
                if (package.Dependencies.ContainsKey(dependencyId)) {
                    log.e($"Dependency {dependencyId} is already added");
                    return;
                }
                var version = dependencyManager.AddDependency(new PackageVersion() {
                    Repository = repository,
                    Hash = hash,
                });
                AddVersionToPackage(package, version, null);
            });
        }

        public void AddDependencyFromBranch(PackageRepository repository, string branch) {
            dataManager.UsingPackage(package => {
                var dependencyId = repository.Encode();
                if (package.Dependencies.ContainsKey(dependencyId)) {
                    log.e($"Dependency {dependencyId} is already added");
                    return;
                }
                var version = dependencyManager.AddDependency(repository, branch);
                AddVersionToPackage(package, version, branch);
            });
        }

        public void RemoveDependency(PackageRepository repository) {
            var dependencyId = repository.Encode();
            dataManager.UsingPackage(package => {
                if (!package.Dependencies.ContainsKey(dependencyId)) {
                    log.e($"Dependency {dependencyId} does not exist");
                    return;
                }
                log.i($"Removing {dependencyId}");
                package.Dependencies.Remove(dependencyId);
            });
            dependencyManager.CleanDependencies();
            log.s($"Successfully removed {dependencyId}");
        }

        public void UpdateDependency(PackageRepository repository) {
            dataManager.UsingPackage(package => {
                UsingDependency(package, repository, dependency => {
                    UpdateDependencyImpl(package, dependency);
                });
            });
            dependencyManager.CleanDependencies();
        }

        public void UpdateDependencyFromBranch(PackageRepository repository, string branch) {
            dataManager.UsingPackage(package => {
                UsingDependency(package, repository, dependency => {
                    UpdateDependencyFromBranchImpl(package, dependency.Version, branch);
                    dependency.Branch = branch;
                });
            });
            dependencyManager.CleanDependencies();
        }

        public void UpdateDependencyFromHash(PackageRepository repository, string hash) {
            dataManager.UsingPackage(package => {
                UsingDependency(package, repository, dependency => {
                    UpdateDependencyFromHashImpl(package, dependency.Version, hash);
                    dependency.Branch = null;
                });
            });
            dependencyManager.CleanDependencies();
        }

        public void Update() {
            dataManager.UsingPackage(package => {
                foreach (var dependency in package.Dependencies.Values) {
                    if (dependency.Branch != null) {
                        UpdateDependencyImpl(package, dependency);
                    }
                }
            });
            dependencyManager.CleanDependencies();
        }

        public void Prepare() {
            dependencyManager.CleanDependencies();
            dataManager.UsingPackage(package => {
                log.i($"Preparing {package.Name}");
                PrepareImpl(package);
                log.s($"Successfully prepared {package.Name}");
            });
            cmakeManager.Generate();
        }

        private void UpdateDependencyImpl(Package package, PackageDependency dependency) {
            var dependencyId = dependency.Version.Repository.Encode();
            if (dependency.Branch == null) {
                log.e($"Dependency {dependencyId} cannot be upgraded (no tracking branch)");
                return;
            }
            UpdateDependencyFromBranchImpl(package, dependency.Version, dependency.Branch);
        }

        private void UpdateDependencyFromBranchImpl(Package package, PackageVersion existingVersion, string branch) {
            var dependencyId = existingVersion.Repository.Encode();
            log.i($"Updating dependency {dependencyId}");
            var version = dependencyManager.AddDependency(existingVersion.Repository, branch);
            UpdateVersionInPackage(package, existingVersion, version);
        }

        private void UpdateDependencyFromHashImpl(Package package, PackageVersion existingVersion, string hash) {
            var dependencyId = existingVersion.Repository.Encode();
            log.i($"Updating dependency {dependencyId}");
            var version = dependencyManager.AddDependency(new PackageVersion() {
                Repository = existingVersion.Repository,
                Hash = hash,
            });
            UpdateVersionInPackage(package, existingVersion, version);
        }

        private void PrepareImpl(Package package) {
            foreach (var dependency in package.Dependencies.Values) {
                if (!dependencyManager.ContainsDependency(dependency.Version)) {
                    dependencyManager.AddDependency(dependency.Version);
                }
            }
        }

        private void UpdateVersionInPackage(Package package, PackageVersion existingVersion, PackageVersion newVersion) {
            var dependencyId = existingVersion.Repository.Encode();
            if (existingVersion.Hash == newVersion.Hash) {
                log.s($"Dependency {existingVersion.Encode()} is up to date");
                return;
            }
            log.s($"Successfully updated {dependencyId} from {existingVersion.Hash} to {newVersion.Hash}");
            package.Dependencies[dependencyId].Version = newVersion;
        }

        private void AddVersionToPackage(Package package, PackageVersion version, string? branch) {
            var dependency = new PackageDependency() {
                Version = version,
                Branch = branch,
            };
            package.Dependencies.Add(version.Repository.Encode(), dependency);
        }

        private void UsingDependency(Package package, PackageRepository repository, Action<PackageDependency> action) {
            var dependencyId = repository.Encode();
            if (!package.Dependencies.ContainsKey(dependencyId)) {
                log.e($"Dependency {dependencyId} does not exist");
                return;
            }
            action.Invoke(package.Dependencies[dependencyId]);
        }

    }

}
