using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetScoutClient
{
    public partial class PortDetailForm : Form
    {
        private readonly string _host;
        private CancellationTokenSource _cts;

        // Well-known port names
        private static readonly Dictionary<int, string> KnownPorts = new()
        {
            {20,"FTP-Data"},{21,"FTP"},{22,"SSH"},{23,"Telnet"},{25,"SMTP"},
            {53,"DNS"},{67,"DHCP"},{68,"DHCP"},{69,"TFTP"},{80,"HTTP"},
            {110,"POP3"},{119,"NNTP"},{123,"NTP"},{135,"RPC"},{137,"NetBIOS"},
            {138,"NetBIOS"},{139,"NetBIOS"},{143,"IMAP"},{161,"SNMP"},
            {194,"IRC"},{443,"HTTPS"},{445,"SMB"},{465,"SMTPS"},{514,"Syslog"},
            {587,"SMTP-TLS"},{631,"IPP"},{993,"IMAPS"},{995,"POP3S"},
            {1433,"MSSQL"},{1521,"Oracle"},{1723,"PPTP"},{3306,"MySQL"},
            {3389,"RDP"},{5432,"PostgreSQL"},{5900,"VNC"},{6379,"Redis"},
            {8080,"HTTP-Alt"},{8443,"HTTPS-Alt"},{8888,"HTTP-Dev"},
            {9200,"Elasticsearch"},{27017,"MongoDB"}
        };

        public PortDetailForm(string host)
        {
            _host = host;
            InitializeComponent();
            this.Text = $"Port Scan — {host}";
            lblHost.Text = $"🔍 Full Port Scan:  {host}";
        }

        private async void PortDetailForm_Load(object sender, EventArgs e)
        {
            await StartScan();
        }

        private async Task StartScan()
        {
            dgvPorts.Rows.Clear();
            _cts = new CancellationTokenSource();
            btnStop.Enabled  = true;
            btnRescan.Enabled = false;
            progressBar.Value = 0;
            progressBar.Maximum = 1024;
            lblStatus.Text = "⏳ Scanning ports 1–1024...";
            lblStatus.ForeColor = Color.DarkSlateGray;

            int done = 0;
            int found = 0;

            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(100); // max 100 concurrent

            for (int port = 1; port <= 1024; port++)
            {
                if (_cts.Token.IsCancellationRequested) break;
                int p = port;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync(_cts.Token);
                    try
                    {
                        bool open = await IsPortOpen(_host, p, 600, _cts.Token);
                        if (open)
                        {
                            string service = KnownPorts.TryGetValue(p, out var s) ? s : "Unknown";
                            Interlocked.Increment(ref found);
                            AddRow(p, "TCP", service);
                        }
                    }
                    catch { }
                    finally { semaphore.Release(); }

                    int d = Interlocked.Increment(ref done);
                    UpdateProgress(d, found);
                }, _cts.Token));
            }

            try
            {
                await Task.WhenAll(tasks);
                int total = dgvPorts.Rows.Count;
                SetStatus(total == 0
                    ? "✅ Scan complete — No open ports found"
                    : $"✅ Scan complete — {total} open port(s) found",
                    total == 0 ? Color.DimGray : Color.DarkGreen);
            }
            catch (OperationCanceledException)
            {
                SetStatus("⏹ Scan stopped", Color.DarkOrange);
            }
            finally
            {
                btnStop.Enabled  = false;
                btnRescan.Enabled = true;
            }
        }

        private async Task<bool> IsPortOpen(string host, int port, int timeout, CancellationToken ct)
        {
            try
            {
                using var client = new TcpClient();
                var cts2 = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts2.CancelAfter(timeout);
                await client.ConnectAsync(host, port).WaitAsync(cts2.Token);
                return true;
            }
            catch { return false; }
        }

        private void AddRow(int port, string proto, string service)
        {
            if (dgvPorts.InvokeRequired) { dgvPorts.Invoke(() => AddRow(port, proto, service)); return; }
            int idx = dgvPorts.Rows.Add(port, proto, service, "🟢 Open");
            dgvPorts.Rows[idx].DefaultCellStyle.ForeColor = Color.DarkGreen;
        }

        private void UpdateProgress(int done, int found)
        {
            if (progressBar.InvokeRequired) { progressBar.Invoke(() => UpdateProgress(done, found)); return; }
            if (done <= progressBar.Maximum) progressBar.Value = done;
            lblStatus.Text = $"⏳ Scanning... {done}/1024  —  {found} open";
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(() => SetStatus(text, color)); return; }
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void btnStop_Click(object sender, EventArgs e)   => _cts?.Cancel();
        private async void btnRescan_Click(object sender, EventArgs e) => await StartScan();
        private void btnClose_Click(object sender, EventArgs e)  => this.Close();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Cancel();
            base.OnFormClosing(e);
        }
    }
}
