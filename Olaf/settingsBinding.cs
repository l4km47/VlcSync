using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VlcSync
{
    public class settingsBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #region INotifyPropertyChanged Members


        string _LobbyDetails;
        public string LobbyDetails
        {
            get { return _LobbyDetails; }
            set
            {
                if (_LobbyDetails != value)
                {
                    _LobbyDetails = value;
                    OnPropertyChanged(nameof(LobbyDetails));
                }
            }
        }

        bool _isLoading;
        public bool isLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(isLoading));
                }
            }
        }
        string _HostName;
        public string HostName
        {
            get { return _HostName; }
            set
            {
                if (_HostName != value)
                {
                    _HostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }
        string _UUID;
        public string UUID
        {
            get { return _UUID; }
            set
            {
                if (_UUID != value)
                {
                    _UUID = value;
                    OnPropertyChanged(nameof(UUID));
                }
            }
        }

        bool _isHosted;
        public bool isHosted
        {
            get { return _isHosted; }
            set
            {
                if (_isHosted != value)
                {
                    _isHosted = value;
                    OnPropertyChanged(nameof(isHosted));
                }
            }
        }

        bool _isNotHosted;
        public bool isNotHosted
        {
            get { return _isNotHosted; }
            set
            {
                if (_isNotHosted != value)
                {
                    _isNotHosted = value;
                    OnPropertyChanged(nameof(isNotHosted));
                }
            }
        }


        string _ServerStatus;
        public string ServerStatus
        {
            get { return _ServerStatus; }
            set
            {
                if (_ServerStatus != value)
                {
                    _ServerStatus = value;
                    OnPropertyChanged(nameof(ServerStatus));
                }
            }
        }

        string _ServerStatusColor;
        public string ServerStatusColor
        {
            get { return _ServerStatusColor; }
            set
            {
                if (_ServerStatusColor != value)
                {
                    _ServerStatusColor = value;
                    OnPropertyChanged(nameof(ServerStatusColor));
                }
            }
        }

        string _MyUUID;
        public string MyUUID
        {
            get { return _MyUUID; }
            set
            {
                if (_MyUUID != value)
                {
                    _MyUUID = value;
                    OnPropertyChanged(nameof(MyUUID));
                }
            }
        }

        bool _isHost;
        public bool isHost
        {
            get { return _isHost; }
            set
            {
                if (_isHost != value)
                {
                    _isHost = value;
                    OnPropertyChanged(nameof(isHost));
                }
            }
        }

        bool _isWatcher;
        public bool isWatcher
        {
            get { return _isWatcher; }
            set
            {
                if (_isWatcher != value)
                {
                    _isWatcher = value;
                    OnPropertyChanged(nameof(isWatcher));
                }
            }
        }


        string _LobbbyOkColor;
        public string LobbbyOkColor
        {
            get { return _LobbbyOkColor; }
            set
            {
                if (_LobbbyOkColor != value)
                {
                    _LobbbyOkColor = value;
                    OnPropertyChanged(nameof(LobbbyOkColor));
                }
            }
        }

        string _LobbbyOkChar;
        public string LobbbyOkChar
        {
            get { return _LobbbyOkChar; }
            set
            {
                if (_LobbbyOkChar != value)
                {
                    _LobbbyOkChar = value;
                    OnPropertyChanged(nameof(LobbbyOkChar));
                }
            }
        }


        string _MediaPath;
        public string MediaPath
        {
            get { return _MediaPath; }
            set
            {
                if (_MediaPath != value)
                {
                    _MediaPath = value;
                    OnPropertyChanged(nameof(MediaPath));
                }
            }
        }


        string _MediaPathOkColor;
        public string MediaPathOkColor
        {
            get { return _MediaPathOkColor; }
            set
            {
                if (_MediaPathOkColor != value)
                {
                    _MediaPathOkColor = value;
                    OnPropertyChanged(nameof(MediaPathOkColor));
                }
            }
        }


        string _MediaPathOkChar;
        public string MediaPathOkChar
        {
            get { return _MediaPathOkChar; }
            set
            {
                if (_MediaPathOkChar != value)
                {
                    _MediaPathOkChar = value;
                    OnPropertyChanged(nameof(MediaPathOkChar));
                }
            }
        }


        string _MediaSubtitlePath;
        public string MediaSubtitlePath
        {
            get { return _MediaSubtitlePath; }
            set
            {
                if (_MediaSubtitlePath != value)
                {
                    _MediaSubtitlePath = value;
                    OnPropertyChanged(nameof(MediaSubtitlePath));
                }
            }
        }

        string _MediaSubtitlePathOkColor;
        public string MediaSubtitlePathOkColor
        {
            get { return _MediaSubtitlePathOkColor; }
            set
            {
                if (_MediaSubtitlePathOkColor != value)
                {
                    _MediaSubtitlePathOkColor = value;
                    OnPropertyChanged(nameof(MediaSubtitlePathOkColor));
                }
            }
        }


        string _MediaSubtitlePathOkChar;
        public string MediaSubtitlePathOkChar
        {
            get { return _MediaSubtitlePathOkChar; }
            set
            {
                if (_MediaSubtitlePathOkChar != value)
                {
                    _MediaSubtitlePathOkChar = value;
                    OnPropertyChanged(nameof(MediaSubtitlePathOkChar));
                }
            }
        }


        string _HostNameOkColor;
        public string HostNameOkColor
        {
            get { return _HostNameOkColor; }
            set
            {
                if (_HostNameOkColor != value)
                {
                    _HostNameOkColor = value;
                    OnPropertyChanged(nameof(HostNameOkColor));
                }
            }
        }

        string _HostNameOkChar;
        public string HostNameOkChar
        {
            get { return _HostNameOkChar; }
            set
            {
                if (_HostNameOkChar != value)
                {
                    _HostNameOkChar = value;
                    OnPropertyChanged(nameof(HostNameOkChar));
                }
            }
        }

        string _PasswordOkColor;
        public string PasswordOkColor
        {
            get { return _PasswordOkColor; }
            set
            {
                if (_PasswordOkColor != value)
                {
                    _PasswordOkColor = value;
                    OnPropertyChanged(nameof(PasswordOkColor));
                }
            }
        }


        string _PasswordOkChar;
        public string PasswordOkChar
        {
            get { return _PasswordOkChar; }
            set
            {
                if (_PasswordOkChar != value)
                {
                    _PasswordOkChar = value;
                    OnPropertyChanged(nameof(PasswordOkChar));
                }
            }
        }

        #endregion
    }
}
