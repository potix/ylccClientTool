using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ylccClientTool
{
    public class GroupingChoice: BaseModel
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

    public class GroupingModel
    {
        public ObservableCollection<GroupingChoice> GroupingChoices { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string BoxForegroundColor { get; set; }

        public string BoxBackgroundColor { get; set; }

        public string BoxBorderColor { get; set; }

        public int FontSize { get; set; }

        public int Padding { get; set; }

        public int Total { get; set; }

        public GroupingModel() {
            GroupingChoices = new ObservableCollection<GroupingChoice>();
            Total = 0;
            BoxForegroundColor = "#FFFFFFFF";
            BoxBackgroundColor = "#FF4169E1";
            BoxBorderColor = "#FF000080";
            Height = 900;
            Width = 1440;
            FontSize = 20;
            Padding = 32;
        }

    }
}
