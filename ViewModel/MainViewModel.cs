using Client.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Sockets;

namespace Client.ViewModel;
public partial class MainViewModel : ObservableObject
{
    public required Model.Client Client;

    [ObservableProperty]
    public bool isConnected = false;
    [ObservableProperty]
    public bool isNotConnected = true;
    [ObservableProperty]
    private string ip = "127.0.0.1";
    [ObservableProperty]
    private int port = 1024;
    [ObservableProperty]
    private int command;
    
    public ObservableCollection<Message> MessageHistory { get; set; } = [];
    public ObservableCollection<string> Commands { get; set; } = ["GETDATE", "GETTIME"];

    [RelayCommand]
    private async Task Connect()
    {
        this.Client = new Model.Client(Ip, Port);
        try
        {
            this.MessageHistory.Clear();
            this.Client.Connect();
            if (this.Client.IsConnected())
            {
                this.IsConnected = this.Client.IsConnected();
                this.IsNotConnected = !this.IsConnected;
                this.MessageHistory.Add(new Message()
                {
                    Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Content = "Connected!",
                    Sender = "SYSTEM:"
                });
                int bytesRead = 0;
                while (this.Client.IsConnected())
                {
                    bytesRead = await this.Client.Receive();
                    this.MessageHistory.Add(new Message()
                    {
                        Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        Content = System.Text.Encoding.UTF8.GetString(this.Client.Buffer, 0, bytesRead),
                        Sender = "server:"
                    });
                }
            }
            else
            {
                this.MessageHistory.Add(new Message()
                {
                    Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Content = "Server is unreachable!",
                    Sender = "SYSTEM:"
                });
            }
        }
        catch (SocketException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            if (IsConnected)
            {
                this.MessageHistory.Add(new Message()
                {
                    Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Content = "Connection lost!",
                    Sender = "SYSTEM:"
                });
                this.Disconnect();
            }
        }
    }
    [RelayCommand]
    private void Disconnect()
    {
        if (this.IsConnected)
        {
            this.Client.Disconnect();
            this.IsNotConnected = true;
            this.IsConnected = !this.IsNotConnected;
            this.MessageHistory.Add(new Message()
            {
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Content = "Disconnecting...",
                Sender = "SYSTEM:"
            });
        }
    }
    [RelayCommand]
    private void Send()
    {
        this.Client.Send(this.Commands[this.Command]);
        this.MessageHistory.Add(new Message()
        {
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            Sender = "me:",
            Content = this.Commands[this.Command]
        });
    }
}
