using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowser(object sender, System.EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = fbd.SelectedPath;
                }
            }
        }

        private async void btnReceive(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSavePath.Text) || !Directory.Exists(txtSavePath.Text))
            {
                MessageBox.Show("Vui lòng chọn thư mục lưu file trước!");
                return;
            }

            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 8080))
                {
                    txtStatus.Text = "Đã kết nối Server!";
                    using (NetworkStream ns = client.GetStream())
                    {
                        byte[] nameLenBytes = new byte[4];
                        await ns.ReadAsync(nameLenBytes, 0, 4);
                        int nameLen = BitConverter.ToInt32(nameLenBytes, 0);

                        byte[] nameBytes = new byte[nameLen];
                        await ns.ReadAsync(nameBytes, 0, nameLen);
                        string fileName = Encoding.UTF8.GetString(nameBytes);

                        byte[] fileLenBytes = new byte[4];
                        await ns.ReadAsync(fileLenBytes, 0, 4);
                        int fileLen = BitConverter.ToInt32(fileLenBytes, 0);

                        byte[] fileData = new byte[fileLen];
                        int totalRead = 0;
                        while (totalRead < fileLen)
                        {
                            int read = await ns.ReadAsync(fileData, totalRead, fileLen - totalRead);
                            if (read == 0) break;
                            totalRead += read;
                        }

                        string fullPath = Path.Combine(txtSavePath.Text, fileName);
                        File.WriteAllBytes(fullPath, fileData);

                        txtStatus.Text = "Đã nhận xong: " + fileName;
                        MessageBox.Show("Nhận tập tin thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
