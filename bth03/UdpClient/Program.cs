using System.Net;
using System.Net.Sockets;
using System.Text;

string serverIP = "127.0.0.1";
int serverPort = 5000;

UdpClient client = new UdpClient();
IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

Console.WriteLine($"[Client] Connected to {serverIP}:{serverPort}\n");

while (true)
{
    Console.Write("Enter integer a: ");
    string? inputA = Console.ReadLine();

    Console.Write("Enter integer b: ");
    string? inputB = Console.ReadLine();

    if (!int.TryParse(inputA, out int a) || !int.TryParse(inputB, out int b))
    {
        Console.WriteLine("[Client] Please enter valid integers.\n");
        continue;
    }

    byte[] data = Encoding.UTF8.GetBytes($"{a} {b}");
    client.Send(data, data.Length, serverEndPoint);

    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
    string result = Encoding.UTF8.GetString(client.Receive(ref remoteEP));
    Console.WriteLine($"[Client] Result: {a} + {b} = {result}\n");

    Console.Write("Continue? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y") break;
    Console.WriteLine();
}

client.Close();
Console.WriteLine("[Client] Connection closed.");
