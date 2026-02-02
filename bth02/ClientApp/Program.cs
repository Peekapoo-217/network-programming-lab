using System.Net.Sockets;

Console.WriteLine("Day la client");

while (true)
{
    try
    {
        using TcpClient client = new("127.0.0.1", 8080);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream) { AutoFlush = true };

        Console.Write("\nInput 1 (hoặc 'exit'): ");
        string? input1 = Console.ReadLine();
        if (input1?.ToLower() == "exit") break;

        Console.Write("Input 2: ");
        string? input2 = Console.ReadLine();

        Console.Write("Operation (+, -, *, /): ");
        string? op = Console.ReadLine();

        await writer.WriteLineAsync($"{input1}|{input2}|{op}");

        string? response = await reader.ReadLineAsync();
        Console.WriteLine($"Result: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        break;
    }
}
