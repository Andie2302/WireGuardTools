namespace WireGuardTools;

public class SshCommandException(string message, int exitCode, string errorOutput) : Exception(message)
{
    public int ExitCode { get; } = exitCode;
    public string ErrorOutput { get; } = errorOutput;
}