using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientApp
{
    class ReceiveState
    {
        public Socket Socket;
        public byte[] Buffer;
        public int TotalRead;
        public int Expected;
        public MemoryStream Stream = new MemoryStream();
        public string FileName;
        public string SavePath;
        public int Phase;
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowser(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnReceive(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSavePath.Text) || !Directory.Exists(txtSavePath.Text))
            {
                MessageBox.Show("Vui long chon thu muc luu file truoc!");
                return;
            }

            button2.Enabled = false;
            txtStatus.Text = "Dang ket noi den Server...";

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

            ReceiveState state = new ReceiveState();
            state.Socket = client;
            state.SavePath = txtSavePath.Text;
            state.Phase = 0;
            state.Expected = 4; // Đọc 4 byte đầu (fileNameLen)
            state.Buffer = new byte[4];
            state.TotalRead = 0;

            client.BeginConnect(iep, new AsyncCallback(ConnectCallback), state);
        }

        private void ConnectCallback(IAsyncResult iar)
        {
            ReceiveState state = (ReceiveState)iar.AsyncState;
            try
            {
                state.Socket.EndConnect(iar);
                this.Invoke(new Action(() => { txtStatus.Text = "Da ket noi! Dang nhan file..."; }));

                // Bắt đầu nhận dữ liệu (phase 0: đọc fileNameLen - 4 bytes)
                state.Socket.BeginReceive(state.Buffer, 0, state.Expected, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (SocketException ex)
            {
                this.Invoke(new Action(() =>
                {
                    txtStatus.Text = "Khong the ket noi: " + ex.Message;
                    button2.Enabled = true;
                }));
            }
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            ReceiveState state = (ReceiveState)iar.AsyncState;
            try
            {
                int recv = state.Socket.EndReceive(iar);
                if (recv == 0)
                {
                    this.Invoke(new Action(() => { txtStatus.Text = "Server ngat ket noi"; button2.Enabled = true; }));
                    state.Socket.Close();
                    return;
                }

                // Phase 3 (đọc file data) xử lý riêng vì buffer chỉ 8KB, TotalRead tích lũy cả file
                if (state.Phase == 3)
                {
                    state.Stream.Write(state.Buffer, 0, recv);
                    state.TotalRead += recv;

                    if (state.TotalRead < state.Expected)
                    {
                        int remaining = state.Expected - state.TotalRead;
                        int nextRead = Math.Min(8192, remaining);
                        state.Socket.BeginReceive(state.Buffer, 0, nextRead, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        string fullPath = Path.Combine(state.SavePath, state.FileName);
                        File.WriteAllBytes(fullPath, state.Stream.ToArray());
                        state.Stream.Close();
                        state.Socket.Close();

                        this.Invoke(new Action(() =>
                        {
                            txtStatus.Text = "Da nhan xong: " + state.FileName;
                            button2.Enabled = true;
                            MessageBox.Show("Nhan file thanh cong!");
                        }));
                    }
                    return;
                }

                // Phase 0, 1, 2: buffer đúng bằng Expected, dùng TotalRead làm offset
                state.TotalRead += recv;

                if (state.TotalRead < state.Expected)
                {
                    state.Socket.BeginReceive(state.Buffer, state.TotalRead, state.Expected - state.TotalRead,
                        SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    return;
                }

                // Đọc đủ → chuyển phase
                switch (state.Phase)
                {
                    case 0: // Đã đọc xong fileNameLen (4 bytes)
                        int nameLen = BitConverter.ToInt32(state.Buffer, 0);
                        state.Phase = 1;
                        state.Buffer = new byte[nameLen];
                        state.Expected = nameLen;
                        state.TotalRead = 0;
                        state.Socket.BeginReceive(state.Buffer, 0, nameLen, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), state);
                        break;

                    case 1: // Đã đọc xong fileName
                        state.FileName = Encoding.UTF8.GetString(state.Buffer);
                        state.Phase = 2;
                        state.Buffer = new byte[4];
                        state.Expected = 4;
                        state.TotalRead = 0;
                        state.Socket.BeginReceive(state.Buffer, 0, 4, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), state);
                        break;

                    case 2: // Đã đọc xong fileLen (4 bytes)
                        int fileLen = BitConverter.ToInt32(state.Buffer, 0);
                        state.Phase = 3;
                        state.Buffer = new byte[8192];
                        state.Expected = fileLen;
                        state.TotalRead = 0;
                        state.Stream = new MemoryStream();
                        int toRead = Math.Min(8192, fileLen);
                        state.Socket.BeginReceive(state.Buffer, 0, toRead, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), state);
                        break;
                }
            }
            catch (SocketException ex)
            {
                this.Invoke(new Action(() =>
                {
                    txtStatus.Text = "Loi nhan: " + ex.Message;
                    button2.Enabled = true;
                }));
                state.Socket.Close();
            }
        }
    }
}
