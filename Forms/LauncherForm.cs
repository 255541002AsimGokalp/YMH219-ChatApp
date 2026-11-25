namespace YMH219_ChatApp.Forms;

public class LauncherForm : Form
{
    public LauncherForm()
    {
        Text = "YMH 219 - TCP Chat Başlatıcı";
        ClientSize = new Size(380, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var lbl = new Label
        {
            Text = "Test etmek için önce bir Sunucu, ardından bir veya birden fazla İstemci penceresi açabilirsiniz.",
            Location = new Point(24, 20),
            Size = new Size(330, 44),
            Font = new Font("Segoe UI", 9.5F)
        };

        var btnServer = new Button
        {
            Text = "Sunucu Penceresi Aç (Server)",
            Location = new Point(24, 78),
            Size = new Size(330, 42),
            BackColor = Color.FromArgb(30, 41, 59),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnServer.Click += (_, _) => new ServerForm().Show();

        var btnClient = new Button
        {
            Text = "İstemci Penceresi Aç (Client)",
            Location = new Point(24, 132),
            Size = new Size(330, 42),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnClient.Click += (_, _) => new ClientForm().Show();

        Controls.AddRange(new Control[] { lbl, btnServer, btnClient });
    }
}
