using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetScoutServer
{
    public class ServerCore
    {
        private Socket _listener;
        private readonly DatabaseHelper _db;
        private bool _running;

        private readonly List<ConnectedUser> _sessionLogins = new();
        private readonly object _lock = new();

        public event Action<string> OnLog;
        public event Action<ConnectedUser> OnUserLoggedIn;
        public event Action<string> OnPasswordResetRequest;  // email

        public ServerCore(DatabaseHelper db) => _db = db;

        public void Start()
        {
            new Thread(RunServer) { IsBackground = true, Name = "TCPServer" }.Start();
        }

        public void Stop()
        {
            _running = false;
            try { _listener?.Close(); } catch { }
        }

        public List<ConnectedUser> GetSessionLogins()
        {
            lock (_lock) return new List<ConnectedUser>(_sessionLogins);
        }

        private void RunServer()
        {
            try
            {
                _running = true;
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, 11111));
                _listener.Listen(10);
                Log("✅ Server started on 0.0.0.0:11111 (accepting all connections)");

                while (_running)
                {
                    try
                    {
                        var client = _listener.Accept();
                        Log($"🔌 New connection from {client.RemoteEndPoint}");
                        new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
                    }
                    catch (SocketException) when (!_running) { break; }
                }
            }
            catch (Exception ex) when (_running)
            {
                Log($"❌ Server error: {ex.Message}");
            }
        }

        private void HandleClient(Socket sock)
        {
            try
            {
                byte[] buf = new byte[4096];
                int n = sock.Receive(buf);
                string data = Encoding.UTF8.GetString(buf, 0, n).Trim();
                string[] parts = data.Split('|');

                // Log safely — hide passwords from admin log
                string safeLog = parts[0] switch
                {
                    "LOGIN"    => $"LOGIN|{parts[1]}|[HASH HIDDEN]",
                    "REGISTER" => $"REGISTER|{parts[1]}|[HASH HIDDEN]|{(parts.Length > 3 ? parts[3] : "")}",
                    _          => data
                };
                Log($"📨 {safeLog}");

                string response = parts[0] switch
                {
                    "LOGIN"           => HandleLogin(parts),
                    "REGISTER"        => HandleRegister(parts),
                    "FORGOT_PASSWORD" => HandleForgotPassword(parts),
                    _                 => "ERROR|Unknown command"
                };

                sock.Send(Encoding.UTF8.GetBytes(response));
                sock.Close();
                Log($"📤 {response}");
            }
            catch (Exception ex) { Log($"❌ Client error: {ex.Message}"); }
        }

        private string HandleLogin(string[] parts)
        {
            if (parts.Length < 3) return "ERROR|Invalid format";

            string userData = _db.ValidateUserWithAdmin(parts[1], parts[2]);
            if (userData == null)
            {
                Log($"❌ Login failed: {parts[1]}");
                return "ERROR|Invalid email or password";
            }

            var d = userData.Split('|');
            var user = new ConnectedUser
            {
                UserId    = int.Parse(d[0]),
                Username  = d[1],
                IsAdmin   = d[2] == "1",
                Email     = parts[1],
                LoginTime = DateTime.Now
            };

            lock (_lock) _sessionLogins.Add(user);
            OnUserLoggedIn?.Invoke(user);
            Log($"✅ Login: {parts[1]} [{(user.IsAdmin ? "ADMIN" : "USER")}]");
            return $"SUCCESS|{userData}";
        }

        private string HandleRegister(string[] parts)
        {
            if (parts.Length < 4) return "ERROR|Invalid format";

            if (_db.EmailExists(parts[1]))
            {
                Log($"❌ Register duplicate: {parts[1]}");
                return "ERROR|Email already registered";
            }

            bool ok = _db.RegisterUser(parts[1], parts[2], parts[3]);
            Log(ok ? $"✅ Registered: {parts[1]}" : $"❌ Registration failed");
            return ok ? "SUCCESS|Registration complete" : "ERROR|Registration failed";
        }

        private string HandleForgotPassword(string[] parts)
        {
            if (parts.Length < 2) return "ERROR|Invalid format";

            string email = parts[1].Trim();
            if (!_db.EmailExists(email))
            {
                Log($"❌ Forgot password: email not found ({email})");
                return "ERROR|No account found with that email address";
            }

            OnPasswordResetRequest?.Invoke(email);
            Log($"🔔 Password reset requested for: {email}");
            return "SUCCESS|Request sent to admin";
        }

        private void Log(string msg) => OnLog?.Invoke(msg);
    }
}
