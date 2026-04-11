using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ScreenshotServer
{
    class Program
    {
        private const int Port = 6000;
        private const string OutputFolder = "CapturedImages";

        // Quản lý việc lắp ghép ảnh. Key là ImageID.
        private static ConcurrentDictionary<int, ImageAssembly> _assemblies = new ConcurrentDictionary<int, ImageAssembly>();

        static async Task Main(string[] args)
        {
            Console.WriteLine("[SERVER] Khởi chạy Screenshot Server...");
            
            // Đảm bảo thư mục lưu ảnh tồn tại
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
            Console.WriteLine($"[INFO] Ảnh sẽ được lưu tại: {Path.GetFullPath(OutputFolder)}");

            UdpClient udpServer = new UdpClient(Port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, Port);

            Console.WriteLine($"[INFO] Đang lắng nghe trên cổng {Port}...");

            // Luồng chạy ngầm để dọn dẹp các mảnh ảnh cũ không nhận đủ (timeout)
            _ = Task.Run(CleanupRoutine);

            while (true)
            {
                try
                {
                    // Nhận gói tin UDP
                    UdpReceiveResult result = await udpServer.ReceiveAsync();
                    byte[] data = result.Buffer;

                    if (data.Length < 12) continue; // Sai định dạng gói tin

                    // Giải mã Header: ImageID (4), PartIndex (4), TotalParts (4)
                    int imageId = BitConverter.ToInt32(data, 0);
                    int partIndex = BitConverter.ToInt32(data, 4);
                    int totalParts = BitConverter.ToInt32(data, 8);

                    // Lấy hoặc tạo mới trình lắp ghép ảnh
                    var assembly = _assemblies.GetOrAdd(imageId, id => new ImageAssembly(totalParts));
                    
                    // Lưu mảnh dữ liệu (Data nằm sau 12 bytes header)
                    byte[] imageData = new byte[data.Length - 12];
                    Buffer.BlockCopy(data, 12, imageData, 0, imageData.Length);
                    
                    if (assembly.AddPart(partIndex, imageData))
                    {
                        // Nếu đã nhận đủ tất cả các mảnh
                        Console.WriteLine($"[DONE] Đã nhận đủ các mảnh của ảnh #{imageId}. Tiến hành lắp ghép...");
                        
                        await SaveImage(imageId, assembly, result.RemoteEndPoint.Address.ToString());
                        
                        // Xóa khỏi cache sau khi xử lý xong
                        _assemblies.TryRemove(imageId, out _);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Lỗi xử lý: {ex.Message}");
                }
            }
        }

        private static async Task SaveImage(int imageId, ImageAssembly assembly, string clientIp)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                // Loại bỏ ký tự không hợp lệ trong IP nếu có (ví dụ ::1)
                string safeIp = clientIp.Replace(":", ".");
                string fileName = Path.Combine(OutputFolder, $"{safeIp}_{timestamp}.jpg");

                // Hợp nhất các mảnh byte
                using (MemoryStream ms = new MemoryStream())
                {
                    foreach (var part in assembly.Parts.OrderBy(p => p.Key))
                    {
                        await ms.WriteAsync(part.Value, 0, part.Value.Length);
                    }

                    // Lưu file ảnh
                    await File.WriteAllBytesAsync(fileName, ms.ToArray());
                    Console.WriteLine($"[SAVE] Đã lưu ảnh thành công: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Không thể lưu ảnh #{imageId}: {ex.Message}");
            }
        }

        private static async Task CleanupRoutine()
        {
            while (true)
            {
                await Task.Delay(10000); // Mỗi 10 giây dọn dẹp một lần
                var now = DateTime.Now;

                foreach (var entry in _assemblies)
                {
                    // Nếu ảnh đã tồn tại quá 30 giây mà chưa nhận đủ -> Xóa (UDP mất gói hoặc client dừng)
                    if ((now - entry.Value.CreationTime).TotalSeconds > 30)
                    {
                        if (_assemblies.TryRemove(entry.Key, out _))
                        {
                            Console.WriteLine($"[CLEANUP] Đã hủy bỏ ảnh #{entry.Key} do nhận thiếu mảnh.");
                        }
                    }
                }
            }
        }
    }

    public class ImageAssembly
    {
        public int TotalParts { get; }
        public ConcurrentDictionary<int, byte[]> Parts { get; } = new ConcurrentDictionary<int, byte[]>();
        public DateTime CreationTime { get; } = DateTime.Now;

        public ImageAssembly(int totalParts)
        {
            TotalParts = totalParts;
        }

        public bool AddPart(int index, byte[] data)
        {
            Parts.TryAdd(index, data);
            return Parts.Count == TotalParts;
        }
    }
}
