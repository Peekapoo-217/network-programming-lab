using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void btnBrowser(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                }
            }
        }

        private async void btnSendClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Vui lòng nhấn Browse để chọn tập tin trước!");
                return;
            }
            //port: 8080
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);

            try
            {
                listener.Start();
                txtStatus.Text = "Đang đợi Client kết nối...";

                using (TcpClient client = await listener.AcceptTcpClientAsync())
                {
                    txtStatus.Text = "Đã kết nối!";

                    using (NetworkStream ns = client.GetStream())
                    {

                        byte[] fileData = File.ReadAllBytes(txtFilePath.Text);
                        string fileName = Path.GetFileName(txtFilePath.Text);
                        byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                        byte[] fileNameLen = BitConverter.GetBytes(fileNameBytes.Length);
                        await ns.WriteAsync(fileNameLen, 0, 4);


                        await ns.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                        byte[] fileLen = BitConverter.GetBytes(fileData.Length);
                        await ns.WriteAsync(fileLen, 0, 4);

                        await ns.WriteAsync(fileData, 0, fileData.Length);

                        txtStatus.Text = "Gửi file thành công: " + fileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình gửi: " + ex.Message);
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
