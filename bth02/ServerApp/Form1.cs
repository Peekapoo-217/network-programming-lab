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
        Socket server;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

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

        private void btnSendClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Vui long chon file truoc!");
                return;
            }

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8080);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(1);

            txtStatus.Text = "Dang cho Client ket noi...";
            button2.Enabled = false;

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult iar)
        {
            Socket clientSocket = server.EndAccept(iar);
            this.Invoke(new Action(() => { txtStatus.Text = "Client da ket noi! Dang gui file..."; }));

            // Đọc file
            string filePath = "";
            this.Invoke(new Action(() => { filePath = txtFilePath.Text; }));

            byte[] fileData = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // Ghép tất cả vào 1 mảng: [fileNameLen(4)] + [fileName] + [fileLen(4)] + [fileData]
            byte[] fileNameLen = BitConverter.GetBytes(fileNameBytes.Length);
            byte[] fileLen = BitConverter.GetBytes(fileData.Length);

            byte[] allData = new byte[4 + fileNameBytes.Length + 4 + fileData.Length];
            int offset = 0;

            Buffer.BlockCopy(fileNameLen, 0, allData, offset, 4);
            offset += 4;
            Buffer.BlockCopy(fileNameBytes, 0, allData, offset, fileNameBytes.Length);
            offset += fileNameBytes.Length;
            Buffer.BlockCopy(fileLen, 0, allData, offset, 4);
            offset += 4;
            Buffer.BlockCopy(fileData, 0, allData, offset, fileData.Length);

            // Gửi bất đồng bộ
            clientSocket.BeginSend(allData, 0, allData.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), clientSocket);
        }

        private void SendCallback(IAsyncResult iar)
        {
            Socket clientSocket = (Socket)iar.AsyncState;
            try
            {
                int sent = clientSocket.EndSend(iar);
                string fileName = "";
                this.Invoke(new Action(() => { fileName = Path.GetFileName(txtFilePath.Text); }));

                this.Invoke(new Action(() =>
                {
                    txtStatus.Text = "Gui file thanh cong: " + fileName;
                    button2.Enabled = true;
                }));
            }
            catch (SocketException ex)
            {
                this.Invoke(new Action(() =>
                {
                    txtStatus.Text = "Loi gui: " + ex.Message;
                    button2.Enabled = true;
                }));
            }
            finally
            {
                clientSocket.Close();
                server.Close();
            }
        }
    }
}
