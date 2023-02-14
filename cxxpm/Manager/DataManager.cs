using cxxpm.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace cxxpm.Manager {

    internal class DataManager {

        private const string PACKAGE_FILE = "cxxpm.json";

        private const string WORKSPACE_FILE = "workspace.json";

        private const string DATA_DIRECTORY = ".cxxpm";

        private const string PACKAGES_DIRECTORY = "packages";

        private const string CMAKE_FILE = "CMakeLists.txt";

        public DataManager() {
            Directory.CreateDirectory(DATA_DIRECTORY);
            Directory.CreateDirectory(Path.Join(DATA_DIRECTORY, PACKAGES_DIRECTORY));
        }

        public Package LoadPackage() {
            var path = GetPackageFilePath();
            if (File.Exists(path)) {
                return Package.Load(path);
            } else {
                return new Package();
            }
        }

        public Package LoadPackage(string directory) {
            return Package.Load(GetPackageFilePath(directory));
        }

        public void SavePackage(Package package) {
            Package.Save(GetPackageFilePath(), package);
        }

        public void SavePackage(string directory, Package package) {
            Package.Save(GetPackageFilePath(directory), package);
        }

        public void UsingPackage(Action<Package> action) {
            var package = LoadPackage();
            action.Invoke(package);
            SavePackage(package);
        }

        public T UsingPackage<T>(Func<Package, T> func) {
            var package = LoadPackage();
            var result = func.Invoke(package);
            SavePackage(package);
            return result;
        }

        public Workspace LoadWorkspace() {
            var path = GetWorkspaceFilePath();
            if (File.Exists(path)) {
                return Workspace.Load(GetWorkspaceFilePath());
            } else {
                return new Workspace();
            }
        }

        public void SaveWorkspace(Workspace workspace) {
            Workspace.Save(GetWorkspaceFilePath(), workspace);
        }

        public void UsingWorkspace(Action<Workspace> action) {
            var workspace = LoadWorkspace();
            action.Invoke(workspace);
            SaveWorkspace(workspace);
        }

        public T UsingWorkspace<T>(Func<Workspace, T> func) {
            var workspace = LoadWorkspace();
            var result = func.Invoke(workspace);
            SaveWorkspace(workspace);
            return result;
        }

        public string GetWorkspaceFilePath() {
            return Path.Join(DATA_DIRECTORY, WORKSPACE_FILE);
        }

        public string GetPackageFilePath() {
            return PACKAGE_FILE;
        }

        public string GetPackagePath(string directory) {
            return Path.Join(DATA_DIRECTORY, PACKAGES_DIRECTORY, directory);
        }

        public string GetPackageFilePath(string directory) {
            return Path.Join(GetPackagePath(directory), PACKAGE_FILE);
        }

        public string GetPackageCMakeFilePath() {
            return CMAKE_FILE;
        }

        public string GetPackageCMakeFilePath(string directory) {
            return Path.Join(GetPackagePath(directory), CMAKE_FILE);
        }

        public void DeletePackage(string directory) {
            try {
                Directory.Delete(GetPackagePath(directory), true);
            } catch (Exception) {
                //
            }
        }

    }

}
