using cxxpm.Format;
using cxxpm.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Git {

    internal class GitRepository {

        private readonly string directory;

        private readonly PackageRepository repository;

        public GitRepository(string directory, PackageRepository repository) {
            this.directory = directory;
            this.repository = repository;
        }

        public string? GetBranch() {
            return DirectoryUtil.In(directory, () => {
                var revParseCmd = CreateGetBranchCommand();
                if (revParseCmd.GetExitCode() != 0) {
                    throw new InvalidOperationException(string.Format("Failed to resolve branch of {0}", repository.Name));
                }
                var result = revParseCmd.GetStdOut().Trim();
                return result == "HEAD" ? null : result;
            });
        }

        public string GetHash() {
            return DirectoryUtil.In(directory, () => {
                var revParseCmd = CreateGetHashCommand();
                if (revParseCmd.GetExitCode() != 0) {
                    throw new InvalidOperationException(string.Format("Failed to resolve hash of {0}", repository.Name));
                }
                return revParseCmd.GetStdOut().Trim().ToLower();
            });
        }

        public void Checkout(string target) {
            DirectoryUtil.In(directory, () => {
                var checkoutCmd = CreateCheckoutCommand(target);
                if (checkoutCmd.GetExitCode() != 0) {
                    throw new InvalidOperationException(string.Format("Failed to checkout target {0} on {1}", target, repository.Name));
                }
            });
        }

        public void Clone() {
            var cloneCmd = CreateCloneCommand();
            if (cloneCmd.GetExitCode() != 0) {
                throw new InvalidOperationException(string.Format("Failed to clone repository {0}", repository.Name));
            }
        }

        private CommandUtil CreateCloneCommand() {
            return new CommandUtil("git", new string[] {
                "clone",
                "--recurse-submodules",
                CreateCloneUrl(),
                directory});
        }

        private CommandUtil CreateCheckoutCommand(string target) {
            return new CommandUtil("git", new string[] {
                "checkout",
                target});
        }

        private CommandUtil CreateGetBranchCommand() {
            return new CommandUtil("git", new string[] {
                "rev-parse",
                "--abbrev-ref",
                "HEAD"});
        }

        private CommandUtil CreateGetHashCommand() {
            return new CommandUtil("git", new string[] {
                "rev-parse",
                "--short",
                "HEAD"});
        }

        private string CreateCloneUrl() {
            return string.Format("git@{0}:{1}/{2}.git",
                repository.Host,
                repository.User,
                repository.Name);
        }

    }

}
