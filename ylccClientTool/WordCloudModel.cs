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

        private int messageLimit;
        public int MessageLimit
        {
            get
            {
                return this.messageLimit;
            }
            set
            {
                this.messageLimit = value;
                OnPropertyChanged("MessageLimit");
            }
        }

        private int fontMaxSize;
        public int FontMaxSize
        {
            get
            {
                return this.fontMaxSize;
            }
            set
            {
                this.fontMaxSize = value;
                OnPropertyChanged("FontMaxSize");
            }
        }

        private int fontMinSize;
        public int FontMinSize
        {
            get
            {
                return this.fontMinSize;
            }
            set
            {
                this.fontMinSize = value;
                OnPropertyChanged("FontMinSize");
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

        private string imageBackgroundColor;
        public string ImageBackgroundColor
        {
            get
            {
                return this.imageBackgroundColor;
            }
            set
            {
                this.imageBackgroundColor = value;
                OnPropertyChanged("ImageBackgroundColor");
            }
        }

        private  ObservableCollection<FontColor> fontColors;
        public ObservableCollection<FontColor> FontColors
        {
            get
            {
                return this.fontColors;
            }
            set
            {
                this.fontColors = value;
                OnPropertyChanged("FontColors");
            }
        }

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

        public void Update(WordCloudModel newModel)
        {
            MessageLimit = newModel.MessageLimit;
            FontMaxSize = newModel.FontMaxSize;
            FontMinSize = newModel.FontMinSize;
            Width = newModel.Width;
            Height = newModel.Height;
            ImageBackgroundColor = newModel.ImageBackgroundColor;
            FontColors.Clear();
            foreach (FontColor fc in newModel.FontColors)
            {
                FontColors.Add(fc);
            }
        }
    }
}
