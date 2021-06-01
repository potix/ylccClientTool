using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ylccClientTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CommonModel _commonModel = new CommonModel();
        private WatchMessagesModel _watchMessagesModel = new WatchMessagesModel();

        public MainWindow()
        {
            InitializeComponent();
            VideoIdTextBox.DataContext = _commonModel;
            URITextBox.DataContext = _commonModel;
            InsecureCheckBox.DataContext = _commonModel;
            WatchMessagesDataGrid.DataContext = _watchMessagesModel;
            WatchMessagesVolumeSlider.DataContext = _watchMessagesModel;
            WatchMessagesVolumeLabel.DataContext = _watchMessagesModel;
            WatchMessagesMediaTextBox.DataContext = _watchMessagesModel;

        }

        private void AddWatchMessageClick(object sender, EventArgs e)
        {
            if (WatchMessageTextBox.Text == null || WatchMessageTextBox.Text == "")
            {
                return;
            }
            _watchMessagesModel.WatchMessages.Add(new WatchMessage() { Message = WatchMessageTextBox.Text, Active = true });
            WatchMessageTextBox.Text = "";
        }

        private void WatchMessageRemove(object sender, EventArgs e)
        {
            if (WatchMessagesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _watchMessagesModel.WatchMessages.Remove(_watchMessagesModel.WatchMessages[WatchMessagesDataGrid.SelectedIndex]);
            WatchMessagesDataGrid.SelectedIndex = -1;
        }

        private void WatchMessagesSelectMedia(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "メディアファイル (*.wav)|*.wav|メディアファイル (*.wma)|*.wma|メディアファイル (*.wmv)|*.wmv|メディアファイル (*.midi)|*.midi|メディアファイル (*.avi)|*.avi|メディアファイル (*.mpg)|*.mpg|メディアファイル (*.mpeg)|*.mpeg|メディアファイル (*.mp4)|*.mp4|メディアファイル (*.mov)|*.mov|全てのファイル (*.*)|*.*";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                _watchMessagesModel.MediaFile = dialog.FileName;
            }
        }

        private void WatchMessagesTestMedia(object sender, EventArgs e)
        {
            if (WatchMessagesMediaTextBox.Text == null | WatchMessagesMediaTextBox.Text == "") {
                return;
            }
            MediaTestWindow w = new MediaTestWindow(_watchMessagesModel.MediaFile, _watchMessagesModel.Volume / 100);
            w.Show();
        }

        private void WatchMessagesStart(object sender, EventArgs e)
        {
            foreach(WatchMessage w in _watchMessagesModel.WatchMessages)
            {
                w.Active = false;
            }
        }

        private void RandomChoiceSelectMedia(object sender, EventArgs e)
        {

        }

        private void RandomChoiceTestMedia(object sender, EventArgs e)
        {

        }

        private void RandomChoiceStart(object sender, EventArgs e)
        {

        }

    }
}
