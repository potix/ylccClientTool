﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ylccClientTool
{

    public class WatchMessage : BaseModel
    {    
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

        private string author;
        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
                OnPropertyChanged("Author");
            }
        }
    }

    public class WatchMessagesModel : RandomChoiceModel
    {

        public ObservableCollection<WatchMessage> WatchMessages { get; set; }

        public WatchMessagesModel()
        {
            WatchMessages = new ObservableCollection<WatchMessage>();
        }
    }
}
