using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private string rootPath;
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
            btnSelectRoot.Click += BtnSelectRoot_Click;
        }

        private void BtnSelectRoot_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    rootPath = fbd.SelectedPath;
                    txtRootPath.Text = rootPath;
                    StartServer();
                }
            }
        }

        private async void StartServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 8080);
                listener.Start();
                isRunning = true;
                UpdateStatus("Server đang chạy trên port 8080...");
                
                while (isRunning)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Lỗi: " + ex.Message);
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            string clientEndpoint = client.Client.RemoteEndPoint.ToString();
            AddLog($"Client kết nối: {clientEndpoint}");
            
            try
            {
                using (var ns = client.GetStream())
                using (var reader = new StreamReader(ns, Encoding.UTF8))
                using (var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true })
                {
                    string currentPath = "";
                    
                    while (client.Connected)
                    {
                        string command = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(command)) break;
                        
                        AddLog($"[{clientEndpoint}] Command: {command}");
                        
                        string[] parts = command.Split('|');
                        string cmd = parts[0];
                        
                        try
                        {
                            switch (cmd)
                            {
                                case "LIST":
                                    await HandleList(writer, currentPath);
                                    break;
                                    
                                case "CD":
                                    currentPath = HandleCD(writer, currentPath, parts[1]);
                                    break;
                                    
                                case "BACK":
                                    currentPath = HandleBack(writer, currentPath);
                                    break;
                                    
                                case "MKDIR":
                                    HandleMkdir(writer, currentPath, parts[1]);
                                    break;
                                    
                                case "MKFILE":
                                    HandleMkfile(writer, currentPath, parts[1]);
                                    break;
                                    
                                case "DELETE":
                                    HandleDelete(writer, currentPath, parts[1]);
                                    break;
                                    
                                case "DOWNLOAD":
                                    await HandleDownload(ns, writer, currentPath, parts[1]);
                                    break;
                                    
                                case "UPLOAD":
                                    await HandleUpload(ns, reader, currentPath, parts[1], long.Parse(parts[2]));
                                    break;
                                    
                                default:
                                    await writer.WriteLineAsync("ERROR|Unknown command");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            await writer.WriteLineAsync($"ERROR|{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"[{clientEndpoint}] Lỗi: {ex.Message}");
            }
            finally
            {
                client.Close();
                AddLog($"Client ngắt kết nối: {clientEndpoint}");
            }
        }

        private async Task HandleList(StreamWriter writer, string currentPath)
        {
            string fullPath = Path.Combine(rootPath, currentPath);
            var sb = new StringBuilder();
            
            foreach (var dir in Directory.GetDirectories(fullPath))
            {
                sb.AppendLine($"FOLDER|{Path.GetFileName(dir)}");
            }
            
            foreach (var file in Directory.GetFiles(fullPath))
            {
                var fi = new FileInfo(file);
                sb.AppendLine($"FILE|{Path.GetFileName(file)}|{fi.Length}");
            }
            
            await writer.WriteLineAsync($"OK|{currentPath}");
            await writer.WriteLineAsync(sb.ToString().TrimEnd());
            await writer.WriteLineAsync("END");
        }

        private string HandleCD(StreamWriter writer, string currentPath, string folderName)
        {
            string newPath = Path.Combine(currentPath, folderName);
            string fullPath = Path.Combine(rootPath, newPath);
            
            if (Directory.Exists(fullPath))
            {
                writer.WriteLine("OK");
                return newPath;
            }
            else
            {
                writer.WriteLine("ERROR|Thư mục không tồn tại");
                return currentPath;
            }
        }

        private string HandleBack(StreamWriter writer, string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                writer.WriteLine("OK");
                return currentPath;
            }
            
            string newPath = Path.GetDirectoryName(currentPath) ?? "";
            writer.WriteLine("OK");
            return newPath;
        }

        private void HandleMkdir(StreamWriter writer, string currentPath, string folderName)
        {
            string fullPath = Path.Combine(rootPath, currentPath, folderName);
            Directory.CreateDirectory(fullPath);
            writer.WriteLine("OK");
        }

        private void HandleMkfile(StreamWriter writer, string currentPath, string fileName)
        {
            string fullPath = Path.Combine(rootPath, currentPath, fileName);
            File.WriteAllText(fullPath, "");
            writer.WriteLine("OK");
        }

        private void HandleDelete(StreamWriter writer, string currentPath, string name)
        {
            string fullPath = Path.Combine(rootPath, currentPath, name);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                writer.WriteLine("OK");
            }
            else if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                writer.WriteLine("OK");
            }
            else
            {
                writer.WriteLine("ERROR|Không tìm thấy file/folder");
            }
        }

        private async Task HandleDownload(NetworkStream ns, StreamWriter writer, string currentPath, string fileName)
        {
            string fullPath = Path.Combine(rootPath, currentPath, fileName);
            
            if (!File.Exists(fullPath))
            {
                await writer.WriteLineAsync("ERROR|File không tồn tại");
                return;
            }
            
            var fileData = File.ReadAllBytes(fullPath);
            await writer.WriteLineAsync($"OK|{fileData.Length}");
            await ns.WriteAsync(fileData, 0, fileData.Length);
            await ns.FlushAsync();
        }

        private async Task HandleUpload(NetworkStream ns, StreamReader reader, string currentPath, string fileName, long fileSize)
        {
            string fullPath = Path.Combine(rootPath, currentPath, fileName);
            
            byte[] buffer = new byte[fileSize];
            int totalRead = 0;
            
            while (totalRead < fileSize)
            {
                int read = await ns.ReadAsync(buffer, totalRead, (int)(fileSize - totalRead));
                if (read == 0) throw new Exception("Connection lost");
                totalRead += read;
            }
            
            File.WriteAllBytes(fullPath, buffer);
            
            var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };
            await writer.WriteLineAsync("OK");
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => lbStatus.Text = message));
            }
            else
            {
                lbStatus.Text = message;
            }
        }

        private void AddLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => lbLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}")));
            }
            else
            {
                lbLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }

        private void lbLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isRunning = false;
            listener?.Stop();
            base.OnFormClosing(e);
        }
    }
}
