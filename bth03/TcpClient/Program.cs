using System.Net;
using System.Net.Sockets;
using System.Text;

string serverIP = "127.0.0.1";
int serverPort = 5001;

Console.WriteLine($"[TcpClient] Connecting to {serverIP}:{serverPort}\n");

while (true)
{
    Console.Write("Enter integer a: ");
    string? inputA = Console.ReadLine();

    Console.Write("Enter integer b: ");
    string? inputB = Console.ReadLine();

    if (!int.TryParse(inputA, out int a) || !int.TryParse(inputB, out int b))
    {
        Console.WriteLine("[TcpClient] Please enter valid integers.\n");
        continue;
    }

    TcpClient client = new TcpClient(serverIP, serverPort);
    NetworkStream stream = client.GetStream();

    byte[] data = Encoding.UTF8.GetBytes($"{a} {b}");
    stream.Write(data, 0, data.Length);

    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);
    string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    Console.WriteLine($"[TcpClient] Result: {a} + {b} = {result}\n");

    client.Close();

    Console.Write("Continue? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y") break;
    Console.WriteLine();
}

Console.WriteLine("[TcpClient] Connection closed.");
