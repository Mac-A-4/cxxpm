using CommandLine.Text;
using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm {

    [Verb("init", HelpText = "Initialize new package.")]
    internal class InitOptions { }

    [Verb("add", HelpText = "Add dependency to package.")]
    internal class AddOptions {

        [Value(0, MetaName = "repository", Required = true, HelpText = "Repository e.g. \"host.com:username:repository\".")]
        public string Repository { get; set; } = string.Empty;

        [Option('b', "branch", Required = false, HelpText = "Name of branch to track.")]
        public string? Branch { get; set; }

        [Option('h', "hash", Required = false, HelpText = "Name of commit to clone.")]
        public string? Hash { get; set; }

    }

    [Verb("remove", HelpText = "Remove dependency from package.")]
    internal class RemoveOptions {

        [Value(0, MetaName = "repository", Required = true, HelpText = "Repository e.g. \"host.com:username:repository\".")]
        public string Repository { get; set; } = string.Empty;

    }

    [Verb("list", HelpText = "List dependencies in package.")]
    internal class ListOptions { }

    [Verb("update", HelpText = "Update dependency of package.")]
    internal class UpdateOptions {

        [Value(0, MetaName = "repository", HelpText = "Repository e.g. \"host.com:username:repository\".")]
        public string? Repository { get; set; }

        [Option('b', "branch", Required = false, HelpText = "Name of branch to track.")]
        public string? Branch { get; set; }

        [Option('h', "hash", Required = false, HelpText = "Name of commit to clone.")]
        public string? Hash { get; set; }

    }

    [Verb("prepare", HelpText = "Prepare workspace and build scripts.")]
    internal class PrepareOptions { }

    [Verb("clean", HelpText = "Clean workspace.")]
    internal class CleanOptions { }

}
