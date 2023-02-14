using CommandLine;
using cxxpm.Format;
using cxxpm.Manager;
using cxxpm.Utility;
using System.Diagnostics;

namespace cxxpm {

    internal class Program {

        static void Init(DataManager manager, ConsoleUtil log) {
            manager.UsingPackage(package => {
                package.Name = log.Query("Name: ");
                package.Include = log.Query("Include path: ");
                package.Source = log.Query("Source path: ");
                package.Type = log.QueryEnum<PackageType>("Type: ");
                log.s($"Successfully initialized package {package.Name}");
            });
        }

        static void Add(AddOptions options, WorkspaceManager manager) {
            if (options.Branch != null && options.Hash != null) {
                throw new ArgumentException("Cannot add dependency with both branch and hash");
            } else if (options.Branch != null) {
                manager.AddDependencyFromBranch(PackageRepository.Decode(options.Repository), options.Branch);
            } else if (options.Hash != null) {
                manager.AddDependencyFromHash(PackageRepository.Decode(options.Repository), options.Hash);
            } else {
                manager.AddDependency(PackageRepository.Decode(options.Repository));
            }
        }

        static void Remove(RemoveOptions options, WorkspaceManager manager) {
            manager.RemoveDependency(PackageRepository.Decode(options.Repository));
        }

        static void List(WorkspaceManager manager, ConsoleUtil log) {
            foreach (var dependency in manager.ListDependencies()) {
                if (dependency.Branch != null) {
                    log.i($"{dependency.Version.Encode()} branch \"{dependency.Branch}\"");
                } else {
                    log.i($"{dependency.Version.Encode()}");
                }
            }
        }

        static void Update(UpdateOptions options, WorkspaceManager manager) {
            if (options.Repository == null) {
                manager.Update();
            } else if (options.Branch != null && options.Hash != null) {
                throw new ArgumentException("Cannot update dependency with both branch and hash");
            } else if (options.Branch != null) {
                manager.UpdateDependencyFromBranch(PackageRepository.Decode(options.Repository), options.Branch);
            } else if (options.Hash != null) {
                manager.UpdateDependencyFromHash(PackageRepository.Decode(options.Repository), options.Hash);
            } else {
                manager.UpdateDependency(PackageRepository.Decode(options.Repository));
            }
        }

        static void Clean(DependencyManager manager) {
            manager.CleanDependencies();
        }

        static void Prepare(WorkspaceManager manager) {
            manager.Prepare();
        }

        static void Main(string[] args) {
            var log
                = new ConsoleUtil();
            var dataManager
                = new DataManager();
            var dependencyManager
                = new DependencyManager(dataManager);
            var cmakeManager
                = new CMakeManager(dataManager);
            var workspaceManager
                = new WorkspaceManager(dataManager, dependencyManager, cmakeManager);

            try {

                Parser.Default.ParseArguments<InitOptions, AddOptions, RemoveOptions, ListOptions, UpdateOptions, PrepareOptions, CleanOptions>(args)
                .WithParsed<InitOptions>(options => {
                    Init(dataManager, log);
                })
                .WithParsed<AddOptions>(options => {
                    Add(options, workspaceManager);
                })
                .WithParsed<RemoveOptions>(options => {
                    Remove(options, workspaceManager);
                })
                .WithParsed<ListOptions>(options => {
                    List(workspaceManager, log);
                })
                .WithParsed<UpdateOptions>(options => {
                    Update(options, workspaceManager);
                })
                .WithParsed<PrepareOptions>(options => {
                    Prepare(workspaceManager);
                })
                .WithParsed<CleanOptions>(options => {
                    Clean(dependencyManager);
                });

            } catch (Exception e) {
                log.e($"An error occurred: {e.Message}");
                Environment.Exit(-1);
            }
        }

    }

}
