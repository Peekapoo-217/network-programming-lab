using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 6000;
var accounts = new Dictionary<string, string>(); // username -> password

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
serverSocket.Listen(10);

Console.WriteLine($"[AccountServer] Listening on port {port}...\n");

while (true)
{
    Socket clientSocket = serverSocket.Accept();
    Console.WriteLine($"[AccountServer] Client connected: {clientSocket.RemoteEndPoint}");

    byte[] buffer = new byte[1024];
    int bytesRead = clientSocket.Receive(buffer);
    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

    Console.WriteLine($"[AccountServer] Request: {request}");

    string[] parts = request.Split('|');
    string command = parts[0].ToUpper();
    string response;

    switch (command)
    {
        case "REGISTER":
            if (parts.Length < 3)
            {
                response = "ERROR: Invalid format. Use REGISTER|username|password";
            }
            else if (accounts.ContainsKey(parts[1]))
            {
                response = "ERROR: Username already exists";
            }
            else
            {
                accounts[parts[1]] = parts[2];
                response = "OK: Account registered successfully";
                Console.WriteLine($"[AccountServer] Registered: {parts[1]}");
            }
            break;

        case "LOGIN":
            if (parts.Length < 3)
            {
                response = "ERROR: Invalid format. Use LOGIN|username|password";
            }
            else if (!accounts.ContainsKey(parts[1]))
            {
                response = "ERROR: Username not found";
            }
            else if (accounts[parts[1]] != parts[2])
            {
                response = "ERROR: Invalid password";
            }
            else
            {
                response = "OK: Login successful";
                Console.WriteLine($"[AccountServer] Logged in: {parts[1]}");
            }
            break;

        case "CHANGE_PASSWORD":
            if (parts.Length < 4)
            {
                response = "ERROR: Invalid format. Use CHANGE_PASSWORD|username|oldPassword|newPassword";
            }
            else if (!accounts.ContainsKey(parts[1]))
            {
                response = "ERROR: Username not found";
            }
            else if (accounts[parts[1]] != parts[2])
            {
                response = "ERROR: Invalid current password";
            }
            else
            {
                accounts[parts[1]] = parts[3];
                response = "OK: Password changed successfully";
                Console.WriteLine($"[AccountServer] Password changed: {parts[1]}");
            }
            break;

        case "DELETE":
            if (parts.Length < 3)
            {
                response = "ERROR: Invalid format. Use DELETE|username|password";
            }
            else if (!accounts.ContainsKey(parts[1]))
            {
                response = "ERROR: Username not found";
            }
            else if (accounts[parts[1]] != parts[2])
            {
                response = "ERROR: Invalid password";
            }
            else
            {
                accounts.Remove(parts[1]);
                response = "OK: Account deleted successfully";
                Console.WriteLine($"[AccountServer] Deleted account: {parts[1]}");
            }
            break;

        default:
            response = "ERROR: Unknown command";
            break;
    }

    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
    clientSocket.Send(responseBytes);
    Console.WriteLine($"[AccountServer] Response: {response}\n");

    clientSocket.Shutdown(SocketShutdown.Both);
    clientSocket.Close();
}
