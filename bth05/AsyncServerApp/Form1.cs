using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncServerApp
{
    public partial class Form1 : Form
    {
        private Socket serverSocket;
        private Socket clientSocket;
        private byte[] buffer = new byte[1024];

        public Form1()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(txtHost.Text);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));

                serverSocket.Bind(endPoint);
                serverSocket.Listen(5);

                btnListen.Enabled = false;
                LogMessage("Server started. Listening on " + endPoint.ToString());

                // Begin accepting connection
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket = serverSocket.EndAccept(AR);
                LogMessage("Client connected: " + clientSocket.RemoteEndPoint.ToString());

                // Start receiving data
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (ObjectDisposedException)
            {
                // Socket was closed
            }
            catch (Exception ex)
            {
                LogMessage("Accept Error: " + ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    int received = clientSocket.EndReceive(AR);

                    if (received > 0)
                    {
                        string text = Encoding.UTF8.GetString(buffer, 0, received);
                        LogMessage("Client: " + text);

                        // Continue receiving
                        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                    }
                    else
                    {
                        LogMessage("Client disconnected.");
                        clientSocket.Close();
                    }
                }
            }
            catch (SocketException)
            {
                LogMessage("Client disconnected ungracefully.");
                clientSocket?.Close();
            }
            catch (ObjectDisposedException)
            {
                // Socket closed
            }
            catch (Exception ex)
            {
                LogMessage("Receive Error: " + ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    string message = txtMessage.Text;
                    if (string.IsNullOrWhiteSpace(message)) return;

                    byte[] data = Encoding.UTF8.GetBytes(message);
                    clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);

                    LogMessage("Server: " + message);
                    txtMessage.Clear();
                }
                catch (Exception ex)
                {
                    LogMessage("Send Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No client connected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.EndSend(AR);
                }
            }
            catch (Exception ex)
            {
                LogMessage("SendCallback Error: " + ex.Message);
            }
        }

        private void LogMessage(string message)
        {
            if (lstMessages.InvokeRequired)
            {
                lstMessages.Invoke(new Action(() => lstMessages.Items.Add(message)));
            }
            else
            {
                lstMessages.Items.Add(message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Close();
            }
            if (serverSocket != null)
            {
                serverSocket.Close();
            }
        }

        private void lstMessages_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
