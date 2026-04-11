using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        private const string ServerIp = "127.0.0.1";
        private const int Port = 5000;

        static async Task Main(string[] args)
        {
            // Thiết lập mã hóa UTF-8 để hiển thị tiếng Việt trên Console
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            TcpClient client = new TcpClient();

            try
            {
                Console.WriteLine("[CLIENT] Đang kết nối tới Server...");
                await client.ConnectAsync(ServerIp, Port);
                Console.WriteLine("[CLIENT] Kết nối thành công!");

                NetworkStream stream = client.GetStream();
                // StreamReader và StreamWriter bọc quanh NetworkStream để xử lý chuỗi ký tự
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Gửi username lên server đầu tiên
                Console.Write("Nhập tên của bạn: ");
                string username = Console.ReadLine();
                await writer.WriteLineAsync(username);

                // Khởi tạo luồng chạy ngầm để liên tục lắng nghe tin nhắn từ Server
                // Sử dụng Task.Run để không làm chặn luồng nhập (Main Thread)
                _ = Task.Run(() => ReceiveMessages(reader));

                Console.WriteLine("--- Bắt đầu Chat (Gõ 'exit' để thoát) ---");

                // Luồng chính (Main Thread) dùng để đọc dữ liệu từ bàn phím và gửi đi
                while (true)
                {
                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;

                    if (input.ToLower() == "exit")
                    {
                        break;
                    }

                    // Gửi tin nhắn tới Server
                    await writer.WriteLineAsync(input);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("[ERROR] Không thể kết nối tới Server. Hãy đảm bảo Server đang chạy.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Có lỗi xảy ra: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("[CLIENT] Đã ngắt kết nối.");
            }
        }

        /// <summary>
        /// Luồng nhận tin nhắn chạy song song với luồng gửi tin
        /// </summary>
        private static async Task ReceiveMessages(StreamReader reader)
        {
            try
            {
                string message;
                // ReadLineAsync sẽ treo ở đây cho đến khi có dữ liệu từ Server gửi về
                while ((message = await reader.ReadLineAsync()) != null)
                {
                    // Khi nhận được tin nhắn, in ra màn hình
                    Console.WriteLine(message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[INFO] Mất kết nối từ Server.");
            }
        }
    }
}
