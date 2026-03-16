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
        static Socket server;

        private static void CallAccept(IAsyncResult iar)
        {
            try
            {
                Socket serverSocket = (Socket)iar.AsyncState;
                Socket clientSocket = serverSocket.EndAccept(iar);
                Console.WriteLine("Client da ket noi: {0}", clientSocket.RemoteEndPoint);

                serverSocket.BeginAccept(new AsyncCallback(CallAccept), serverSocket);

                ClientState state = new ClientState();
                state.Socket = clientSocket;

                // Bắt đầu nhận dữ liệu từ client
                clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (ObjectDisposedException)
            {
                // Server đã đóng, bỏ qua
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi Accept: " + ex.Message);
            }
        }

        static void ReceivedData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int recv = state.Socket.EndReceive(iar);

                // Kiểm tra client ngắt kết nối
                if (recv == 0)
                {
                    Console.WriteLine("Client da ngat ket noi: {0}", state.Socket.RemoteEndPoint);
                    state.Socket.Close();
                    return;
                }

                string receivedData = Encoding.ASCII.GetString(state.Buffer, 0, recv);
                Console.WriteLine("Nhan tu client: {0}", receivedData);

                // Đổi sang chữ HOA
                string upperData = receivedData.ToUpper();
                byte[] sendData = Encoding.ASCII.GetBytes(upperData);

                // Gửi lại cho client
                state.Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None,
                    new AsyncCallback(SendData), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi Receive: " + ex.Message);
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

                // Reset buffer và tiếp tục nhận dữ liệu
                state.Buffer = new byte[1024];
                state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client mat ket noi");
                state.Socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi Send: " + ex.Message);
                state.Socket.Close();
            }
        }

        static void Main(string[] args)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 2014);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);

            server.BeginAccept(new AsyncCallback(CallAccept), server);

            Console.WriteLine("Nhan Enter de dung server.");
            Console.ReadLine();

            server.Close();
            Console.WriteLine("Server da dung.");
        }
    }
}
