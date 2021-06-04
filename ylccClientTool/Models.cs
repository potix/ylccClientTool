using System;
using System.Collections.Generic;
using System.Text;

namespace ylccClientTool
{
    public class Models
    {
        public CommonModel CommonModel;
        public WatchMessagesModel WatchMessagesModel;
        public RandomChoiceModel RandomChoiceModel;
        public GroupingModel GroupingModel;
        public WordCloudModel WordCloudModel;
        public VoteModel VoteModel;

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
