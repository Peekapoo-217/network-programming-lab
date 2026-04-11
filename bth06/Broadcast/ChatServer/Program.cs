using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        // Sử dụng ConcurrentDictionary để quản lý danh sách Client kết nối.
        // Key: Username, Value: TcpClient.
        // ConcurrentDictionary giúp đảm bảo an toàn đa luồng (Thread-safety) khi nhiều luồng cùng thêm/xóa client.
        private static ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();

        // Port mặc định cho Server
        private const int Port = 5000;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Khởi tạo TcpListener để lắng nghe các kết nối TCP tới cổng Port
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            
            try
            {
                listener.Start();
                Console.WriteLine($"[SERVER] Server đã khởi động tại cổng {Port}...");
                Console.WriteLine("[SERVER] Đang chờ các Client kết nối...");

                while (true)
                {
                    // Chấp nhận một kết nối mới từ Client (Bất đồng bộ)
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    
                    // Xử lý mỗi Client trong một Task riêng biệt (tương đương luồng chạy ngầm)
                    // Việc này giúp Server có thể phục vụ hàng trăm Client cùng lúc mà không bị chặn (blocking)
                    _ = Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi Server: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            string username = "";
            NetworkStream stream = client.GetStream();
            
            // StreamReader và StreamWriter giúp làm việc với dữ liệu text trên Stream dễ dàng hơn
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                // Bước đầu tiên: Yêu cầu Client nhập Username
                username = await reader.ReadLineAsync();

                if (string.IsNullOrEmpty(username) || _clients.ContainsKey(username))
                {
                    await writer.WriteLineAsync("ERROR: Username đã tồn tại hoặc không hợp lệ. Đang ngắt kết nối.");
                    client.Close();
                    return;
                }

                // Lưu Client vào danh sách quản lý
                _clients.TryAdd(username, client);
                Console.WriteLine($"[CONN] {username} đã tham gia phòng chat. (IP: {client.Client.RemoteEndPoint})");

                // Thông báo cho tất cả mọi người là có thành viên mới
                Broadcast($"--- {username} đã tham gia phòng chat ---", "SYSTEM");

                string message;
                // Vòng lặp liên tục lắng nghe tin nhắn từ Client này
                while ((message = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(message)) continue;

                    // Định dạng tin nhắn: [HH:mm:ss] [Username]: Message
                    string formattedMsg = $"[{DateTime.Now:HH:mm:ss}] {username}: {message}";
                    Console.WriteLine(formattedMsg);

                    // Gửi tin nhắn này tới tất cả các Client khác
                    Broadcast(formattedMsg, username);
                }
            }
            catch (IOException)
            {
                // Xử lý khi Client ngắt kết nối đột ngột
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi xử lý client {username}: {ex.Message}");
            }
            finally
            {
                // Khi Client rời đi hoặc lỗi xảy ra, dọn dẹp tài nguyên
                if (!string.IsNullOrEmpty(username))
                {
                    _clients.TryRemove(username, out _);
                    Console.WriteLine($"[DISC] {username} đã rời phòng chat.");
                    Broadcast($"--- {username} đã rời phòng chat ---", "SYSTEM");
                }
                client.Close();
            }
        }

        /// <summary>
        /// Phát tin nhắn tới tất cả Client đang kết nối (trừ người gửi nếu cần)
        /// </summary>
        private static void Broadcast(string message, string sender)
        {
            foreach (var clientPair in _clients)
            {
                try
                {
                    // Không gửi lại chính tin nhắn đó cho người gửi (trừ khi là tin hệ thống)
                    if (clientPair.Key == sender) continue;

                    TcpClient client = clientPair.Value;
                    if (client.Connected)
                    {
                        // NetworkStream là luồng dữ liệu thô giữa Server và Client
                        NetworkStream stream = client.GetStream();
                        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                        writer.WriteLine(message);
                    }
                }
                catch
                {
                    // Nếu lỗi khi gửi cho một client, ta có thể bỏ qua hoặc xử lý dọn dẹp sau
                }
            }
        }
    }
}
