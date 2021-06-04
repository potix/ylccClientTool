using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ylccProtocol;

namespace ylccClientTool
{
    public class FontColor: BaseModel
    {
        private string color;
        public string Color {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                OnPropertyChanged("Color");
            }
        }

    }

    public class WordCloudModel: BaseModel
    {
        private readonly YlccProtocol protocol = new YlccProtocol();

        public int MessageLimit { get; set; }

        public int FontMaxSize { get; set; }

        public int FontMinSize { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageBackgroundColor { get; set; }

        public ObservableCollection<FontColor> FontColors { get; set; }

        public WordCloudModel()
        {
            MessageLimit = 25;
            FontMaxSize = 128;
            FontMinSize = 16;
            Width = 1000;
            Height = 500;
            ImageBackgroundColor = "#FFFFFF";
            FontColors = new ObservableCollection<FontColor>();
            FontColors.Add(new FontColor() { Color = "#DF0000" });
            FontColors.Add(new FontColor() { Color = "#00DF00" });
            FontColors.Add(new FontColor() { Color = "#0000DF" });
            FontColors.Add(new FontColor() { Color = "#DFDF00" });
            FontColors.Add(new FontColor() { Color = "#00DFDF" });
            FontColors.Add(new FontColor() { Color = "#DF00DF" });
        }

        public Collection<Color> GetFontColors()
        {
            Collection<Color> colors = new Collection<Color>();
            foreach (FontColor fontColor in FontColors)
            {
                colors.Add(protocol.BuildColor(fontColor.Color));
            }
            return colors;

        }

        public Color GetBackgroundColor()
        {
            return protocol.BuildColor(ImageBackgroundColor);
        }
    }
}
