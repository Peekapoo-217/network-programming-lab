using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5001;
TcpListener listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine($"[TcpServer] Listening on port {port}...\n");

while (true)
{
    TcpClient client = listener.AcceptTcpClient();
    Console.WriteLine($"[TcpServer] Client connected: {client.Client.RemoteEndPoint}");

    NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);

    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    Console.WriteLine($"[TcpServer] Received: \"{received}\"");

    string[] parts = received.Split(' ');
    if (parts.Length == 2 && int.TryParse(parts[0], out int a) && int.TryParse(parts[1], out int b))
    {
        int sum = a + b;
        byte[] response = Encoding.UTF8.GetBytes(sum.ToString());
        stream.Write(response, 0, response.Length);
        Console.WriteLine($"[TcpServer] Sent result: {a} + {b} = {sum}\n");
    }
    else
    {
        byte[] error = Encoding.UTF8.GetBytes("ERROR: Invalid input");
        stream.Write(error, 0, error.Length);
        Console.WriteLine("[TcpServer] Invalid data received.\n");
    }

    client.Close();
}
