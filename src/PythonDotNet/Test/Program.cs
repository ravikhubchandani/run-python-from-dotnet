using PythonDotNet;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            PythonLauncher launcher = new PythonLauncher(@"C:\Program Files\Python3.9.1\python.exe");
            var result = launcher.Launch("helloworld.py");

            if(result.ExitCode == 0)
                Console.Out.WriteLine($"Execution Succesful. Output: {result.Output}");
            else
                Console.Out.WriteLine($"Execution Error: {result.Errors}");

            _ = Console.In.ReadLine();
        }
    }
}
