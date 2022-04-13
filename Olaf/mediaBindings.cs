using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VlcSync
{
    internal class mediaBindings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }

        string _fps;
        public string Fps
        {
            get { return _fps; }
            set
            {
                if (_fps != value)
                {
                    _fps = value;
                    OnPropertyChanged(nameof(Fps));
                }
            }
        }

        int _row;
        public int RowSpan
        {
            get { return _row; }
            set
            {
                if (_row != value)
                {
                    _row = value;
                    OnPropertyChanged(nameof(RowSpan));
                }
            }
        }

        int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        int _volumemax;
        public int VolumeMax
        {
            get { return _volumemax; }
            set
            {
                if (_volumemax != value)
                {
                    _volumemax = value;
                    OnPropertyChanged(nameof(VolumeMax));
                }
            }
        }

        string _mediatime;
        public string MediaTime
        {
            get { return _mediatime; }
            set
            {
                if (_mediatime != value)
                {
                    _mediatime = value;
                    OnPropertyChanged(nameof(MediaTime));
                }
            }
        }

        int _duration;
        public int Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }
        int _currentduration;
        public int CurrentDuration
        {
            get { return _currentduration; }
            set
            {
                if (_currentduration != value)
                {
                    _currentduration = value;
                    OnPropertyChanged(nameof(CurrentDuration));
                }
            }
        }
        string _mediatitle;
        public string MediaTitle
        {
            get { return _mediatitle; }
            set
            {
                if (_mediatitle != value)
                {
                    _mediatitle = value;
                    OnPropertyChanged(nameof(MediaTitle));
                }
            }
        }
    }
}
