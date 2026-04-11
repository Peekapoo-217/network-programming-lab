using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AsyncClientApp
{
    public partial class Form1 : Form
    {
        private Socket clientSocket;
        private byte[] buffer = new byte[1024];

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(txtHost.Text);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));

                btnConnect.Enabled = false;
                LogMessage("Connecting to server...");

                // Begin asynchronous connect
                clientSocket.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection initialization error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnect.Enabled = true;
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndConnect(AR);
                LogMessage("Connected to server.");

                this.Invoke(new Action(() => {
                    btnConnect.Enabled = false;
                }));

                // Start receiving data
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception ex)
            {
                LogMessage("Connection Error: " + ex.Message);
                this.Invoke(new Action(() => {
                    btnConnect.Enabled = true;
                }));
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
                        LogMessage("Server: " + text);

                        // Continue receiving
                        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                    }
                    else
                    {
                        LogMessage("Server disconnected.");
                        clientSocket.Close();
                        this.Invoke(new Action(() => {
                            btnConnect.Enabled = true;
                        }));
                    }
                }
            }
            catch (SocketException)
            {
                LogMessage("Server disconnected ungracefully.");
                clientSocket?.Close();
                this.Invoke(new Action(() => {
                    btnConnect.Enabled = true;
                }));
            }
            catch (ObjectDisposedException)
            {
                // Socket was closed
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
                    
                    LogMessage("Client: " + message);
                    txtMessage.Clear();
                }
                catch (Exception ex)
                {
                    LogMessage("Send Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Not connected to server.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }
    }
}
