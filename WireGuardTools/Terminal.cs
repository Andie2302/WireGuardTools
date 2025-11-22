using Renci.SshNet;
using Renci.SshNet.Common;

namespace WireGuardTools;

/// <summary>
/// Facilitates the execution of commands on a remote server using SSH connections.
/// </summary>
/// <remarks>
/// This class simplifies the process of connecting to a remote host over SSH by providing methods for authentication via password or private key.
/// It allows execution of secure shell commands and handles connection lifecycle, including disposal of resources.
/// </remarks>
public sealed class Terminal : IDisposable
{
    /// <summary>
    /// Defines a message indicating that a connection attempt failed because a connection is already established.
    /// </summary>
    /// <remarks>
    /// This constant error message is used to handle cases where an operation attempts to initiate a new SSH connection
    /// while another connection is still active. It prevents multiple simultaneous connections using the same instance of <c>Terminal</c>.
    /// </remarks>
    private const string AlreadyConnectedError = "The connection is already established.";

    /// <summary>
    /// Represents the error message used when an SSH connection has not been established.
    /// </summary>
    /// <remarks>
    /// This constant is used internally within the <c>Terminal</c> class to notify users
    /// that a connection must be established before attempting any operations, such as executing commands.
    /// </remarks>
    private const string NotConnectedError = "SSH connection is not established. Please call Connect() first.";

    /// <summary>
    /// Represents the error message used when a disposed <c>Terminal</c> object is accessed.
    /// </summary>
    /// <remarks>
    /// This constant string is utilized within the <c>Terminal</c> class to throw an <see cref="ObjectDisposedException"/>
    /// when methods are invoked on a disposed <c>Terminal</c> instance.
    /// </remarks>
    private const string ObjectDisposedError = "The Terminal object has already been disposed.";

    /// <summary>
    /// Represents the underlying SSH client used for managing SSH connections and executing commands.
    /// </summary>
    /// <remarks>
    /// This field is a nullable instance of <see cref="Renci.SshNet.SshClient"/> and is used internally by the <c>Terminal</c> class
    /// to handle SSH operations such as connecting, executing commands, and disconnecting. It supports authentication methods
    /// defined in the various <c>IAuthenticationStrategy</c> implementations.
    /// </remarks>
    private SshClient? _sshClient;

    /// <summary>
    /// Indicates whether the <see cref="Terminal"/> instance has already been disposed.
    /// </summary>
    /// <remarks>
    /// The <c>_disposed</c> field ensures that resource cleanup is performed only once by the
    /// <see cref="Dispose()"/> method. If the instance is already disposed, further operations
    /// will throw an <see cref="ObjectDisposedException"/> to prevent the use of an invalid state.
    /// </remarks>
    private bool _disposed = false;


    /// <summary>
    /// Establishes a connection to a remote host using SSH.
    /// </summary>
    /// <remarks>
    /// This method establishes an SSH connection to the specified host using the provided credentials.
    /// The connection is authenticated with a username and password. The default port is 22 if not specified.
    /// </remarks>
    /// <param name="host">The hostname or IP address of the remote SSH server.</param>
    /// <param name="username">The username used for the SSH login.</param>
    /// <param name="password">The password used for the SSH login.</param>
    /// <param name="port">The port used for the SSH connection. Defaults to 22 if not specified.</param>
    public void Connect(string host, string username, string password, int port = 22)
    {
        var parameters = new ConnectionParameters(host, port, username);
        var authStrategy = new PasswordAuthenticationStrategy(password);
        ConnectInternal(parameters, authStrategy);
    }


    /// <summary>
    /// Establishes a connection to a remote host using SSH and a private key for authentication.
    /// </summary>
    /// <param name="host">The hostname or IP address of the remote host to connect to.</param>
    /// <param name="username">The username to use for authentication on the remote host.</param>
    /// <param name="privateKeyPath">The path to the private key file used for SSH authentication.</param>
    /// <param name="passphrase">The passphrase for the private key file, if applicable.</param>
    public void ConnectWithKey(string host, string username, string privateKeyPath, string passphrase)
    {
        var parameters = new ConnectionParameters(host, 22, username);
        var authStrategy = new KeyAuthenticationStrategy(privateKeyPath, passphrase);
        ConnectInternal(parameters, authStrategy);
    }


    /// <summary>
    /// Executes a given command and performs the associated action.
    /// </summary>
    /// <param name="command">The command string to be executed.</param>
    /// <param name="parameters">An optional list of parameters required to execute the command.</param>
    /// <returns>A boolean value indicating whether the command was executed successfully.</returns>
    public string ExecuteCommand(string command)
    {
        ValidateIsConnected();
        
        using var cmd = _sshClient!.CreateCommand(command);
        return ExecuteAndValidateCommand(cmd, command);
    }


    /// <summary>
    /// Disconnects the active SSH connection if it is currently connected.
    /// </summary>
    /// <remarks>
    /// This method ensures the proper termination of the SSH session by
    /// disconnecting the <c>SshClient</c> if it is in a connected state.
    /// It helps to free up underlying resources associated with the session
    /// and should be called when the connection is no longer needed.
    /// </remarks>
    public void Close()
    {
        if (_sshClient?.IsConnected == true)
        {
            _sshClient.Disconnect();
        }
    }


