using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ylccProtocol;

namespace ylccClientTool
{
    public class TargetValue: BaseModel
    {
        private string label;
        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
                OnPropertyChanged("Label");
            }
        }

        private Target target;
        public Target Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
                OnPropertyChanged("Target");
            }
        }
    }

    public class CommonModel: BaseModel
    {
        private string videoId;
        public string VideoId
        {
            get
            {
                return this.videoId;
            }
            set
            {
                this.videoId = value;
                OnPropertyChanged("VideoId");
            }
        }

        private string uri;
        public string Uri
        {
            get
            {
                return this.uri;
            }
            set
            {
                this.uri = value;
                OnPropertyChanged("Uri");
            }
        }

        private bool isInsecure;
        public bool IsInsecure
        {
            get
            {
                return this.isInsecure;
            }
            set
            {
                this.isInsecure = value;
                OnPropertyChanged("IsInsecure");
            }
        }

        private ObservableCollection<TargetValue> candidateTargetValues;
        public ObservableCollection<TargetValue> CandidateTargetValues
        {
            get
            {
                return this.candidateTargetValues;
            }
            set
            {
                this.candidateTargetValues = value;
                OnPropertyChanged("CandidateTargetValues");
            }
        }

        private TargetValue targetValue;
        public TargetValue TargetValue
        {
            get
            {
                return this.targetValue;
            }
            set
            {
                this.targetValue = value;
                OnPropertyChanged("TargetValue");
            }
        }

        private string windowBackgroundColor;
        public string WindowBackgroundColor
        {
            get
            {
                return this.windowBackgroundColor;
            }
            set
            {
                this.windowBackgroundColor = value;
                OnPropertyChanged("WindowBackgroundColor");
            }
        }

        public CommonModel()
        {
            TargetValue = new TargetValue { Label = "all user", Target = Target.AllUser };
            CandidateTargetValues = new ObservableCollection<TargetValue>();
            CandidateTargetValues.Add(TargetValue);
            CandidateTargetValues.Add(new TargetValue { Label = "owner and moderator and sponsor", Target = Target.OwnerModeratorSponsor });
            CandidateTargetValues.Add(new TargetValue { Label = "owner and moderator", Target = Target.OwnerModerator });
            VideoId = "";
            Uri = "http://127.0.0.1:12345";
            IsInsecure = true;
            WindowBackgroundColor = "#FF000000";
        }

        public void Update(CommonModel newModel)
        {
            VideoId = newModel.VideoId;
            Uri = newModel.Uri;
            IsInsecure = newModel.IsInsecure;
            WindowBackgroundColor = newModel.WindowBackgroundColor;
        }
    }
}
