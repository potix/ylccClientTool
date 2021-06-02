using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ylccProtocol;

namespace ylccClientTool
{
    public class TargetValue
    {
        public string Label { get; set; }
        public Target Target { get; set; }
    }

    public class CommonModel
    {
        public string VideoId { get; set; }

        public string Uri { get; set; }

        public bool IsInsecure { get; set; }

        public ObservableCollection<TargetValue> CandidateTargetValues { get; set; }

        public TargetValue TargetValue { get; set; }

        public string WindowBackgroundColor { get; set; }

        public CommonModel()
        {
            VideoId = "";
            Uri = "http://127.0.0.1:12345";
            IsInsecure = true;
            CandidateTargetValues = new ObservableCollection<TargetValue>();
            TargetValue defaultTargetValue = new TargetValue { Label = "all user", Target = Target.AllUser };
            CandidateTargetValues.Add(defaultTargetValue);
            CandidateTargetValues.Add(new TargetValue { Label = "owner and moderator and sponsor", Target = Target.OwnerModeratorSponsor });
            CandidateTargetValues.Add(new TargetValue { Label = "owner and moderator", Target = Target.OwnerModerator });
            TargetValue = defaultTargetValue;
            WindowBackgroundColor = "#FF000000";
        }
    }
}
