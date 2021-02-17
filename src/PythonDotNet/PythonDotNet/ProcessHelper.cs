using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PythonDotNet
{
    /// <summary>
    /// Adapted from https://gist.github.com/AlexMAS/f2dc6c0527646fd0284f34dafdfcdc21
    /// </summary>
    public static class ProcessHelper
    {
        internal static PythonProcessOutput ExecuteShellCommand(string command, string arguments, int timeout = int.MaxValue)
        {
            var result = new PythonProcessOutput();

            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                using (var outputCloseEvent = new AutoResetEvent(false))
                using (var errorCloseEvent = new AutoResetEvent(false))
                {
                    var copyOutputCloseEvent = outputCloseEvent;

                    process.OutputDataReceived += (s, e) =>
                    {
                        // Output stream is closed (process completed)
                        if (string.IsNullOrEmpty(e.Data))
                        {
                            copyOutputCloseEvent.Set();
                        }
                        else
                        {
                            outputBuilder.AppendLine(e.Data);
                        }
                    };

                    var copyErrorCloseEvent = errorCloseEvent;

                    process.ErrorDataReceived += (s, e) =>
                    {
                        // Error stream is closed (process completed)
                        if (string.IsNullOrEmpty(e.Data))
                        {
                            copyErrorCloseEvent.Set();
                        }
                        else
                        {
                            errorBuilder.AppendLine(e.Data);
                        }
                    };

                    bool isStarted;

                    try
                    {
                        isStarted = process.Start();
                    }
                    catch (Exception error)
                    {
                        result.ExitCode = -1;
                        result.Errors = error.Message;

                        isStarted = false;
                    }

                    if (isStarted)
                    {
                        // Read the output stream first and then wait because deadlocks are possible
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(timeout)
                            && outputCloseEvent.WaitOne(timeout)
                            && errorCloseEvent.WaitOne(timeout))
                        {
                            result.ExitCode = process.ExitCode;
                            result.Output = $"{outputBuilder}{errorBuilder}";
                        }
                        else
                        {
                            try
                            {
                                // Kill hung process
                                process.Kill();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
