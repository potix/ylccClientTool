using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ylccClientTool
{
    /// <summary>
    /// MediaTestWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaTestWindow : Window
    {

        public MediaTestWindow(CommonModel commonModel, RandomChoiceModel randomChoiceModel)
        {
            InitializeComponent();

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            Color mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = randomChoiceModel.MediaWidth + 40;
            Height = randomChoiceModel.MediaHeight + 40;

            TestMediaElement.Width = randomChoiceModel.MediaWidth;
            TestMediaElement.Height = randomChoiceModel.MediaHeight;

            dColor = System.Drawing.ColorTranslator.FromHtml(randomChoiceModel.LabelForeground);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            AuthorLabel.Foreground = new SolidColorBrush(mColor);
            AuthorLabel.FontSize = randomChoiceModel.LabelFontSize;
            MessageLabel.Foreground = new SolidColorBrush(mColor);
            MessageLabel.FontSize = randomChoiceModel.LabelFontSize;

            TestMediaElement.Source = new Uri(randomChoiceModel.MediaFile);
            TestMediaElement.Volume = randomChoiceModel.Volume / 100;
            AuthorLabel.Content = "Author";
            MessageLabel.Content = "Message";
        }
    }
}
