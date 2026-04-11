using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotClient
{
    class Program
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 6000;
        private const int MaxChunkSize = 60000; // Ngưỡng an toàn cho gói tin UDP

        static async Task Main(string[] args)
        {
            Console.WriteLine("[CLIENT] Khởi chạy Screenshot Client...");
            UdpClient udpClient = new UdpClient();
            int imageId = 0;

            while (true)
            {
                try
                {
                    imageId++;
                    Console.WriteLine($"\n[INFO] Bắt đầu chụp ảnh #{imageId}...");

                    // 1. Chụp màn hình
                    byte[] imageData = CaptureScreen();
                    
                    if (imageData != null)
                    {
                        // 2. Chia nhỏ và gửi dữ liệu
                        await SendImageData(udpClient, imageId, imageData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Lỗi: {ex.Message}");
                }

                // Chờ n giây trước khi chụp bức tiếp theo (yêu cầu là 5 giây)
                Console.WriteLine("[INFO] Chờ 5 giây...");
                await Task.Delay(5000);
            }
        }

        private static byte[] CaptureScreen()
        {
            try
            {
                // Lấy kích thước toàn màn hình
                Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // Chụp màn hình
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    // Nén ảnh sang JPG và lưu vào mảng byte
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Không thể chụp màn hình: {ex.Message}");
                return null;
            }
        }

        private static async Task SendImageData(UdpClient udpClient, int imageId, byte[] imageData)
        {
            int totalParts = (int)Math.Ceiling((double)imageData.Length / MaxChunkSize);
            Console.WriteLine($"[SEND] Kích thước ảnh: {imageData.Length} bytes, chia làm {totalParts} mảnh.");

            for (int i = 0; i < totalParts; i++)
            {
                int chunkSize = Math.Min(MaxChunkSize, imageData.Length - (i * MaxChunkSize));
                
                // Tạo mảng byte cho gói tin (Header 12 bytes + Data)
                byte[] packet = new byte[12 + chunkSize];

                // Header: ImageID (4), PartIndex (4), TotalParts (4)
                Buffer.BlockCopy(BitConverter.GetBytes(imageId), 0, packet, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(i), 0, packet, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(totalParts), 0, packet, 8, 4);

                // Dữ liệu ảnh
                Buffer.BlockCopy(imageData, i * MaxChunkSize, packet, 12, chunkSize);

                // Gửi qua UDP
                await udpClient.SendAsync(packet, packet.Length, ServerIp, ServerPort);
                
                // Một khoảng nghỉ cực ngắn giúp giảm nghẽn mạng và mất gói (UDP)
                Thread.Sleep(5); 
            }

            Console.WriteLine($"[SEND] Đã gửi xong tất cả {totalParts} mảnh của ảnh #{imageId}.");
        }
    }
}
