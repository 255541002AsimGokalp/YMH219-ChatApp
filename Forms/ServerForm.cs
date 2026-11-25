using YMH219_ChatApp.Network;

namespace YMH219_ChatApp.Forms;

public class ServerForm : Form
{
    private readonly ChatServerService _server = new();
    private readonly TextBox _txtPort = new() { Text = "5000", Location = new Point(62, 14), Width = 80 };
    private readonly Button _btnStart = new() { Text = "Sunucuyu Başlat", Location = new Point(155, 11), Size = new Size(135, 30), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    private readonly Button _btnStop = new() { Text = "Durdur", Location = new Point(300, 11), Size = new Size(90, 30), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
    private readonly ListBox _lstClients = new() { Location = new Point(14, 75), Size = new Size(250, 310) };
    private readonly ListBox _lstLogs = new() { Location = new Point(276, 75), Size = new Size(390, 310) };

    public ServerForm()
    {
        Text = "YMH 219 - Chat Sunucusu (TCP Server)";
        ClientSize = new Size(680, 405);
        StartPosition = FormStartPosition.CenterScreen;

        var lblPort = new Label { Text = "Port:", Location = new Point(14, 17), AutoSize = true };
        var lblClients = new Label { Text = "Bağlı İstemciler:", Location = new Point(14, 52), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        var lblLogs = new Label { Text = "Sunucu Mesaj & Olay Günlüğü:", Location = new Point(276, 52), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

        _btnStart.Click += BtnStart_Click;
        _btnStop.Click += (_, _) => StopServer();

        _server.OnLogMessage += msg => SafeInvoke(() =>
        {
            _lstLogs.Items.Add(msg);
            _lstLogs.TopIndex = _lstLogs.Items.Count - 1;
        });

        _server.OnClientListChanged += list => SafeInvoke(() =>
        {
            _lstClients.Items.Clear();
            foreach (var item in list)
                _lstClients.Items.Add(item);
        });

        FormClosing += (_, _) => _server.Stop();

        Controls.AddRange(new Control[] { lblPort, _txtPort, _btnStart, _btnStop, lblClients, _lstClients, lblLogs, _lstLogs });
    }

    private async void BtnStart_Click(object? sender, EventArgs e)
    {
        if (!int.TryParse(_txtPort.Text, out int port) || port < 1024 || port > 65535)
        {
            MessageBox.Show("Lütfen 1024 ile 65535 arasında geçerli bir port numarası girin.", "Geçersiz Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _btnStart.Enabled = false;
        _btnStop.Enabled = true;
        _txtPort.Enabled = false;

        try
        {
            await _server.StartAsync(port);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Sunucu başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            StopServer();
        }
    }

    private void StopServer()
    {
        _server.Stop();
        _btnStart.Enabled = true;
        _btnStop.Enabled = false;
        _txtPort.Enabled = true;
    }

    private void SafeInvoke(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) Invoke(action);
        else action();
    }
}
