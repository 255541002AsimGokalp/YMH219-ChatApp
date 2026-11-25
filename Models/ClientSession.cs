using System.Net.Sockets;

namespace YMH219_ChatApp.Models;

public class ClientSession
{
    public string Username { get; set; } = "Anonim";
    public string EndPoint { get; }
    public DateTime ConnectedAt { get; }
    public TcpClient TcpClient { get; }
    public StreamWriter Writer { get; }

    public ClientSession(TcpClient client, StreamWriter writer)
    {
        TcpClient = client;
        Writer = writer;
        EndPoint = client.Client.RemoteEndPoint?.ToString() ?? "Bilinmiyor";
        ConnectedAt = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Username} ({EndPoint}) - {ConnectedAt:HH:mm:ss}";
    }
}
