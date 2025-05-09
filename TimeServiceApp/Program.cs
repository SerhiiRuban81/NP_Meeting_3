// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;


Console.WriteLine("Hello, World!");
Socket socket  = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
    ProtocolType.IP);
Timer timer = new Timer(SendCurrentTime, socket, 0, 3000);
Console.WriteLine("Press Enter to finish");
Console.ReadLine();


void SendCurrentTime(object? obj)
{
    if(obj is Socket socket)
    {
        IPAddress clientAddress = IPAddress.Parse("192.168.0.255");
        int port = 11000;
        IPEndPoint remoteEP = new IPEndPoint(clientAddress, port);
        byte[] buff = Encoding.UTF8.GetBytes($"Поточний час: {DateTime.Now.ToLongTimeString()}") ;
        socket.SendTo(buff, remoteEP);
    }
}