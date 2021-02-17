using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PythonDotNet
{
    public class PythonLauncher
    {
        private readonly string _python;
        public int TaskTimeout { get; set; } = int.MaxValue;

        public PythonLauncher(string pythonPath)
        {
            _python = pythonPath;
        }

        /// <summary>
        /// Launch a Python script synchronously
        /// </summary>
        public PythonProcessOutput Launch(string script, params string[] args)
        {
            return Execute(script, args);
        }

        /// <summary>
        /// Launch a Python script synchronously
        /// </summary>
        public PythonProcessOutput Launch(PythonProcessInput info)
        {
            return Execute(info.Script, info.Args);
        }

        /// <summary>
        /// Launch multiple Python scripts synchronously
        /// </summary>
        public IEnumerable<PythonProcessOutput> Launch(IEnumerable<PythonProcessInput> info)
        {
            return info.Select(x => Execute(x.Script, x.Args));
        }

        /// <summary>
        /// Launch a Python script asynchronously
        /// </summary>
        public async Task<PythonProcessOutput> LaunchAsync(string script, params string[] args)
        {
            return await ExecuteAsync(script, args);
        }

        /// <summary>
        /// Launch a Python script asynchronously
        /// </summary>
        public async Task<PythonProcessOutput> LaunchAsync(PythonProcessInput info)
        {
            return await ExecuteAsync(info.Script, info.Args);
        }

        /// <summary>
        /// Launch multiple Python scripts asynchronously
        /// </summary>
        public async Task<IEnumerable<PythonProcessOutput>> LaunchAsync(IEnumerable<PythonProcessInput> info)
        {
            var tasks = info.Select(x => ExecuteAsync(x.Script, x.Args));
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result);
        }

        private PythonProcessOutput Execute(string script, params string[] args)
        {
            var processArgs = $"{script} {string.Join(" ", args)}";
            return ProcessHelper.ExecuteShellCommand(_python, processArgs, TaskTimeout);
        }

        private async Task<PythonProcessOutput> ExecuteAsync(string script, params string[] args)
        {
            return await Task.Run(() => Execute(script, args));
        }
    }
}
