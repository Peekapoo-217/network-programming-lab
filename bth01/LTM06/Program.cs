using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

int port = 5000;
TcpListener server = new TcpListener(IPAddress.Any, port);
server.Start();
using TcpClient client = server.AcceptTcpClient();

using NetworkStream stream = client.GetStream();
using StreamReader reader = new StreamReader(stream);
using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

try
{ 
    string strA = reader.ReadLine();
    string strB = reader.ReadLine();

    if (int.TryParse(strA, out int a) && int.TryParse(strB, out int b))
    {
        int sum = a + b;
        Console.WriteLine($"Nhan duoc: a = {a}, b = {b}. Tong: {sum}");

        writer.WriteLine(sum);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Message}");
}

server.Stop();