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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Cloud.Firestore;
using LibVLCSharp.Shared;
using DocumentReference = Google.Cloud.Firestore.DocumentReference;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace VlcSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        FirestoreDb db;
        const string PROJECT_ID = "vlcsync-af4e8";
        settingsBinding settingsbbinding;
        string hostpassword;
        long hostplaytime = 0;
        public MainWindow(settingsBinding sbbinding, string password, long _hostplaytime)
        {
            InitializeComponent();
            Activate();
            settingsbbinding = sbbinding;
            hostpassword = password;
            this.hostplaytime = _hostplaytime;
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += Player_loaded;
            Unloaded += MainWindow_Unloaded;
            Loaded += _Loaded;
            this.DataContext = binding;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

            filename = sbbinding.MediaPath;
            subtitle = sbbinding.MediaSubtitlePath;
            db = FirestoreDb.Create(PROJECT_ID);

        }

        private void Player_loaded(object sender, RoutedEventArgs e)
        {
            VideoView.MediaPlayer = _mediaPlayer;
            play();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Hidden;
            grdmain.RowDefinitions[0].Height = new GridLength(0);
            lblpaused.Text = "";
            dispatcherTimer.Stop();
        }

        private void VideoView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying) return;
            lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
            grdmain.RowDefinitions[0].Height = new GridLength(32);
            dispatcherTimer.Start();

        }
        private readonly mediaBindings binding = new mediaBindings()
        {
            CurrentDuration = 0,
            Duration = 0,
            MediaTitle = "",
            RowSpan = 2,
        };
        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoView.Dispose();
        }
        string filename = "", subtitle = "";
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

            play();
        }

        async void play()
        {
            try
            {
                if (VideoView.MediaPlayer.Media != null)
                {
                    if (!settingsbbinding.isHost) return;
                    if (VideoView.MediaPlayer.IsPlaying)
                    {
                        VideoView.MediaPlayer.Pause();


                        hostplaytime = VideoView.MediaPlayer.Time;
                        btnPlay.Content = "Play";
                        lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
                        grdmain.RowDefinitions[0].Height = new GridLength(32);
                        dispatcherTimer.Stop();
                    }
                    else
                    {
                        dispatcherTimer.Start();
                        VideoView.MediaPlayer.Play();
                        btnPlay.Content = "Pause";
                    }
                }
                else
                {
                    if (!VideoView.MediaPlayer.IsPlaying)
                    {
                        if (!System.IO.File.Exists(filename)) return;
                        //  lblpaused.Visibility = Visibility.Hidden;
                        lblpaused.Text = "Playback started";
                        using (var media = new Media(_libVLC,
                            new Uri(filename).AbsoluteUri
                            , FromType.FromLocation, new string[] { $"--sub-file={subtitle}" }))
                        {


                            Console.WriteLine(hostplaytime);
                            VideoView.MediaPlayer.Media = media;
                            VideoView.MediaPlayer.Play();
                            VideoView.MediaPlayer.Time = hostplaytime;
                            this.binding.MediaTitle = VideoView.MediaPlayer.Media.Meta(MetadataType.Title);
                            VideoView.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                            VideoView.MediaPlayer.EndReached += MediaPlayer_EndReached;
                            VideoView.MediaPlayer.Paused += Paused;
                            VideoView.MediaPlayer.Playing += Playing1;
                            lblpaused.Text = "";
                            btnPlay.Content = "Pause";
                            dispatcherTimer.Start();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async void setPlayState()
        {
            await this.Dispatcher.Invoke(async () =>
             {
                 if (!settingsbbinding.isHost) return;
                 DocumentReference docRef = db.Collection("users").Document(settingsbbinding.MyUUID);
                 Dictionary<string, object> user = new Dictionary<string, object>
                                     {
                                        { "uuid", settingsbbinding.UUID },
                                        { "name", settingsbbinding.HostName },
                                        { "isHost", settingsbbinding.isHost },
                                        { "mediapath",System.IO.Path.GetFileName(settingsbbinding.MediaPath) },
                                        { "mediasubtitle",System.IO.Path.GetFileName(settingsbbinding.MediaSubtitlePath) },
                                        { "password", hostpassword },
                                        { "started",true },
                                        { "IsPlaying",VideoView.MediaPlayer.IsPlaying },
                                        { "playtime",VideoView.MediaPlayer.Time },
                                     };
                 await docRef.SetAsync(user);
             });
        }
        private void Playing1(object sender, EventArgs e)
        {
            setPlayState();
        }

        private void Paused(object sender, EventArgs e)
        {
            setPlayState();
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                if (VideoView.MediaPlayer is null)
                    MessageBox.Show("media player is null");
                seekbar.Value = 0;
                btnPlay.Content = "Play";

            });
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblpaused.Text = "";
            });

        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!VideoView.MediaPlayer.IsPlaying) return;

                TimeSpan t = TimeSpan.FromMilliseconds(VideoView.MediaPlayer.Length);
                string length = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds);
                TimeSpan t2 = TimeSpan.FromMilliseconds(VideoView.MediaPlayer.Time);
                string Position = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                this.binding.CurrentDuration = (int)VideoView.MediaPlayer.Time;
                this.binding.Duration = (int)VideoView.MediaPlayer.Length;
                this.binding.MediaTime = $"{Position} / {length}";
                this.binding.Volume = (int)VideoView.MediaPlayer.Volume;
                this.binding.VolumeMax = 200;
                binding.Fps = "Fps " + VideoView.MediaPlayer.Fps.ToString();

                if (settingsbbinding.isHost)
                {
                    DocumentReference docRef = db.Collection("users").Document(settingsbbinding.MyUUID);
                    Dictionary<string, object> user = new Dictionary<string, object>
                        {
                            { "uuid", settingsbbinding.UUID },
                            { "name", settingsbbinding.HostName },
                            { "isHost", settingsbbinding.isHost },
                            { "mediapath",System.IO.Path.GetFileName(settingsbbinding.MediaPath) },
                            { "mediasubtitle",System.IO.Path.GetFileName(settingsbbinding.MediaSubtitlePath) },
                            { "password", hostpassword },
                            { "started",true },
                            { "IsPlaying",VideoView.MediaPlayer.IsPlaying },
                            { "playtime",VideoView.MediaPlayer.Time },
                        };
                    docRef.SetAsync(user);
                }

            });

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (VideoView.MediaPlayer.IsPlaying)
            {
                lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
                grdmain.RowDefinitions[0].Height = new GridLength(32);
                VideoView.MediaPlayer.Stop();
                lblpaused.Text = "Stoped";
            }
        }

        private void VideoView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        bool seeking = false;
        private void seekbar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            VideoView.MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
            seeking = true;
            dispatcherTimer.Stop();
        }

        private void seekbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (seeking)
            {
                VideoView.MediaPlayer.Time = (long)seekbar.Value;
            }
        }

        private void seekbar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            VideoView.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            seeking = false;
            dispatcherTimer.Start();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying) return;
            lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
            grdmain.RowDefinitions[0].Height = new GridLength(32);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
            switch (e.Key)
            {
                case Key.Space:
                    if (!settingsbbinding.isHost) return;
                    if (VideoView.MediaPlayer.IsPlaying)
                    {
                        lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
                        dispatcherTimer.Stop();
                        VideoView.MediaPlayer.Pause();
                        lblpaused.Text = "Paused";
                    }
                    else
                    {
                        VideoView.MediaPlayer.Play();
                        lblpaused.Text = "";
                    }
                    break;
                case Key.Left:
                    if (!settingsbbinding.isHost) return;
                    VideoView.MediaPlayer.Time -= 5000;
                    lblpaused.Text = "-5sec";

                    break;
                case Key.Right:
                    if (!settingsbbinding.isHost) return;
                    VideoView.MediaPlayer.Time += 5000;
                    lblpaused.Text = "+5sec";
                    break;
                case Key.Up:
                    if (VideoView.MediaPlayer.Volume <= 200)
                    {
                        VideoView.MediaPlayer.Volume += 5;
                    }
                    break;
                case Key.Down:
                    VideoView.MediaPlayer.Volume -= 5;
                    break;
                default:
                    break;
            }
        }

        private void Window_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VideoView.MediaPlayer.IsPlaying)
            {
                lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
                grdmain.RowDefinitions[0].Height = new GridLength(32);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                dispatcherTimer.Start();
            }
        }

        private void VideoView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying) return;
            lblVolume.Visibility = pbVolume.Visibility = titlebar.Visibility = controalpannel.Visibility = Visibility.Visible;
            grdmain.RowDefinitions[0].Height = new GridLength(32);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
            if (e.Delta > 0)
            {
                if (VideoView.MediaPlayer.Volume <= 200)
                {
                    VideoView.MediaPlayer.Volume += 5;
                }
            }
            else if (e.Delta < 0)
                VideoView.MediaPlayer.Volume -= 5;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void VideoView_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Button_Click_4(object sender, MouseButtonEventArgs e)
        {
            new host().ShowDialog();
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, MouseButtonEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void Button_Click_2(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_3(object sender, MouseButtonEventArgs e)
        {
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
                filename = dlg.FileName;
                VideoView.MediaPlayer.Media = null;
                play();
            }
        }
    }
}