    /// <summary>
    /// Releases all resources used by the <see cref="Terminal"/> instance.
    /// </summary>
    /// <remarks>
    /// This method performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources. It ensures proper cleanup of managed and unmanaged
    /// resources when the <see cref="Terminal"/> instance is no longer required.
    /// </remarks>
    /// <example>
    /// Always invoke <see cref="Dispose"/> when finished using the <see cref="Terminal"/> instance,
    /// or use a `using` statement to ensure proper disposal.
    /// </example>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the <c>Terminal</c> instance.
    /// </summary>
    /// <param name="disposing">
    /// A boolean value indicating whether to release both managed and unmanaged resources.
    /// When set to <c>true</c>, both managed and unmanaged resources are released.
    /// When set to <c>false</c>, only unmanaged resources are released.
    /// </param>
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            Close();
            _sshClient?.Dispose();
            _sshClient = null;
        }
        
        _disposed = true;
    }

    /// <summary>
    /// Establishes an SSH connection to a remote host using specified connection parameters and authentication strategy.
    /// </summary>
    /// <param name="parameters">The <see cref="ConnectionParameters"/> object containing the host, port, and username details required for establishing the connection.</param>
    /// <param name="authStrategy">The <see cref="IAuthenticationStrategy"/> implementation used to authenticate the connection, such as password or key-based authentication.</param>
    /// <exception cref="SshAuthenticationException">Thrown when an authentication error occurs while establishing the SSH connection.</exception>
    /// <exception cref="Exception">Thrown when a general error occurs during the connection process.</exception>
    private void ConnectInternal(ConnectionParameters parameters, IAuthenticationStrategy authStrategy)
    {
        ValidateCanConnect();
        
        SshClient? client = null;
        try
        {
            client = authStrategy.CreateClient(parameters);
            client.Connect();
            _sshClient = client;
        }
        catch (SshAuthenticationException ex)
        {
            client?.Dispose();
            throw new Exception($"SSH-Authentifizierungsfehler{authStrategy.AuthenticationTypeSuffix}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            client?.Dispose();
            throw new Exception($"Fehler beim Verbindungsaufbau{authStrategy.AuthenticationTypeSuffix}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validates whether a connection can be initiated to a remote host.
    /// </summary>
    /// <remarks>
    /// This method ensures that any necessary preconditions for establishing a connection are met.
    /// It checks whether the <c>Terminal</c> instance has been disposed and whether
    /// there is an active connection already established. Throws exceptions if any
    /// violations of these preconditions are detected.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the <c>Terminal</c> instance has been disposed and is no longer usable.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an active connection already exists, preventing the initiation of a new connection.
    /// </exception>
    private void ValidateCanConnect()
    {
        ThrowIfDisposed();
        EnsureNotConnected();
    }

    /// <summary>
    /// Validates whether the SSH connection is established and throws an exception if not.
    /// </summary>
    /// <remarks>
    /// This method checks if the <c>Terminal</c> instance has been disposed and ensures that the SSH connection is active.
    /// If the connection is not active or the object has been disposed, appropriate exceptions are thrown to prevent execution of operations on an invalid state.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the <c>Terminal</c> instance has already been disposed.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the SSH connection is not established.
    /// </exception>
    private void ValidateIsConnected()
    {
        ThrowIfDisposed();
        EnsureConnected();
    }

    /// <summary>
    /// Ensures that the object has not been disposed and is still valid for use.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the method is called on a disposed <c>Terminal</c> object.
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Terminal), ObjectDisposedError);
        }
    }

    /// <summary>
    /// Ensures that the <see cref="Terminal"/> instance is not currently connected to a remote host.
    /// </summary>
    /// <remarks>
    /// This method verifies whether an existing connection is active. If the instance is connected, it throws an
    /// <see cref="InvalidOperationException"/> to prevent duplicate connections. It serves as a precaution to ensure
    /// that no active connection exists before attempting to establish a new one.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <see cref="Terminal"/> instance is already connected to a remote host.
    /// </exception>
    private void EnsureNotConnected()
    {
        if (_sshClient?.IsConnected == true)
        {
            throw new InvalidOperationException(AlreadyConnectedError);
        }
    }

    /// <summary>
    /// Ensures that the SSH client is currently connected to the remote server.
    /// </summary>
    /// <remarks>
    /// This method verifies the connection status of the SSH client. If the client is not connected,
    /// it throws an <see cref="InvalidOperationException"/> with an appropriate error message.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the SSH client is not connected to the remote server.
    /// </exception>
    private void EnsureConnected()
    {
        if (_sshClient?.IsConnected != true)
        {
            throw new InvalidOperationException(NotConnectedError);
        }
    }

    /// <summary>
    /// Executes an SSH command and validates its result based on the exit status.
    /// </summary>
    /// <param name="cmd">The <c>SshCommand</c> object that represents the command to execute.</param>
    /// <param name="command">The string representation of the command to execute, used for error messages.</param>
    /// <returns>The output of the executed command as a string.</returns>
    /// <exception cref="SshCommandException">
    /// Thrown when the command execution fails with a non-zero exit status. Provides details about the failure.
    /// </exception>
    private static string ExecuteAndValidateCommand(SshCommand cmd, string command)
    {
        var output = cmd.Execute();
        
        if (cmd.ExitStatus != 0)
        {
            throw new SshCommandException(
                $"Befehl '{command}' schlug fehl mit Exit Code {cmd.ExitStatus}. Fehler: {cmd.Error}",
                cmd.ExitStatus,
                cmd.Error
            );
        }
        
        return output;
    }
}