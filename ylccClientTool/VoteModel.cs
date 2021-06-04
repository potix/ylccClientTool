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

        public string BoxForegroundColor { get; set; }

        public string BoxBackgroundColor { get; set; }

        public string BoxBorderColor { get; set; }

        public int FontSize { get; set; }

        public int Padding { get; set; }

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
