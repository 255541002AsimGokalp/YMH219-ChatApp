using YMH219_ChatApp.Network;

namespace YMH219_ChatApp.Forms;

public class ClientForm : Form
{
    private readonly ChatClientService _client = new();
    private readonly TextBox _txtHost = new() { Text = "127.0.0.1", Location = new Point(45, 14), Width = 100 };
    private readonly TextBox _txtPort = new() { Text = "5000", Location = new Point(190, 14), Width = 60 };
    private readonly TextBox _txtUser = new() { Text = "Asim", Location = new Point(335, 14), Width = 110 };
    private readonly Button _btnConnect = new() { Text = "Bağlan", Location = new Point(458, 11), Size = new Size(85, 30), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    private readonly Button _btnDisconnect = new() { Text = "Kes", Location = new Point(550, 11), Size = new Size(75, 30), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
    private readonly ListBox _lstMessages = new() { Location = new Point(14, 54), Size = new Size(611, 290), Font = new Font("Segoe UI", 9.5F) };
    private readonly TextBox _txtMessage = new() { Location = new Point(14, 355), Size = new Size(495, 28), Enabled = false, PlaceholderText = "Mesajınızı yazın..." };
    private readonly Button _btnSend = new() { Text = "Mesaj Gönder", Location = new Point(518, 353), Size = new Size(107, 30), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };

    public ClientForm()
    {
        Text = "YMH 219 - Chat İstemcisi (TCP Client)";
        ClientSize = new Size(640, 400);
        StartPosition = FormStartPosition.CenterScreen;

        var lblIp = new Label { Text = "IP:", Location = new Point(14, 17), AutoSize = true };
        var lblPort = new Label { Text = "Port:", Location = new Point(152, 17), AutoSize = true };
        var lblUser = new Label { Text = "Kullanıcı Adı:", Location = new Point(256, 17), AutoSize = true };

        _btnConnect.Click += BtnConnect_Click;
        _btnDisconnect.Click += (_, _) => _client.Disconnect("Kullanıcı bağlantıyı kesti.");
        _btnSend.Click += BtnSend_Click;
        _txtMessage.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnSend_Click(s, e);
            }
        };

        _client.OnMessageReceived += msg => SafeInvoke(() =>
        {
            _lstMessages.Items.Add(msg);
            _lstMessages.TopIndex = _lstMessages.Items.Count - 1;
        });

        _client.OnDisconnected += reason => SafeInvoke(() =>
        {
            _lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] Sistem: {reason}");
            SetConnectedUi(false);
        });

        FormClosing += (_, _) => _client.Disconnect();

        Controls.AddRange(new Control[]
        {
            lblIp, _txtHost, lblPort, _txtPort, lblUser, _txtUser,
            _btnConnect, _btnDisconnect, _lstMessages, _txtMessage, _btnSend
        });
    }

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtUser.Text))
        {
            MessageBox.Show("Lütfen bir kullanıcı adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(_txtPort.Text, out int port))
        {
            MessageBox.Show("Geçerli bir port numarası girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _btnConnect.Enabled = false;
            await _client.ConnectAsync(_txtHost.Text.Trim(), port, _txtUser.Text.Trim());
            SetConnectedUi(true);
            _txtMessage.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Sunucuya bağlanılamadı: {ex.Message}", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetConnectedUi(false);
        }
    }

    private async void BtnSend_Click(object? sender, EventArgs e)
    {
        string text = _txtMessage.Text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        try
        {
            await _client.SendMessageAsync(text);
            _txtMessage.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mesaj gönderilemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetConnectedUi(bool connected)
    {
        _btnConnect.Enabled = !connected;
        _txtHost.Enabled = !connected;
        _txtPort.Enabled = !connected;
        _txtUser.Enabled = !connected;
        _btnDisconnect.Enabled = connected;
        _txtMessage.Enabled = connected;
        _btnSend.Enabled = connected;
    }

    private void SafeInvoke(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) Invoke(action);
        else action();
    }
}
