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
    /// WatchMessagesWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WatchMessagesWindow : Window
    {
        private CommonModel _commonModel;
        private WatchMessagesModel _watchMessagesModel;

        public WatchMessagesWindow(CommonModel commonModel, WatchMessagesModel watchMessagesModel)
        {
            InitializeComponent();
            _commonModel = commonModel;
            _watchMessagesModel = watchMessagesModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            Color mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = watchMessagesModel.MediaWidth + 40;
            Height = watchMessagesModel.MediaHeight + 40;

            WatchMessagesMediaElement.Width = watchMessagesModel.MediaWidth;
            WatchMessagesMediaElement.Height = watchMessagesModel.MediaHeight;

            //TestMediaElement.Source = new Uri(watchMessagesModel.MediaFile);
            //TestMediaElement.Volume = watchMessagesModel.Volume / 100;
        }
    }
}
