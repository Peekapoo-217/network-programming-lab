// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
while (true)
{
    Console.Write("Input: ");
    string input = Console.ReadLine();
    if (input == "exit") break;

    IPAddress address;
    bool isValid = IPAddress.TryParse(input, out address);
    if (isValid)
    {
        if(address.AddressFamily == AddressFamily.InterNetwork)
        {
            if (input.Split('.').Length == 4)
            {
                Console.WriteLine("Ipv4");
            }
            else
            {
                Console.WriteLine("Unformat Ipv4");
            }
        }
        else if(address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            Console.WriteLine("Ipv6");break;
        }
    }

    Console.WriteLine("Invalid address");
}
