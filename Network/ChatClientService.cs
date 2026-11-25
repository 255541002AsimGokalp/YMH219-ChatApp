using System.Net.Sockets;
using System.Text;

namespace YMH219_ChatApp.Network;

public class ChatClientService
{
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private CancellationTokenSource? _cts;

    public bool IsConnected => _client?.Connected == true;

    public event Action<string>? OnMessageReceived;
    public event Action<string>? OnDisconnected;

    public async Task ConnectAsync(string host, int port, string username)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port);

        var stream = _client.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8);
        _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        await _writer.WriteLineAsync(username);

        _cts = new CancellationTokenSource();
        _ = ListenLoopAsync(_cts.Token);
    }

    private async Task ListenLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && _reader != null)
            {
                string? line = await _reader.ReadLineAsync(token);
                if (line == null) break;
                OnMessageReceived?.Invoke(line);
            }
        }
        catch (Exception ex) when (ex is IOException || ex is OperationCanceledException || ex is ObjectDisposedException)
        {
            // Bağlantı kapandı
        }
        finally
        {
            Disconnect("Sunucu bağlantısı sonlandı.");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (!IsConnected || _writer == null)
            throw new InvalidOperationException("Sunucuya bağlı değilsiniz.");

        await _writer.WriteLineAsync(message);
    }

    public void Disconnect(string reason = "Bağlantı kesildi.")
    {
        if (_client == null) return;

        _cts?.Cancel();
        _reader?.Dispose();
        _writer?.Dispose();
        _client?.Close();
        _client = null;
        OnDisconnected?.Invoke(reason);
    }
}
