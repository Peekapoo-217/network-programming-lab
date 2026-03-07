using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5000;
UdpClient server = new UdpClient(port);
IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

Console.WriteLine($"[Server] Listening on port {port}...\n");

while (true)
{
    byte[] data = server.Receive(ref clientEndPoint);
    string received = Encoding.UTF8.GetString(data);
    Console.WriteLine($"[Server] Received from {clientEndPoint}: \"{received}\"");

    string[] parts = received.Split(' ');
    if (parts.Length == 2 && int.TryParse(parts[0], out int a) && int.TryParse(parts[1], out int b))
    {
        int sum = a + b;
        byte[] response = Encoding.UTF8.GetBytes(sum.ToString());
        server.Send(response, response.Length, clientEndPoint);
        Console.WriteLine($"[Server] Sent result: {a} + {b} = {sum}\n");
    }
    else
    {
        byte[] error = Encoding.UTF8.GetBytes("ERROR: Invalid input");
        server.Send(error, error.Length, clientEndPoint);
        Console.WriteLine("[Server] Invalid data received.\n");
    }
}
