using System.Net;
using System.Net.Sockets;
using System.Text;

string serverIP = "127.0.0.1";
int serverPort = 6000;

Console.WriteLine("[AccountClient] User Account Management");
Console.WriteLine("========================================\n");

while (true)
{
    Console.WriteLine("1. Register");
    Console.WriteLine("2. Login");
    Console.WriteLine("3. Change Password");
    Console.WriteLine("4. Delete Account");
    Console.WriteLine("5. Exit");
    Console.Write("\nSelect option: ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    string? request = null;

    switch (choice)
    {
        case "1":
            Console.Write("Username: ");
            string? regUser = Console.ReadLine();
            Console.Write("Password: ");
            string? regPass = Console.ReadLine();
            request = $"REGISTER|{regUser}|{regPass}";
            break;

        case "2":
            Console.Write("Username: ");
            string? loginUser = Console.ReadLine();
            Console.Write("Password: ");
            string? loginPass = Console.ReadLine();
            request = $"LOGIN|{loginUser}|{loginPass}";
            break;

        case "3":
            Console.Write("Username: ");
            string? chgUser = Console.ReadLine();
            Console.Write("Current Password: ");
            string? oldPass = Console.ReadLine();
            Console.Write("New Password: ");
            string? newPass = Console.ReadLine();
            request = $"CHANGE_PASSWORD|{chgUser}|{oldPass}|{newPass}";
            break;

        case "4":
            Console.Write("Username: ");
            string? delUser = Console.ReadLine();
            Console.Write("Password: ");
            string? delPass = Console.ReadLine();
            request = $"DELETE|{delUser}|{delPass}";
            break;

        case "5":
            Console.WriteLine("[AccountClient] Exiting.");
            return;

        default:
            Console.WriteLine("Invalid option. Please try again.\n");
            continue;
    }

    if (request != null)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort));

        byte[] data = Encoding.UTF8.GetBytes(request);
        socket.Send(data);

        byte[] buffer = new byte[1024];
        int bytesRead = socket.Receive(buffer);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"[Server] {response}\n");

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
