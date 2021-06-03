using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ylccProtocol;

namespace ylccClientTool
{

    public class VoteChoice: BaseModel
    {
        public string Text { get; set; }
    }

    public class VoteResult: BaseModel
    {
        public int Count { get; set; }

        public double Rate { get; set; }
    }

    public class VoteModel: BaseModel
    {
        public ObservableCollection<VoteChoice> VoteChoices { get; set; }

        public int Duration { get; set; }

        public string BoxForegroundColor { get; set; }

        public string BoxBackgroundColor { get; set; }

        public string BoxBorderColor { get; set; }

        public int FontSize { get; set; }

        public int Padding { get; set; }

        public int Total { get; set; }

        public ObservableCollection<VoteResult> VoteResults { get; set; }

        public VoteModel()
        {
            VoteChoices = new ObservableCollection<VoteChoice>();
            Duration = 5;
            Total = 0;
            BoxForegroundColor = "#FFFFFF";
            BoxBackgroundColor = "#4169E1";
            BoxBorderColor = "#000080";
            VoteResults = null;
            FontSize = 20;
            Padding = 32;
        }

        public bool UpdateResults(int total, ICollection<VoteCount> counts)
        {
            if (total == 0)
            {
                return false;
            }
            Total = total;
            VoteResults = new ObservableCollection<VoteResult>();
            foreach (VoteCount count in counts)
            {
                VoteResults.Add(new VoteResult() { Count = count.Count, Rate = Math.Ceiling((double)count.Count * 100.0 / (double)total * 10) / 10 });
            }
            return true;
        }
    }
}
