using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetScoutClient
{
    public partial class MainDashboard : Form
    {
        private CancellationTokenSource _cts;
        private string _selectedScan = "Ping";

        public MainDashboard()
        {
            InitializeComponent();
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text  = $"Welcome, {UserSession.Username}";
            lblAdmin.Visible = UserSession.IsAdmin;
            SelectScanType("Ping");

            // Right-click context menu on results grid
            var menu = new ContextMenuStrip();
            var itemFullScan = new ToolStripMenuItem("🔍  Full Port Scan (1–1024)");
            var itemCopyIP   = new ToolStripMenuItem("📋  Copy IP / Host");

            itemFullScan.Click += (s, e) =>
            {
                string host = GetSelectedHost();
                if (string.IsNullOrEmpty(host)) return;
                new PortDetailForm(host).Show(this);
            };

            itemCopyIP.Click += (s, e) =>
            {
                string host = GetSelectedHost();
                if (!string.IsNullOrEmpty(host))
                    Clipboard.SetText(host);
            };

            menu.Items.Add(itemFullScan);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemCopyIP);

            dgvResults.ContextMenuStrip = menu;

            // Double-click also opens full port scan
            dgvResults.CellDoubleClick += (s, e) =>
            {
                string host = GetSelectedHost();
                if (!string.IsNullOrEmpty(host))
                    new PortDetailForm(host).Show(this);
            };
        }

        private string GetSelectedHost()
        {
            if (dgvResults.SelectedRows.Count == 0) return null;
            return dgvResults.SelectedRows[0].Cells["colHost"].Value?.ToString();
        }

        private void MainDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cts?.Cancel();
            UserSession.Clear();
            Application.Exit();
        }

        // ── SCAN TYPE SELECTION ─────────────────────────────────────────

        private void SelectScanType(string type)
        {
            _selectedScan = type;
            var map = new Dictionary<string, Button>
            {
                { "Ping", btnPing }, { "Port", btnPort },
                { "OS", btnOS },     { "Aggressive", btnAggressive }
            };
            foreach (var kv in map)
            {
                kv.Value.BackColor = kv.Key == type
                    ? Color.FromArgb(42, 82, 82)
                    : Color.FromArgb(200, 200, 200);
                kv.Value.ForeColor = kv.Key == type ? Color.White : Color.DarkSlateGray;
            }
            SetupColumns(type);
        }

        private void btnPing_Click(object sender, EventArgs e)       => SelectScanType("Ping");
        private void btnPort_Click(object sender, EventArgs e)       => SelectScanType("Port");
        private void btnOS_Click(object sender, EventArgs e)         => SelectScanType("OS");
        private void btnAggressive_Click(object sender, EventArgs e) => SelectScanType("Aggressive");

        private void SetupColumns(string type)
        {
            dgvResults.Columns.Clear();
            switch (type)
            {
                case "Ping":
                    AddCol("colHost",   "IP / Host",      25);
                    AddCol("colStatus", "Status",         15);
                    AddCol("colTime",   "Response (ms)",  20);
                    AddCol("colTTL",    "TTL",            10);
                    break;
                case "Port":
                    AddCol("colHost",    "IP / Host",     25);
                    AddCol("colPort",    "Port",          10);
                    AddCol("colProto",   "Protocol",      10);
                    AddCol("colService", "Service",       20);
                    AddCol("colStatus",  "Status",        15);
                    break;
                case "OS":
                    AddCol("colHost", "IP / Host",        25);
                    AddCol("colTTL",  "TTL",              10);
                    AddCol("colOS",   "Estimated OS",     30);
                    AddCol("colTime", "Response (ms)",    20);
                    break;
                case "Aggressive":
                    AddCol("colHost",   "IP / Host",      20);
                    AddCol("colStatus", "Status",         10);
                    AddCol("colOS",     "OS Estimate",    20);
                    AddCol("colPorts",  "Open Ports",     30);
                    AddCol("colTime",   "Response (ms)",  15);
                    break;
            }
        }

        private void AddCol(string name, string header, int fillWeight)
        {
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name       = name,
                HeaderText = header,
                FillWeight = fillWeight
            });
        }

        // ── START / STOP ────────────────────────────────────────────────

        private async void btnScan_Click(object sender, EventArgs e)
        {
            string target = txtTarget.Text.Trim();
            if (string.IsNullOrEmpty(target))
            {
                MessageBox.Show("Please enter a target IP, hostname, or range.\n\nExamples:\n  192.168.1.1\n  192.168.1.0/24\n  192.168.1.1-254\n  google.com",
                    "No Target", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvResults.Rows.Clear();
            _cts = new CancellationTokenSource();
            SetScanning(true);

            try
            {
                var targets = ParseTargets(target);
                progressBar.Maximum = Math.Max(targets.Count, 1);
                progressBar.Value   = 0;

                switch (_selectedScan)
                {
                    case "Ping":       await RunPingScan(targets, _cts.Token);       break;
                    case "Port":       await RunPortScan(targets, _cts.Token);       break;
                    case "OS":         await RunOSScan(targets, _cts.Token);         break;
                    case "Aggressive": await RunAggressiveScan(targets, _cts.Token); break;
                }

                SetStatus($"✅ Done — {dgvResults.Rows.Count} result(s)", Color.DarkGreen);
            }
            catch (OperationCanceledException)
            {
                SetStatus("⏹ Scan stopped", Color.DarkOrange);
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Error: {ex.Message}", Color.Crimson);
            }
            finally
            {
                SetScanning(false);
            }
        }

        private void btnStop_Click(object sender, EventArgs e) => _cts?.Cancel();

        // ── PING SCAN ───────────────────────────────────────────────────

        private async Task RunPingScan(List<string> targets, CancellationToken ct)
        {
            int done = 0;
            var tasks = targets.Select(async host =>
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using var ping = new Ping();
                    var reply  = await ping.SendPingAsync(host, 1200);
                    bool up    = reply.Status == IPStatus.Success;
                    string ms  = up ? reply.RoundtripTime.ToString() : "-";
                    string ttl = up ? (reply.Options?.Ttl.ToString() ?? "-") : "-";
                    AddRow(new object[] { host, up ? "🟢 Online" : "🔴 Offline", ms, ttl },
                           up ? Color.DarkGreen : Color.Crimson);
                }
                catch { AddRow(new object[] { host, "❌ Error", "-", "-" }, Color.Gray); }
                UpdateProgress(Interlocked.Increment(ref done), targets.Count);
            });
            await Task.WhenAll(tasks);
        }

        // ── PORT SCAN ───────────────────────────────────────────────────

        private async Task RunPortScan(List<string> targets, CancellationToken ct)
        {
            var ports = new Dictionary<int, string>
            {
                { 21, "FTP" }, { 22, "SSH" }, { 23, "Telnet" }, { 25, "SMTP" },
                { 53, "DNS" }, { 80, "HTTP" }, { 110, "POP3" }, { 143, "IMAP" },
                { 443, "HTTPS" }, { 445, "SMB" }, { 3306, "MySQL" },
                { 3389, "RDP" }, { 8080, "HTTP-Alt" }
            };

            progressBar.Maximum = targets.Count * ports.Count;
            int done = 0;

            foreach (var host in targets)
            {
                ct.ThrowIfCancellationRequested();
                var portTasks = ports.Select(async kv =>
                {
                    bool open = await IsPortOpen(host, kv.Key, 800);
                    if (open)
                        AddRow(new object[] { host, kv.Key, "TCP", kv.Value, "🟢 Open" }, Color.DarkGreen);
                    UpdateProgress(Interlocked.Increment(ref done), progressBar.Maximum);
                });
                await Task.WhenAll(portTasks);
            }
        }

        // ── OS DETECTION ────────────────────────────────────────────────

        private async Task RunOSScan(List<string> targets, CancellationToken ct)
        {
            int done = 0;
            var tasks = targets.Select(async host =>
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using var ping = new Ping();
                    var reply = await ping.SendPingAsync(host, 1200);
                    if (reply.Status == IPStatus.Success)
                    {
                        int    ttl  = reply.Options?.Ttl ?? 0;
                        string os   = GuessOS(ttl);
                        string ms   = reply.RoundtripTime.ToString();
                        AddRow(new object[] { host, ttl, os, ms }, Color.DarkSlateGray);
                    }
                    else
                    {
                        AddRow(new object[] { host, "-", "🔴 No Response", "-" }, Color.Gray);
                    }
                }
                catch { AddRow(new object[] { host, "-", "❌ Error", "-" }, Color.Gray); }
                UpdateProgress(Interlocked.Increment(ref done), targets.Count);
            });
            await Task.WhenAll(tasks);
        }

        // ── AGGRESSIVE SCAN ─────────────────────────────────────────────

        private async Task RunAggressiveScan(List<string> targets, CancellationToken ct)
        {
            var topPorts = new Dictionary<int, string>
            {
                { 22, "SSH" }, { 80, "HTTP" }, { 443, "HTTPS" },
                { 3389, "RDP" }, { 3306, "MySQL" }, { 445, "SMB" }, { 8080, "HTTP-Alt" }
            };
            int done = 0;

            foreach (var host in targets)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using var ping = new Ping();
                    var reply  = await ping.SendPingAsync(host, 1200);
                    bool up    = reply.Status == IPStatus.Success;
                    string ms  = up ? reply.RoundtripTime.ToString() : "-";
                    int ttl    = up ? (reply.Options?.Ttl ?? 0) : 0;
                    string os  = up ? GuessOS(ttl) : "-";

                    string openPorts = "-";
                    if (up)
                    {
                        var portTasks = topPorts.Select(async kv =>
                            await IsPortOpen(host, kv.Key, 600) ? $"{kv.Key}({kv.Value})" : null);
                        var found = (await Task.WhenAll(portTasks)).Where(r => r != null);
                        openPorts = string.Join("  ", found);
                        if (string.IsNullOrEmpty(openPorts)) openPorts = "None found";
                    }

                    AddRow(new object[] { host, up ? "🟢 Online" : "🔴 Offline", os, openPorts, ms },
                           up ? Color.DarkSlateGray : Color.Gray);
                }
                catch { AddRow(new object[] { host, "❌ Error", "-", "-", "-" }, Color.Gray); }
                UpdateProgress(Interlocked.Increment(ref done), targets.Count);
            }
        }

        // ── HELPERS ─────────────────────────────────────────────────────

        private static string GuessOS(int ttl) => ttl switch
        {
            >= 128 => "Windows",
            >= 64  => "Linux / macOS",
            >= 32  => "Network Device",
            _      => "Unknown"
        };

        private async Task<bool> IsPortOpen(string host, int port, int timeoutMs)
        {
            try
            {
                using var client = new TcpClient();
                var cts = new CancellationTokenSource(timeoutMs);
                await client.ConnectAsync(host, port).WaitAsync(cts.Token);
                return true;
            }
            catch { return false; }
        }

        private List<string> ParseTargets(string input)
        {
            var list = new List<string>();
            input = input.Trim();

            // CIDR: 192.168.1.0/24
            if (input.Contains("/"))
            {
                var p = input.Split('/');
                if (p.Length == 2 && int.TryParse(p[1], out int prefix) && prefix >= 0 && prefix <= 32)
                {
                    var parts = p[0].Split('.');
                    if (parts.Length == 4)
                    {
                        for (int i = 1; i <= 254; i++)
                            list.Add($"{parts[0]}.{parts[1]}.{parts[2]}.{i}");
                        return list;
                    }
                }
            }

            // Range: 192.168.1.1-254
            if (input.Contains("-"))
            {
                var octets = input.Split('.');
                if (octets.Length == 4 && octets[3].Contains("-"))
                {
                    var range = octets[3].Split('-');
                    if (int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        string prefix = $"{octets[0]}.{octets[1]}.{octets[2]}";
                        for (int i = start; i <= end; i++)
                            list.Add($"{prefix}.{i}");
                        return list;
                    }
                }
            }

            // Single host or IP
            list.Add(input);
            return list;
        }

        private void AddRow(object[] values, Color foreColor)
        {
            if (dgvResults.InvokeRequired) { dgvResults.Invoke(() => AddRow(values, foreColor)); return; }
            int idx = dgvResults.Rows.Add(values);
            dgvResults.Rows[idx].DefaultCellStyle.ForeColor = foreColor;
        }

        private void UpdateProgress(int done, int total)
        {
            if (progressBar.InvokeRequired) { progressBar.Invoke(() => UpdateProgress(done, total)); return; }
            if (done <= progressBar.Maximum) progressBar.Value = done;
            SetStatus($"⏳ Scanning... {done}/{total}", Color.DarkSlateGray);
        }

        private void SetScanning(bool scanning)
        {
            if (InvokeRequired) { Invoke(() => SetScanning(scanning)); return; }
            btnScan.Enabled      = !scanning;
            btnStop.Enabled      = scanning;
            btnPing.Enabled      = !scanning;
            btnPort.Enabled      = !scanning;
            btnOS.Enabled        = !scanning;
            btnAggressive.Enabled = !scanning;
            if (scanning) SetStatus("⏳ Starting scan...", Color.DarkSlateGray);
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(() => SetStatus(text, color)); return; }
            lblStatus.Text      = text;
            lblStatus.ForeColor = color;
        }
    }
}
