using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ylccClientTool
{
    public class RandomChoiceModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
        public Double Volume
        {
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

        public int MediaWidth { get; set; }
        public int MediaHeight { get; set; }

        public string LabelForeground { get; set; }
        public Double LabelFontSize { get; set; }

        public RandomChoiceModel()
        {
            Volume = 100.0;
            MediaWidth = 400;
            MediaHeight = 400;
            LabelForeground = "#FFFFFFFF";
            LabelFontSize = 20;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
