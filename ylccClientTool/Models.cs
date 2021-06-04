using System;
using System.Collections.Generic;
using System.Text;

namespace ylccClientTool
{
    public class Models
    {
        public CommonModel CommonModel { get; set; }
        public WatchMessagesModel WatchMessagesModel { get; set; }
        public RandomChoiceModel RandomChoiceModel { get; set; }
        public GroupingModel GroupingModel { get; set; }
        public WordCloudModel WordCloudModel { get; set; }
        public VoteModel VoteModel { get; set; }

        public Models()
        {
            CommonModel = new CommonModel();
            WatchMessagesModel = new WatchMessagesModel();
            RandomChoiceModel = new RandomChoiceModel();
            GroupingModel = new GroupingModel();
            WordCloudModel = new WordCloudModel();
            VoteModel = new VoteModel();
        }

        public void Update(Models newModels)
        {
            CommonModel.Update(newModels.CommonModel);
            WatchMessagesModel.Update(newModels.WatchMessagesModel);
            RandomChoiceModel.Update(newModels.RandomChoiceModel);
            GroupingModel.Update(newModels.GroupingModel);
            WordCloudModel.Update(newModels.WordCloudModel);
            VoteModel.Update(newModels.VoteModel);
        }
    }
}
