using System.Net;
using System.Net.Sockets;

Console.Write("Nhap dia chi IPv4: ");
string input = Console.ReadLine();

ClassifyIP(input);

static void ClassifyIP(string ipString)
{
    // 1. Kiem tra xem co dung la dinh dang IP hay khong
    if (IPAddress.TryParse(ipString, out IPAddress address))
    {
        // 2. Chi xu ly neu la IPv4
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            byte[] bytes = address.GetAddressBytes();
            int firstByte = bytes[0];

            // 3. Xet tung truong hop
            if (IPAddress.IsLoopback(address))
            {
                Console.WriteLine($"{ipString} la dia chi: Loopback");
            }
            else if (ipString == "255.255.255.255")
            {
                Console.WriteLine($"{ipString} la dia chi: Broadcast");
            }
            else if (firstByte >= 224 && firstByte <= 239)
            {
                Console.WriteLine($"{ipString} la dia chi: Multicast");
            }
            else if (firstByte >= 1 && firstByte <= 223)
            {
                // Bao gom cac lop A, B, C (loai tru Loopback 127)
                Console.WriteLine($"{ipString} la dia chi: Unicast");
            }
            else
            {
                Console.WriteLine($"{ipString} thuoc dai danh rieng (Reserved/Experimental)");
            }
        }
        else
        {
            Console.WriteLine("Day la IPv6, vui long nhap IPv4.");
        }
    }
    else
    {
        Console.WriteLine("Dia chi IP khong hop le!");
    }
}