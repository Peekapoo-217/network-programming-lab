using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientApp
{
    // State object cho client
    class ClientState
    {
        public Socket Socket;
        public byte[] Buffer = new byte[1024];
    }

    internal class Program
    {

        public static void Connected(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                state.Socket.EndConnect(iar);
                Console.WriteLine("Da ket noi den server!");
            }
            catch (SocketException)
            {
                Console.WriteLine("Khong the ket noi den server");
                return;
            }

            // Bắt đầu vòng lặp: nhập và gửi
            PromptAndSend(state);
        }

        private static void PromptAndSend(ClientState state)
        {
            Console.Write("Nhap thong diep (hoac 'exit' de thoat): ");
            string input = Console.ReadLine();

            byte[] sendData = Encoding.ASCII.GetBytes(input);
            state.Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None,
                new AsyncCallback(SendData), state);
        }

        private static void SendData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int sent = state.Socket.EndSend(iar);

                // Nhận phản hồi từ server
                state.Buffer = new byte[1024];
                state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceivedData), state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Loi gui du lieu: " + ex.Message);
                state.Socket.Close();
            }
        }

        static void ReceivedData(IAsyncResult iar)
        {
            ClientState state = (ClientState)iar.AsyncState;
            try
            {
                int recv = state.Socket.EndReceive(iar);

                if (recv == 0)
                {
                    Console.WriteLine("Server da ngat ket noi");
                    state.Socket.Close();
                    return;
                }

                string receivedData = Encoding.ASCII.GetString(state.Buffer, 0, recv);
                Console.WriteLine("Server tra ve: {0}", receivedData);

                // Tiếp tục nhập và gửi
                PromptAndSend(state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Loi nhan du lieu: " + ex.Message);
                state.Socket.Close();
            }
        }

        static void Main(string[] args)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ClientState state = new ClientState();
            state.Socket = client;

            Console.WriteLine("Dang ket noi den server...");
            client.BeginConnect(iep, new AsyncCallback(Connected), state);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
