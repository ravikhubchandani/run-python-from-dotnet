namespace PythonDotNet
{
    public class PythonProcessOutput
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }
    }
}
