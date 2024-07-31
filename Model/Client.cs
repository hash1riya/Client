using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Client.Model;
public class Client
{
    IPEndPoint endp;
    Socket socket;
    public byte[] Buffer = new byte[1024];

    public Client(string ip, int port)
    {
        this.endp = new IPEndPoint(IPAddress.Parse(ip), port);
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
    }

    public bool IsConnected() => this.socket.Connected;
    public void Connect()
    {
        try
        {
            socket.Connect(endp);
        }
        catch (SocketException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public void Disconnect()
    {
        try
        {
            this.socket.Close();
        }
        catch (SocketException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    public async Task<int> Receive() => await this.socket.ReceiveAsync(this.Buffer);
    public async void Send(string strSend)
    {
        if (this.socket != null)
            await this.socket.SendAsync(System.Text.Encoding.UTF8.GetBytes(strSend));
    }
}
