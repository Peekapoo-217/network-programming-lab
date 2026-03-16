using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientFormApp
{
    public partial class Form1 : Form
    {
        Socket client;
        byte[] buffer = new byte[1024];

        public Form1()
        {
            InitializeComponent();
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(iep, new AsyncCallback(Connected), null);
        }

        private void Connected(IAsyncResult iar)
        {
            try
            {
                client.EndConnect(iar);
                this.Invoke(new Action(() =>
                {
                    btnKetNoi.Enabled = false;
                    btnGui.Enabled = true;
                    this.Text = "Client - Da ket noi";
                }));
            }
            catch (SocketException)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Khong the ket noi den server");
                }));
            }
        }

        private void btnGui_Click(object sender, EventArgs e)
        {
            string message = txtSo1.Text + "," + txtSo2.Text;
            byte[] sendData = Encoding.ASCII.GetBytes(message);
            client.BeginSend(sendData, 0, sendData.Length, SocketFlags.None,
                new AsyncCallback(SendData), null);
        }

        private void SendData(IAsyncResult iar)
        {
            int sent = client.EndSend(iar);
            buffer = new byte[1024];
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(ReceivedData), null);
        }

        private void ReceivedData(IAsyncResult iar)
        {
            try
            {
                int recv = client.EndReceive(iar);
                if (recv == 0) return;

                string result = Encoding.ASCII.GetString(buffer, 0, recv);
                this.Invoke(new Action(() =>
                {
                    txtKetQua.Text = result;
                }));
            }
            catch (SocketException ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Loi: " + ex.Message);
                }));
            }
        }
    }
}
