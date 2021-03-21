using PythonDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Windows
            PythonLauncher launcher = new PythonLauncher(@"C:\Program Files\Python3.9.1\python.exe");

            // Linux
            //PythonLauncher launcher = new PythonLauncher("python");
            
            launcher.TaskTimeout = 30 * 1000; // 30 second timeout

            // Test 1 - Simple synchronous task            
            var result = launcher.Launch("helloworld.py");
            Show(result);

            // Test 2 - Single asynchronous tasks
            result = launcher.LaunchAsync("echo_and_sleep.py", "\"Sleep for 1 second\"", "1").Result;
            Show(result);

            // Test 3 - Multiple asynchronous tasks
            // Tasks are launched in parallel, so it will take 5 seconds (longest sleep time) even if sum of all is 15 (5+4+3+2+1)

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var result2 = AsyncTest(launcher);
            Task.WaitAll(result2);
            sw.Stop();
            Console.WriteLine($"Task finished in {sw.ElapsedMilliseconds} ms");
            Show(result2.Result);

            Console.Out.WriteLine("Press ENTER to exit");
            _ = Console.In.ReadLine();
        }

        static void Show(IEnumerable<PythonProcessOutput> results)
        {
            foreach (var result in results)
                Show(result);
        }

        static void Show(PythonProcessOutput result)
        {
            if (result.ExitCode == 0)
                Console.Out.WriteLine($"Execution Succesful. Output: {result.Output}");
            else
                Console.Out.WriteLine($"Execution Error: {result.Errors}");
        }

        static async Task<IEnumerable<PythonProcessOutput>> AsyncTest(PythonLauncher launcher)
        {
            var inputData = Enumerable.Range(1, 5).Select(x => new PythonProcessInput { Script = "echo_and_sleep.py", Args = new string[] { $"\"Sleep for {x} second(s)\"", x.ToString() } });
            return await launcher.LaunchAsync(inputData);
        }
    }
}
