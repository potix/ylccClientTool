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

    public class GroupingModel :BaseModel
    {
        private ObservableCollection<GroupingChoice> groupingChoices;
        public ObservableCollection<GroupingChoice> GroupingChoices
        {
            get
            {
                return this.groupingChoices;
            }
            set
            {
                this.groupingChoices = value;
                OnPropertyChanged("GroupingChoices");
            }
        }

        private int width;
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                OnPropertyChanged("Width");
            }
        }

        private int height;
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
                OnPropertyChanged("Height");
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
