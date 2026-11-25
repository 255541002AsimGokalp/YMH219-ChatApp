using System.Net;
using System.Net.Sockets;
using System.Text;
using YMH219_ChatApp.Models;

namespace YMH219_ChatApp.Network;

public class ChatServerService
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private readonly List<ClientSession> _clients = new();
    private readonly object _lock = new();

    public bool IsRunning { get; private set; }

    public event Action<string>? OnLogMessage;
    public event Action<List<string>>? OnClientListChanged;

    public async Task StartAsync(int port)
    {
        if (IsRunning) return;

        _cts = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        IsRunning = true;
        Log($"Sunucu {port} portunda başlatıldı.");

        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token);
                _ = HandleClientAsync(tcpClient, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal durdurma
        }
        catch (Exception ex)
        {
            Log($"Sunucu hatası: {ex.Message}");
        }
        finally
        {
            Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
    {
        ClientSession? session = null;
        try
        {
            using var stream = tcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            string? usernameLine = await reader.ReadLineAsync(token);
            if (string.IsNullOrWhiteSpace(usernameLine))
                return;

            session = new ClientSession(tcpClient, writer) { Username = usernameLine.Trim() };
            lock (_lock)
            {
                _clients.Add(session);
            }

            NotifyClientList();
            string joinMsg = $"[{DateTime.Now:HH:mm:ss}] * {session.Username} sohbete katıldı.";
            Log(joinMsg);
            await BroadcastAsync(joinMsg);

            while (!token.IsCancellationRequested)
            {
                string? line = await reader.ReadLineAsync(token);
                if (line == null) break;

                string formatted = $"[{DateTime.Now:HH:mm:ss}] {session.Username}: {line}";
                Log(formatted);
                await BroadcastAsync(formatted);
            }
        }
        catch (Exception ex) when (ex is IOException || ex is OperationCanceledException || ex is ObjectDisposedException)
        {
            // İstemci bağlantıyı kapattı
        }
        finally
        {
            if (session != null)
            {
                lock (_lock)
                {
                    _clients.Remove(session);
                }
                NotifyClientList();
                string leaveMsg = $"[{DateTime.Now:HH:mm:ss}] * {session.Username} ayrıldı.";
                Log(leaveMsg);
                await BroadcastAsync(leaveMsg);
            }
            tcpClient.Close();
        }
    }

    public async Task BroadcastAsync(string message)
    {
        List<ClientSession> snapshot;
        lock (_lock)
        {
            snapshot = _clients.ToList();
        }

        foreach (var client in snapshot)
        {
            try
            {
                await client.Writer.WriteLineAsync(message);
            }
            catch
            {
                // Kopan istemci HandleClientAsync içinde temizlenir.
            }
        }
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        _cts?.Cancel();
        _listener?.Stop();

        lock (_lock)
        {
            foreach (var c in _clients)
                c.TcpClient.Close();
            _clients.Clear();
        }

        NotifyClientList();
        Log("Sunucu durduruldu.");
    }

    private void NotifyClientList()
    {
        List<string> list;
        lock (_lock)
        {
            list = _clients.Select(c => c.ToString()).ToList();
        }
        OnClientListChanged?.Invoke(list);
    }

    private void Log(string message) => OnLogMessage?.Invoke(message);
}
