using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private RandomChoiceModel _randomChoiceModel = new RandomChoiceModel();

        public MainWindow()
        {
            InitializeComponent();
            VideoIdTextBox.DataContext = _commonModel;
            URITextBox.DataContext = _commonModel;
            TargetComboBox.DataContext = _commonModel;
            InsecureCheckBox.DataContext = _commonModel;
            WindowBackgroundColorTextBox.DataContext = _commonModel;
            WindowBackgroundColorBorder.DataContext = _commonModel;
            WatchMessagesDataGrid.DataContext = _watchMessagesModel;
            WatchMessagesVolumeSlider.DataContext = _watchMessagesModel;
            WatchMessagesVolumeLabel.DataContext = _watchMessagesModel;
            WatchMessagesMediaTextBox.DataContext = _watchMessagesModel;
            WatchMessagesMediaWidthTextBox.DataContext = _watchMessagesModel;
            WatchMessagesMediaHeightTextBox.DataContext = _watchMessagesModel;
            WatchMessagesLabelForeground.DataContext = _watchMessagesModel;
            RandomChoiceLabelForegroundBorder.DataContext = _watchMessagesModel;
            WatchMessagesLabelFontSize.DataContext = _watchMessagesModel;

            RandomChoiceMediaTextBox.DataContext = _randomChoiceModel;
            RandomChoiceVolumeSlider.DataContext = _randomChoiceModel;
            RandomChoiceVolumeLabel.DataContext = _randomChoiceModel;
            RandomChoiceMediaWidthTextBox.DataContext = _randomChoiceModel;
            RandomChoiceHeightTextBox.DataContext = _randomChoiceModel;
            RandomChoiceLabelForeground.DataContext = _randomChoiceModel;
            RandomChoiceLabelForegroundBorder.DataContext = _randomChoiceModel;
            RandomChoiceLabelFontSize.DataContext = _randomChoiceModel;
        }

        private void AddWatchMessageClick(object sender, EventArgs e)
        {
            if (WatchMessageTextBox.Text == null || WatchMessageTextBox.Text == "")
            {
                return;
            }
            _watchMessagesModel.WatchMessages.Add(new WatchMessage() { Message = WatchMessageTextBox.Text, Active = true, Author = "" });
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
            dialog.Filter = "media file (*.*)|*.*";
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
            MediaTestWindow window = new MediaTestWindow(_commonModel,  _watchMessagesModel);
            window.Show();
        }

        private void WatchMessagesStart(object sender, EventArgs e)
        {

            WatchMessagesWindow window = new WatchMessagesWindow(_commonModel, _watchMessagesModel);
            window.Show();
            window.WatchMessage();
        }

        private void RandomChoiceSelectMedia(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "media file (*.*)|*.*";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                _randomChoiceModel.MediaFile = dialog.FileName;
            }
        }

        private void RandomChoiceTestMedia(object sender, EventArgs e)
        {
            if (RandomChoiceMediaTextBox.Text == null | RandomChoiceMediaTextBox.Text == "")
            {
                return;
            }
            MediaTestWindow window = new MediaTestWindow(_commonModel, _randomChoiceModel);
            window.Show();
        }

        private void RandomChoiceStart(object sender, EventArgs e)
        {
            RandomChoiceWindow window = new RandomChoiceWindow(_commonModel, _randomChoiceModel);
            window.Show();
            window.Start();
        }


        private void DoublePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9.]").IsMatch(e.Text);
        }

        private void DoublePreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void IntPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void ColorPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9a-fA-f#]").IsMatch(e.Text);
        }

        private void ColorPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
