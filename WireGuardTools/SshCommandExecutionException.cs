using System;

namespace WireGuardTools;

/// <summary>
/// Exception that is thrown when an SSH command execution fails.
/// </summary>
public class SshCommandException : Exception
{
    /// <summary>
    /// Gets the exit status code returned by the SSH command.
    /// </summary>
    public int ExitStatus { get; }

    /// <summary>
    /// Gets the error output from the SSH command.
    /// </summary>
    public string CommandError { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshCommandException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="exitStatus">The exit status code returned by the command.</param>
    /// <param name="commandError">The error output from the command.</param>
    public SshCommandException(string message, int exitStatus, string commandError)
        : base(message)
    {
        ExitStatus = exitStatus;
        CommandError = commandError ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshCommandException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="exitStatus">The exit status code returned by the command.</param>
    /// <param name="commandError">The error output from the command.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SshCommandException(string message, int exitStatus, string commandError, Exception innerException)
        : base(message, innerException)
    {
        ExitStatus = exitStatus;
        CommandError = commandError ?? string.Empty;
    }
}
