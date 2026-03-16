using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerFormApp
{
    class ClientState
    {
        public Socket Socket;
        public byte[] Buffer = new byte[1024];
    }

    public partial class Form1 : Form
    {
        Socket server;

        public Form1()
        {
            InitializeComponent();
        }

        private void Log(string msg)
        {
            this.Invoke(new Action(() =>
            {
                txtLog.AppendText(msg + Environment.NewLine);
            }));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 2014);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);

            server.BeginAccept(new AsyncCallback(CallAccept), null);

            btnStart.Enabled = false;
            Log("Server da khoi dong tai port 2014...");
        }

        private void CallAccept(IAsyncResult iar)
        {
            try
            {
                Socket clientSocket = server.EndAccept(iar);
                Log("Client da ket noi: " + clientSocket.RemoteEndPoint);

                server.BeginAccept(new AsyncCallback(CallAccept), null);

                ClientState state = new ClientState();
                state.Socket = clientSocket;

                clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (ObjectDisposedException) { }
        }

        private void ReceivedData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int recv = state.Socket.EndReceive(iar);

                if (recv == 0)
                {
                    Log("Client da ngat ket noi");
                    state.Socket.Close();
                    return;
                }

                string receivedData = Encoding.ASCII.GetString(state.Buffer, 0, recv);
                Log("Nhan tu client: " + receivedData);

                string[] parts = receivedData.Split(',');
                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                int sum = a + b;
                Log(a + " + " + b + " = " + sum);

                byte[] sendData = Encoding.ASCII.GetBytes(sum.ToString());
                state.Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None,
                    new AsyncCallback(SendData), state);
            }
            catch (SocketException)
            {
                Log("Client mat ket noi");
                state.Socket.Close();
            }
        }

        private void SendData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int sent = state.Socket.EndSend(iar);
                Log("Da gui " + sent + " byte");

                state.Buffer = new byte[1024];
                state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (SocketException)
            {
                Log("Client mat ket noi");
                state.Socket.Close();
            }
        }
    }
}
