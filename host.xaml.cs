using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentReference = Google.Cloud.Firestore.DocumentReference;
using AuraWave.WinApi.Olaf;

namespace VlcSync
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class host : Window
    {
        const string CHAR_OK = "✔";
        const string CHAR_ERROR = "❌";
        const string COLOR_OK = "#00f500";
        const string COLOR_ERROR = "#f51800";
        const string PROJECT_ID = "vlcsync-af4e8";

        FirestoreDb db;


        settingsBinding binding = new settingsBinding()
        {
            isHost = true,
            MyUUID = Guid.NewGuid().ToString(),
            isNotHosted = true,
            isHosted = false,
        };
        public host()
        {
            InitializeComponent();
            DataContext = binding;
            binding.ServerStatus = "Connected";
            binding.ServerStatusColor = "#00f500";

            NetworkStatus.AvailabilityChanged += NetworkStatus_AvailabilityChanged;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"service-acc-key.json"));
            db = FirestoreDb.Create(PROJECT_ID);
            try
            {
                ConnectionHelpers.PingOut @out = new ConnectionHelpers.PingOut();
                @out = ConnectionHelpers.PingHost("google.com", out _);
                binding.ServerStatus = NetworkStatus.IsAvailable && @out.isAlive ? "Connected" : "Offline";
                binding.ServerStatusColor = NetworkStatus.IsAvailable && @out.isAlive ? COLOR_OK : COLOR_ERROR;
            }
            catch
            {
                //
            }
        }

        private void NetworkStatus_AvailabilityChanged(object sender, NetworkStatusChangedArgs e)
        {
            ConnectionHelpers.PingOut @out = new ConnectionHelpers.PingOut();
            if (e.IsAvailable)
            {
                @out = ConnectionHelpers.PingHost("google.com", out _);
            }
            binding.ServerStatus = NetworkStatus.IsAvailable && @out.isAlive ? "Connected" : "Offline";
            binding.ServerStatusColor = e.IsAvailable && @out.isAlive ? COLOR_OK : COLOR_ERROR;
        }

        private void btnnewuuid_Click(object sender, RoutedEventArgs e)
        {
            binding.UUID = Guid.NewGuid().ToString();
        }

        private void btnbrowsmedia_Click(object sender, RoutedEventArgs e)
        {
            binding.MediaPathOkChar = "";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".mp4";
            dlg.Filter = "MP4 Files (*.mp4)|*.mp4|MKV Files (*.mkv)|*.mkv|All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                binding.MediaPath = dlg.FileName;

                if (System.IO.File.Exists(binding.MediaPath))
                {
                    binding.MediaPathOkColor = COLOR_OK;
                    binding.MediaPathOkChar = CHAR_OK;
                }
            }
        }

        private void btnsubtitle_Click(object sender, RoutedEventArgs e)
        {
            binding.MediaSubtitlePathOkChar = "";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".srt";
            dlg.Filter = "SRT Files (*.srt)|*.srt|All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                binding.MediaSubtitlePath = dlg.FileName;
                if (System.IO.File.Exists(binding.MediaSubtitlePath))
                {
                    binding.MediaSubtitlePathOkColor = COLOR_OK;
                    binding.MediaSubtitlePathOkChar = CHAR_OK;
                }
            }
        }


        async void setHost()
        {
            try
            {
                binding.isLoading = true;
                DocumentReference docRef = db.Collection("users").Document(binding.MyUUID);
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                    { "uuid", Guid.NewGuid().ToString() },
                    { "name", binding.HostName },
                    { "isHost", binding.isHost },
                    { "mediapath",System.IO.Path.GetFileName(binding.MediaPath) },
                    { "mediasubtitle",System.IO.Path.GetFileName(binding.MediaSubtitlePath) },
                    { "password", txtpassword.Password },
                   { "IsPlaying",false },  { "started",false },
                    { "playtime",0 },
                };
                string json = JsonConvert.SerializeObject(user, Formatting.Indented);
                Console.WriteLine(json);
                await docRef.SetAsync(user);
                binding.isNotHosted = false;
                binding.isHosted = true;
                binding.isLoading = false;
                btnStartPlayback.IsEnabled = true;

            }
            catch (Exception ex)
            {
                binding.isNotHosted = true;
                binding.isHosted = false;
                binding.isLoading = false;
                btnStartHosting.IsEnabled = true;

                MessageBox.Show(ex.Message);
            }
        }
        long playtime = 0;
        async void startPlayback()
        {
            try
            {
                binding.isLoading = true;
                btnStartPlayback.IsEnabled = false;
                DocumentReference docRef = db.Collection("users").Document(binding.MyUUID);
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                    { "uuid", Guid.NewGuid().ToString() },
                    { "name", binding.HostName },
                    { "isHost", binding.isHost },
                    { "mediapath",System.IO.Path.GetFileName(binding.MediaPath) },
                    { "mediasubtitle",System.IO.Path.GetFileName(binding.MediaSubtitlePath) },
                    { "password", txtpassword.Password },
                   { "IsPlaying",true },  { "started",true },
                    { "playtime",0 },
                };
                string json = JsonConvert.SerializeObject(user, Formatting.Indented);
                Console.WriteLine(json);

                await docRef.SetAsync(user);
                binding.isLoading = false;
                //this.WindowState = WindowState.Minimized;
                new MainWindow(binding, txtpassword.Password, playtime).ShowDialog();
                binding.isNotHosted = true;
                binding.isHosted = false;
                binding.isLoading = false;
                btnStartHosting.IsEnabled = true;

                user = new Dictionary<string, object>
                {
                    { "uuid", Guid.NewGuid().ToString() },
                    { "name", binding.HostName },
                    { "isHost", binding.isHost },
                    { "mediapath",System.IO.Path.GetFileName(binding.MediaPath) },
                    { "mediasubtitle",System.IO.Path.GetFileName(binding.MediaSubtitlePath) },
                    { "password", txtpassword.Password },
                   { "IsPlaying",false },  { "started",false },
                    { "playtime",0 },
                };
                json = JsonConvert.SerializeObject(user, Formatting.Indented);
                Console.WriteLine(json);

                await docRef.SetAsync(user);
            }
            catch (Exception ex)
            {
                btnStartPlayback.IsEnabled = true;
                binding.isLoading = false;
                MessageBox.Show(ex.Message);
            }

        }

        private void btnStartHosting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (binding.HostName == null || (binding.HostName.Length < 4))
                {
                    binding.HostNameOkChar = CHAR_ERROR;
                    binding.HostNameOkColor = COLOR_ERROR;
                    return;
                }

                else
                {
                    binding.HostNameOkChar = CHAR_OK;
                    binding.HostNameOkColor = COLOR_OK;
                }

                if (txtpassword.Password == null || (txtpassword.Password.Length < 4))
                {
                    binding.PasswordOkChar = CHAR_ERROR;
                    binding.PasswordOkColor = COLOR_ERROR;
                    return;
                }
                else
                {
                    binding.PasswordOkChar = CHAR_OK;
                    binding.PasswordOkColor = COLOR_OK;
                }
                btnStartHosting.IsEnabled = false;
                setHost();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            JoinLobby(txtpassword.Password);
        }

        private void btncopymyuuid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(binding.MyUUID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStartPlayback_Click(object sender, RoutedEventArgs e)
        {
            startPlayback();
        }

        private void rbHost_Checked(object sender, RoutedEventArgs e)
        {
            binding.UUID = "";
        }

        private void rbHost_Unchecked(object sender, RoutedEventArgs e)
        {
            binding.UUID = "";
        }

        System.Threading.CancellationTokenSource token = new System.Threading.CancellationTokenSource();

        FirestoreChangeListener listner;
        async void JoinLobby(string password)
        {
            switch (btnJoin.Content)
            {
                case "Leave":
                    btnJoin.Content = "Join Lobby";
                    if (listner != null)
                        await listner.StopAsync();
                    token.Cancel();
                    binding.LobbyDetails += "\r\nleaved from lobby";
                    break;
                default:
                    {
                        btnJoin.Content = "Leave";
                        binding.LobbyDetails += "\r\nWaiting for host start..";
                        DocumentReference usersRef = db.Collection("users").Document(binding.UUID);
                        DocumentSnapshot document = await usersRef.GetSnapshotAsync();
                        Console.WriteLine("id: {0}", document.Id);
                        string partpassword = document.ToDictionary()["password"].ToString();
                        long time = (long)document.ToDictionary()["playtime"];
                        if (partpassword != password)
                        {
                            binding.LobbyDetails += "\r\n" + "Incorrect lobby password";
                            btnJoin.Content = "Join Lobby";
                            return;
                        }
                        else
                        {
                            listner = usersRef.Listen(snap =>
                            {
                                Console.WriteLine("snap : {0}, started : {1}", snap.Id, snap.ToDictionary()["started"]);
                                bool isstarted = (bool)snap.ToDictionary()["started"];
                                if (isstarted)
                                {
                                    startPlay(time);
                                }
                            }, token.Token);
                        }
                        break;
                    }
            }
        }

        async void startPlay(long playtime)
        {
            if (listner != null)
                await listner.StopAsync();
            this.Dispatcher.Invoke(() =>
            {
                btnJoin.IsEnabled = false;
                binding.isLoading = false;
                //  WindowState = WindowState.Minimized;
                binding.LobbyDetails += "\r\nParty started";
                new MainWindow(binding, txtpassword.Password, playtime).ShowDialog();
                binding.LobbyDetails += "\r\nParty ended";
                binding.isNotHosted = true;
                binding.isHosted = false;
                binding.isLoading = false;
                btnJoin.IsEnabled = true;
                btnJoin.Content = "Join Lobby";
                btnJoin.IsEnabled = true;
            });
        }
        async void checkLobby()
        {
            try
            {
                DocumentSnapshot document = await db.Collection("users").Document(binding.UUID).GetSnapshotAsync();
                Console.WriteLine("id: {0}", document.Id);
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                List<string> list = new List<string>();
                binding.LobbyDetails = "";

                foreach (var item in documentDictionary)
                {
                    list.Add(item.Key + " : " + item.Value);
                }
                list.Sort();
                binding.LobbyDetails = string.Join("\r\n", list.ToArray());
                //Console.WriteLine("uuid: {0}", documentDictionary["uuid"]);
                //Console.WriteLine("name: {0}", documentDictionary["name"]);
                //Console.WriteLine("isHost: {0}", documentDictionary["isHost"]);
                //Console.WriteLine("mediapath: {0}", documentDictionary["mediapath"]);
                //Console.WriteLine("mediasubtitle: {0}", documentDictionary["mediasubtitle"]);
                //Console.WriteLine("password: {0}", documentDictionary["password"]);
                //Console.WriteLine("started: {0}", documentDictionary["started"]);
                //Console.WriteLine("playtime: {0}", documentDictionary["playtime"]);
                //Console.WriteLine();
                if (documentDictionary != null)
                {
                    string json = JsonConvert.SerializeObject(documentDictionary, Formatting.Indented);
                    Console.WriteLine(json);
                    binding.LobbbyOkChar = CHAR_OK;
                    binding.LobbbyOkColor = COLOR_OK;
                }
                else
                {
                    binding.LobbbyOkChar = CHAR_ERROR;
                    binding.LobbbyOkColor = COLOR_ERROR;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnCheckLobby.IsEnabled = true;
            }
        }
        private void btnCheckLobby_Click(object sender, RoutedEventArgs e)
        {
            btnCheckLobby.IsEnabled = false;
            checkLobby();
        }
    }
}
