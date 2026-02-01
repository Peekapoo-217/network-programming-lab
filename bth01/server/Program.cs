// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("COnnect to serrver");

IPEndPoint iep = new IPEndPoint(IPAddress.Any, 2014);
Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
server.Bind(iep);
server.Listen(10);
Console.WriteLine("waiting");
Socket client = server.Accept();

while (true)
{
    byte[] data = new byte[1024];
    int recv = client.Receive(data);
    if (recv == 0) break;
    string s = Encoding.ASCII.GetString(data, 0, recv);
    Console.WriteLine("from client: {0}", s, s.Length);
    s = s.ToUpper();
    data = new byte[1024];
    data = Encoding.ASCII.GetBytes(s);
    client.Send(data, data.Length, SocketFlags.None);
}

client.Close();
server.Close();