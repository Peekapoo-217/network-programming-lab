// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;

TcpListener sv = new(IPAddress.Any, 8080);
sv.Start();

Console.WriteLine("day la server");

while (true)
{
    try
    {
        using TcpClient client = await sv.AcceptTcpClientAsync();
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream);

        string request = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(request)) continue;

        string[] parts = request.Split('|');
        if (parts.Length == 3 && double.TryParse(parts[0], out double a) && double.TryParse(parts[1], out double b))
        {
            string op = parts[2];
            double result = op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => b != 0 ? a / b : double.NaN,
                _ => 0
            };
            await writer.WriteLineAsync(result.ToString());
            await writer.FlushAsync();
            Console.WriteLine($"[Log]: {a} {op} {b} = {result}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}