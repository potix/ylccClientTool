using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ylccClientTool
{
    public class RandomChoiceModel : BaseModel
    {     
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

        private int mediaWidth;
        public int MediaWidth
        {
            get
            {
                return this.mediaWidth;
            }
            set
            {
                this.mediaWidth = value;
                OnPropertyChanged("MediaWidth");
            }
        }

        private int hediaHeight;
        public int MediaHeight
        {
            get
            {
                return this.hediaHeight;
            }
            set
            {
                this.hediaHeight = value;
                OnPropertyChanged("MediaHeight");
            }
        }

        private string labelForeground;
        public string LabelForeground
        {
            get
            {
                return this.labelForeground;
            }
            set
            {
                this.labelForeground = value;
                OnPropertyChanged("LabelForeground");
            }
        }

        private int labelFontSize;
        public int LabelFontSize
        {
            get
            {
                return this.labelFontSize;
            }
            set
            {
                this.labelFontSize = value;
                OnPropertyChanged("LabelFontSize");
            }
        }


        public RandomChoiceModel()
        {
            Volume = 100.0;
            MediaWidth = 400;
            MediaHeight = 400;
            LabelForeground = "#FFFFFFFF";
            LabelFontSize = 20;
        }

        public void Update(RandomChoiceModel newModel)
        {
            Volume = newModel.Volume;
            MediaWidth = newModel.MediaWidth;
            MediaHeight = newModel.MediaHeight;
            LabelForeground = newModel.LabelForeground;
            LabelFontSize = newModel.LabelFontSize;
        }
    }
}
