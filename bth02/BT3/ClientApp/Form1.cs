using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private string currentPath = "";
        private bool isConnected = false;

        public Form1()
        {
            InitializeComponent();
            lvFile.DoubleClick += LvFile_DoubleClick;
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Thử kết nối với retry mechanism
            int maxRetries = 10;
            int retryDelay = 1000; // 1 giây

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    txtcurrentPath.Text = $"Đang kết nối đến server... (lần {i + 1}/{maxRetries})";

                    client = new TcpClient();
                    await client.ConnectAsync("127.0.0.1", 8080);
                    ns = client.GetStream();
                    reader = new StreamReader(ns, Encoding.UTF8);
                    writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

                    isConnected = true;
                    await RefreshList();
                    return; // Kết nối thành công, thoát
                }
                catch (Exception)
                {
                    // Nếu chưa phải lần cuối, đợi rồi thử lại
                    if (i < maxRetries - 1)
                    {
                        await Task.Delay(retryDelay);
                    }
                }
            }

            // Sau khi retry hết vẫn không connect được
            var result = MessageBox.Show(
                "Không thể kết nối đến Server!\n\n" +
                "Hãy đảm bảo Server đã khởi động và chọn thư mục.\n\n" +
                "Bấm 'Thử lại' để kết nối lại, hoặc 'Hủy' để thoát.",
                "Lỗi kết nối",
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Retry)
            {
                Form1_Load(sender, e); // Thử lại
            }
            else
            {
                Application.Exit();
            }
        }

        private async Task RefreshList()
        {
            await writer.WriteLineAsync("LIST");

            string response = await reader.ReadLineAsync();
            if (response.StartsWith("OK"))
            {
                currentPath = response.Split('|')[1];
                txtcurrentPath.Text = string.IsNullOrEmpty(currentPath) ? "\\" : "\\" + currentPath;

                lvFile.Items.Clear();

                string line;
                while ((line = await reader.ReadLineAsync()) != "END")
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    var item = new ListViewItem(parts[1]);

                    if (parts[0] == "FOLDER")
                    {
                        item.SubItems.Add("Folder");
                        item.Tag = "FOLDER";
                    }
                    else
                    {
                        item.SubItems.Add(FormatFileSize(long.Parse(parts[2])));
                        item.Tag = "FILE";
                    }

                    lvFile.Items.Add(item);
                }
            }
        }

        private async void LvFile_DoubleClick(object sender, EventArgs e)
        {
            if (lvFile.SelectedItems.Count == 0) return;

            var item = lvFile.SelectedItems[0];
            if (item.Tag.ToString() == "FOLDER")
            {
                await writer.WriteLineAsync($"CD|{item.Text}");
                string response = await reader.ReadLineAsync();

                if (response == "OK")
                {
                    await RefreshList();
                }
                else
                {
                    MessageBox.Show(response.Split('|')[1]);
                }
            }
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            await writer.WriteLineAsync("BACK");
            string response = await reader.ReadLineAsync();

            if (response == "OK")
            {
                await RefreshList();
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (!isConnected || writer == null)
            {
                MessageBox.Show("Chưa kết nối đến server!");
                return;
            }

            using (var form = new Form())
            {
                form.Text = "Tạo mới";
                form.Width = 350;
                form.Height = 200;
                form.StartPosition = FormStartPosition.CenterParent;

                var label = new Label { Text = "Tên:", Left = 20, Top = 20, Width = 80 };
                var txtName = new TextBox { Left = 100, Top = 20, Width = 200 };
                var rbFile = new RadioButton { Text = "File", Left = 100, Top = 50, Checked = true };
                var rbFolder = new RadioButton { Text = "Folder", Left = 180, Top = 50 };
                var btnOk = new Button { Text = "OK", Left = 100, Top = 90, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Hủy", Left = 200, Top = 90, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, txtName, rbFile, rbFolder, btnOk, btnCancel });
                form.AcceptButton = btnOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string name = txtName.Text.Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("Vui lòng nhập tên!");
                        return;
                    }

                    string cmd = rbFile.Checked ? "MKFILE" : "MKDIR";
                    await writer.WriteLineAsync($"{cmd}|{name}");
                    string response = await reader.ReadLineAsync();

                    if (response == "OK")
                    {
                        await RefreshList();
                    }
                    else
                    {
                        MessageBox.Show(response.Split('|')[1]);
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!isConnected || writer == null)
            {
                MessageBox.Show("Chưa kết nối đến server!");
                return;
            }

            if (lvFile.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn file/folder để xóa!");
                return;
            }

            var item = lvFile.SelectedItems[0];
            var result = MessageBox.Show($"Bạn có chắc muốn xóa '{item.Text}'?", "Xác nhận",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await writer.WriteLineAsync($"DELETE|{item.Text}");
                string response = await reader.ReadLineAsync();

                if (response == "OK")
                {
                    await RefreshList();
                }
                else
                {
                    MessageBox.Show(response.Split('|')[1]);
                }
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (!isConnected || writer == null)
            {
                MessageBox.Show("Chưa kết nối đến server!");
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] fileData = File.ReadAllBytes(ofd.FileName);
                        string fileName = Path.GetFileName(ofd.FileName);

                        await writer.WriteLineAsync($"UPLOAD|{fileName}|{fileData.Length}");
                        await ns.WriteAsync(fileData, 0, fileData.Length);
                        await ns.FlushAsync();

                        string response = await reader.ReadLineAsync();
                        if (response == "OK")
                        {
                            MessageBox.Show("Upload thành công!");
                            await RefreshList();
                        }
                        else
                        {
                            MessageBox.Show(response.Split('|')[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi upload: " + ex.Message);
                    }
                }
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (!isConnected || writer == null)
            {
                MessageBox.Show("Chưa kết nối đến server!");
                return;
            }

            if (lvFile.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn file để download!");
                return;
            }

            var item = lvFile.SelectedItems[0];
            if (item.Tag.ToString() != "FILE")
            {
                MessageBox.Show("Chỉ có thể download file!");
                return;
            }

            using (var sfd = new SaveFileDialog { FileName = item.Text })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await writer.WriteLineAsync($"DOWNLOAD|{item.Text}");
                        string response = await reader.ReadLineAsync();

                        if (response.StartsWith("OK"))
                        {
                            long fileSize = long.Parse(response.Split('|')[1]);
                            byte[] buffer = new byte[fileSize];
                            int totalRead = 0;

                            while (totalRead < fileSize)
                            {
                                int read = await ns.ReadAsync(buffer, totalRead, (int)(fileSize - totalRead));
                                if (read == 0) throw new Exception("Connection lost");
                                totalRead += read;
                            }

                            File.WriteAllBytes(sfd.FileName, buffer);
                            MessageBox.Show("Download thành công!");
                        }
                        else
                        {
                            MessageBox.Show(response.Split('|')[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi download: " + ex.Message);
                    }
                }
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtcurrentPath_TextChanged(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            client?.Close();
            base.OnFormClosing(e);
        }
    }
}
