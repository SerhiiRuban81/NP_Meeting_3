using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NP_Meeting_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Task? listenerTask;
        Socket? serverSocket;
        string addressStr;

        int port;
        private void Form1_Load(object sender, EventArgs e)
        {
            addressStr = "192.168.0.109";
            port = 11000;
            textBox1.Text = addressStr;
            textBox2.Text = port.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listenerTask != null)
                return;
            listenerTask = Task.Factory.StartNew(() =>
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                    ProtocolType.IP);
                IPAddress serverId = IPAddress.Parse(textBox1.Text);
                port = int.Parse(textBox2.Text);
                IPEndPoint serverEP = new IPEndPoint(serverId, port);
                try
                {
                    serverSocket.Bind(serverEP);
                    this.BeginInvoke(new MethodInvoker(() => {
                        textBox1.Enabled = false;
                        textBox2.Enabled = false;
                    }));
                    while (true)
                    {
                        EndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                        byte[] buff = new byte[1024];
                        int len = serverSocket.ReceiveFrom(buff, ref remoteEP);
                        StringBuilder sb = new StringBuilder();
                        sb.Append(textBox3.Text);
                        if (remoteEP is IPEndPoint remoteIPEndoint)
                        {
                            sb.AppendLine($"{DateTime.Now.ToLongTimeString()}. " +
                                $"Отримано повідомлення від клієнта {remoteIPEndoint}");
                        }
                        string message = Encoding.UTF8.GetString(buff, 0, len);
                        sb.AppendLine(message);
                        //Оновлення стрічки повідомлень
                        textBox3.BeginInvoke(new MethodInvoker(() => {
                            textBox3.Text = sb.ToString();
                        }));
                        //label1.BeginInvoke(new MethodInvoker(() => {
                        //    label1.Text = message;
                        //}));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    serverSocket?.Close();
                    serverSocket = null;
                    listenerTask = null;
                    this.BeginInvoke(new MethodInvoker(() => {
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                    }));                  
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                ProtocolType.IP);
            byte[] buff = Encoding.UTF8.GetBytes(textBox4.Text);
            IPAddress serverAddr = IPAddress.Parse(textBox6.Text);
            // Broadcast-розсилання повідомлень
            //IPAddress serverAddr = IPAddress.Parse("192.168.0.255");
            int p = int.Parse(textBox5.Text);
            client.SendTo(buff, new IPEndPoint(serverAddr, port: p));
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
