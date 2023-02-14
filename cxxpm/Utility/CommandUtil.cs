using System.Diagnostics;

namespace cxxpm.Utility {

    internal class CommandUtil {

        private Process process;

        private bool exited;

        private string? stdOut;

        public CommandUtil(string exe, string[] args) {
            var _process = Process.Start(CreateStartInfo(exe, args));
            if (_process == null) {
                throw new ArgumentException(string.Format("Executable {0} not found", exe));
            }
            process = _process;
        }

        public int GetExitCode() {
            WaitForExit();
            return process.ExitCode;
        }

        public string GetStdOut() {
            WaitForExit();
            if (stdOut == null) {
                stdOut = process.StandardOutput.ReadToEnd();
            }
            return stdOut;
        }

        private void WaitForExit() {
            if (!exited) {
                process.WaitForExit();
                exited = true;
            }
        }

        private ProcessStartInfo CreateStartInfo(string exe, string[] args) {
            var startInfo = new ProcessStartInfo {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = exe
            };
            foreach (var e in args) {
                startInfo.ArgumentList.Add(e);
            }
            return startInfo;
        }

    }

}
