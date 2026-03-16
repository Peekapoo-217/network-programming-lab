using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class ClientState
    {
        public Socket Socket;
        public byte[] Buffer = new byte[1024];
    }

    internal class Program
    {
        static void CallAccept(IAsyncResult iar)
        {
            Socket server = (Socket)iar.AsyncState;
            Socket clientSocket = server.EndAccept(iar);
            Console.WriteLine("Client da ket noi: {0}", clientSocket.RemoteEndPoint);

            server.BeginAccept(new AsyncCallback(CallAccept), server);

            ClientState state = new ClientState();
            state.Socket = clientSocket;

            clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                new AsyncCallback(ReceivedData), state);
        }

        static void ReceivedData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int recv = state.Socket.EndReceive(iar);

                if (recv == 0)
                {
                    Console.WriteLine("Client da ngat ket noi");
                    state.Socket.Close();
                    return;
                }

                string receivedData = Encoding.ASCII.GetString(state.Buffer, 0, recv);
                Console.WriteLine("Nhan tu client: {0}", receivedData);

                string[] parts = receivedData.Split(',');
                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                int sum = a + b;

                byte[] sendData = Encoding.ASCII.GetBytes(sum.ToString());
                state.Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None,
                    new AsyncCallback(SendData), state);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client mat ket noi");
                state.Socket.Close();
            }
        }

        static void SendData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int sent = state.Socket.EndSend(iar);
                Console.WriteLine("Da gui {0} byte", sent);

                state.Buffer = new byte[1024];
                state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client mat ket noi");
                state.Socket.Close();
            }
        }

        static void Main(string[] args)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 2014);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);
            Console.WriteLine("Server dang lang nghe tai port 2014...");

            server.BeginAccept(new AsyncCallback(CallAccept), server);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
