using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ylccClientTool
{

    public class WatchMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string message;
        public string Message {
            get 
            {
                return this.message;
            }
            set
            {
                this.message = value;
                OnPropertyChanged("Message");
            }
        }

        private bool active;
        public bool Active {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
                OnPropertyChanged("Active");
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class WatchMessagesModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WatchMessage> WatchMessages { get; set; }

        private string mediaFile;
        public string MediaFile
        {
            get
            {
                return this.mediaFile;
            }
            set
            {
                this.mediaFile = value;
                OnPropertyChanged("MediaFile");
            }
        } 

        private Double volume;
        public Double Volume {
            get 
            {
                return this.volume;
            }
            set
            {
                this.volume = value;
                OnPropertyChanged("Volume");
            }
        }



        public WatchMessagesModel()
        {
            WatchMessages = new ObservableCollection<WatchMessage>();
            Volume = 100.0;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
