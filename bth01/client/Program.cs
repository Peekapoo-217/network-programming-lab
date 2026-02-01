// CODE CLIENT
using System.Net;
using System.Net.Sockets;
using System.Text;

// 1. Kết nối đến Server
// Sửa lỗi cú pháp: Parse IP riêng, sau đó mới đưa vào IPEndPoint cùng Port
IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014);
Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    Console.WriteLine("Dang ket noi den Server...");
    client.Connect(iep);
    Console.WriteLine("Da ket noi thanh cong!");

    while (true)
    {
        // 2. Nhập dữ liệu từ bàn phím
        Console.Write("Nhap chuoi (go 'exit' de thoat): ");
        string input = Console.ReadLine();

        // Điều kiện thoát vòng lặp
        if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
        {
            break;
        }

        // 3. Gửi dữ liệu lên Server (Mã hóa sang byte)
        byte[] dataSend = Encoding.ASCII.GetBytes(input);
        client.Send(dataSend);

        // 4. Nhận kết quả trả về từ Server
        byte[] dataRecv = new byte[1024];
        // Hàm Receive sẽ chờ đến khi Server gửi dữ liệu về
        int bytesReceived = client.Receive(dataRecv);

        if (bytesReceived == 0) break; 

        string result = Encoding.ASCII.GetString(dataRecv, 0, bytesReceived);
        Console.WriteLine("Ket qua tu Server: " + result);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Loi ket noi: " + ex.Message);
}
finally
{
    // Đóng kết nối gọn gàng
    client.Close();
}