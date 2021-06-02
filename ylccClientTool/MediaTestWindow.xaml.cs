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

        public MediaTestWindow(CommonModel commonModel, WatchMessagesModel watchMessagesModel)
        {
            InitializeComponent();

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            Color mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = watchMessagesModel.MediaWidth + 40;
            Height = watchMessagesModel.MediaHeight + 40;

            TestMediaElement.Width = watchMessagesModel.MediaWidth;
            TestMediaElement.Height = watchMessagesModel.MediaHeight;

            dColor = System.Drawing.ColorTranslator.FromHtml(watchMessagesModel.LabelForeground);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            AuthorLabel.Foreground = new SolidColorBrush(mColor);
            AuthorLabel.FontSize = watchMessagesModel.LabelFontSize;
            MessageLabel.Foreground = new SolidColorBrush(mColor);
            MessageLabel.FontSize = watchMessagesModel.LabelFontSize;

            TestMediaElement.Source = new Uri(watchMessagesModel.MediaFile);
            TestMediaElement.Volume = watchMessagesModel.Volume / 100;
            AuthorLabel.Content = "Author";
            MessageLabel.Content = "Message";
        }
    }
}
