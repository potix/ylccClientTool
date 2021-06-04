using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ylccProtocol;

namespace ylccClientTool
{
    public class VoteChoice : BaseModel
    {
        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                OnPropertyChanged("Text");
            }
        }
    }

    public class VoteModel: BaseModel
    {
        public ObservableCollection<VoteChoice> VoteChoices { get; set; }

        private int duration;
        public int Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                OnPropertyChanged("Duration");
            } 
        }

        private string boxForegroundColor;
        public string BoxForegroundColor
        {
            get
            {
                return this.boxForegroundColor;
            }
            set
            {
                this.boxForegroundColor = value;
                OnPropertyChanged("BoxForegroundColor");
            }
        }

        private string boxBackgroundColor;
        public string BoxBackgroundColor
        {
            get
            {
                return this.boxBackgroundColor;
            }
            set
            {
                this.boxBackgroundColor = value;
                OnPropertyChanged("BoxBackgroundColor");
            }
        }

        private string boxBorderColor;
        public string BoxBorderColor
        {
            get
            {
                return this.boxBorderColor;
            }
            set
            {
                this.boxBorderColor = value;
                OnPropertyChanged("BoxBorderColor");
            }
        }

        private int fontSize;
        public int FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                this.fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        private int padding;
        public int Padding
        {
            get
            {
                return this.padding;
            }
            set
            {
                this.padding = value;
                OnPropertyChanged("Padding");
            }
        }

        private int total;
        public int Total
        {
            get
            {
                return this.total;
            }
            set
            {
                this.total = value;
                OnPropertyChanged("Total");
            }
        }

        public VoteModel()
        {
            VoteChoices = new ObservableCollection<VoteChoice>();
            Duration = 5;
            Total = 0;
            BoxForegroundColor = "#FFFFFF";
            BoxBackgroundColor = "#4169E1";
            BoxBorderColor = "#000080";
            FontSize = 20;
            Padding = 32;
        }
    }
}
