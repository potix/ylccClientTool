using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
        Models _models = new Models();

        public MainWindow()
        {
            InitializeComponent();

            string json = Properties.Settings.Default.lastConfig;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Models));
                Models newModels = ser.ReadObject(ms) as Models;
                ms.Close();
                _models.Update(newModels);
            } 
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                Debug.Print("Deserialization error: " + ex.ToString());
            }
            finally
            {
                ms.Close();
            }

            VideoIdTextBox.DataContext = _models.CommonModel;
            URITextBox.DataContext = _models.CommonModel;
            TargetComboBox.DataContext = _models.CommonModel;
            InsecureCheckBox.DataContext = _models.CommonModel;
            WindowBackgroundColorTextBox.DataContext = _models.CommonModel;
            WindowBackgroundColorBorder.DataContext = _models.CommonModel;

            WatchMessagesDataGrid.DataContext = _models.WatchMessagesModel;
            WatchMessagesVolumeSlider.DataContext = _models.WatchMessagesModel;
            WatchMessagesVolumeLabel.DataContext = _models.WatchMessagesModel;
            WatchMessagesMediaTextBox.DataContext = _models.WatchMessagesModel;
            WatchMessagesMediaWidthTextBox.DataContext = _models.WatchMessagesModel;
            WatchMessagesMediaHeightTextBox.DataContext = _models.WatchMessagesModel;
            WatchMessagesLabelForegroundTextBox.DataContext = _models.WatchMessagesModel;
            RandomChoiceLabelForegroundBorder.DataContext = _models.WatchMessagesModel;
            WatchMessagesLabelFontSize.DataContext = _models.WatchMessagesModel;

            RandomChoiceMediaTextBox.DataContext = _models.RandomChoiceModel;
            RandomChoiceVolumeSlider.DataContext = _models.RandomChoiceModel;
            RandomChoiceVolumeLabel.DataContext = _models.RandomChoiceModel;
            RandomChoiceMediaWidthTextBox.DataContext = _models.RandomChoiceModel;
            RandomChoiceHeightTextBox.DataContext = _models.RandomChoiceModel;
            RandomChoiceLabelForegroundTextBox.DataContext = _models.RandomChoiceModel;
            RandomChoiceLabelForegroundBorder.DataContext = _models.RandomChoiceModel;
            RandomChoiceLabelFontSize.DataContext = _models.RandomChoiceModel;

            GroupingChoicesDataGrid.DataContext = _models.GroupingModel;
            GroupingWidthTextBox.DataContext = _models.GroupingModel;
            GroupingHeightTextBox.DataContext = _models.GroupingModel;
            GroupingBoxForegroundColorTextBox.DataContext = _models.GroupingModel;
            GroupingBoxForegroundColorBorder.DataContext = _models.GroupingModel;
            GroupingBoxBackgroundColorTextBox.DataContext = _models.GroupingModel;
            GroupingBoxBackgroundColorBorder.DataContext = _models.GroupingModel;
            GroupingBoxBorderColorTextBox.DataContext = _models.GroupingModel;
            GroupingBoxBorderColorBorder.DataContext = _models.GroupingModel;
            GroupingGroupingFontSizeTextBox.DataContext = _models.GroupingModel;
            GroupingPaddingTextBox.DataContext = _models.GroupingModel;

            VoteChoicesDataGrid.DataContext = _models.VoteModel;
            VoteDurationSlider.DataContext = _models.VoteModel;
            VoteDurationLabel.DataContext = _models.VoteModel;
            VoteBoxForegroundColorTextBox.DataContext = _models.VoteModel;
            VoteBoxForegroundColorBorder.DataContext = _models.VoteModel;
            VoteBoxBackgroundColorTextBox.DataContext = _models.VoteModel;
            VoteBoxBackgroundColorBorder.DataContext = _models.VoteModel;
            VoteBoxBorderColorTextBox.DataContext = _models.VoteModel;
            VoteBoxBorderColorBorder.DataContext = _models.VoteModel;
            VoteFontSizeTextBox.DataContext = _models.VoteModel;
            VotePaddingTextBox.DataContext = _models.VoteModel;

            WordCloudMessageLimitTextBox.DataContext = _models.WordCloudModel;
            WordCloudFontMaxSizeTextBox.DataContext = _models.WordCloudModel;
            WordCloudFontMinSizeTextBox.DataContext = _models.WordCloudModel;
            WordCloudWidthTextBox.DataContext = _models.WordCloudModel;
            WordCloudHeightTextBox.DataContext = _models.WordCloudModel;
            WordCloudImageBackgroundColorTextBox.DataContext = _models.WordCloudModel;
            WordCloudImageBackgroundColorBorder.DataContext = _models.WordCloudModel;
            WordCloudFontColorsDataGrid.DataContext = _models.WordCloudModel;         
        }

        private void LoadConfigFile(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "config file (*.conf)|*.conf";
            if (dialog.ShowDialog() == true)
            {
                ConfigFile.Text = dialog.FileName;
                string json = System.IO.File.ReadAllText(ConfigFile.Text);
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                try
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Models));
                    Models newModels = ser.ReadObject(ms) as Models;
                    _models.Update(newModels);
                }
                catch (System.Runtime.Serialization.SerializationException ex)
                {
                    Debug.Print("Deserialization error: " + ex.ToString());
                }
                finally
                {
                    ms.Close();
                }
            }
        }

        private void SaveConfigFile(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "config file (*.conf)|*.conf";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ConfigFile.Text = dialog.FileName;
                    using (var ms = new MemoryStream())
                    using (var sr = new StreamReader(ms))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Models));
                        serializer.WriteObject(ms, _models);
                        ms.Position = 0;
                        string json = sr.ReadToEnd();
                        File.WriteAllText(ConfigFile.Text, json);
                    }
                }
                catch (System.Runtime.Serialization.SerializationException ex)
                {
                    Debug.Print("Serialization error: " + ex.ToString());
                }
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                using (var ms = new MemoryStream())
                using (var sr = new StreamReader(ms))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Models));
                    serializer.WriteObject(ms, _models);
                    ms.Position = 0;
                    string json = sr.ReadToEnd();
                    Properties.Settings.Default.lastConfig = json;
                    Properties.Settings.Default.Save();
                }
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                Debug.Print("Serialization error: " + ex.ToString());
            }
        }

        private void WatchMessagesAddWatchMessageClick(object sender, EventArgs e)
        {
            if (WatchMessageTextBox.Text == null || WatchMessageTextBox.Text == "")
            {
                return;
            }
            _models.WatchMessagesModel.WatchMessages.Add(new WatchMessage() { Message = WatchMessageTextBox.Text, Active = true, Author = "" });
            WatchMessageTextBox.Text = "";
        }
        private void WatchMessagesRemoveWatchMessageClick(object sender, EventArgs e)
        {
            if (WatchMessagesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _models.WatchMessagesModel.WatchMessages.Remove(_models.WatchMessagesModel.WatchMessages[WatchMessagesDataGrid.SelectedIndex]);
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
                _models.WatchMessagesModel.MediaFile = dialog.FileName;
            }
        }

        private void WatchMessagesTestMedia(object sender, EventArgs e)
        {
            if (WatchMessagesMediaTextBox.Text == null | WatchMessagesMediaTextBox.Text == "") {
                return;
            }
            MediaTestWindow window = new MediaTestWindow(_models.CommonModel, _models.WatchMessagesModel);
            window.Show();
        }

        private void WatchMessagesStart(object sender, EventArgs e)
        {

            WatchMessagesWindow window = new WatchMessagesWindow(_models.CommonModel, _models.WatchMessagesModel);
            window.Show();
            window.Start();
        }

        private void RandomChoiceSelectMedia(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "media file (*.*)|*.*";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                _models.RandomChoiceModel.MediaFile = dialog.FileName;
            }
        }

        private void RandomChoiceTestMedia(object sender, EventArgs e)
        {
            if (RandomChoiceMediaTextBox.Text == null | RandomChoiceMediaTextBox.Text == "")
            {
                return;
            }
            MediaTestWindow window = new MediaTestWindow(_models.CommonModel, _models.RandomChoiceModel);
            window.Show();
        }

        private void RandomChoiceStart(object sender, EventArgs e)
        {
            RandomChoiceWindow window = new RandomChoiceWindow(_models.CommonModel, _models.RandomChoiceModel);
            window.Show();
            window.Start();
        }

        private void GroupingAddChoiceClick(object sender, EventArgs e)
        {
            if (GroupingChoiceTextBox.Text == null || GroupingChoiceTextBox.Text == "")
            {
                return;
            }
            _models.GroupingModel.GroupingChoices.Add(new GroupingChoice() { Text = GroupingChoiceTextBox.Text });
            GroupingChoiceTextBox.Text = "";
        }

        private void GroupingRemoveChoiceClick(object sender, EventArgs e)
        {
            if (GroupingChoicesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _models.GroupingModel.GroupingChoices.Remove(_models.GroupingModel.GroupingChoices[GroupingChoicesDataGrid.SelectedIndex]);
            GroupingChoicesDataGrid.SelectedIndex = -1;
        }

        private void GroupingStart(object sender, EventArgs e)
        {
            if (_models.GroupingModel.GroupingChoices.Count == 0)
            {
                return;
            }
            GroupingWindow window = new GroupingWindow(_models.CommonModel, _models.GroupingModel);
            window.Show();
            window.Start();
        }

        private void VoteAddChoiceClick(object sender, EventArgs e)
        {
            if (VoteChoiceTextBox.Text == null || VoteChoiceTextBox.Text == "")
            {
                return;
            }
            _models.VoteModel.VoteChoices.Add(new VoteChoice() { Text = VoteChoiceTextBox.Text});
            VoteChoiceTextBox.Text = "";
        }

        private void VoteRemoveChoiceClick(object sender, EventArgs e)
        {
            if (VoteChoicesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _models.VoteModel.VoteChoices.Remove(_models.VoteModel.VoteChoices[VoteChoicesDataGrid.SelectedIndex]);
            VoteChoicesDataGrid.SelectedIndex = -1;
        }

        private void VoteStart(object sender, EventArgs e)
        {
            if (_models.VoteModel.VoteChoices.Count == 0)
            {
                return;
            }
            VoteWindow window = new VoteWindow(_models.CommonModel, _models.VoteModel);
            window.Show();
            window.Start();
        }

        private void WordCloudAddFontColorClick(object sender, EventArgs e)
        {
            if (WordCloudFontColorTextBox.Text == null || WordCloudFontColorTextBox.Text == "")
            {
                return;
            }
            _models.WordCloudModel.FontColors.Add(new FontColor() { Color = WordCloudFontColorTextBox.Text });
            WatchMessageTextBox.Text = "";
        }

        private void WordCloudRemoveFontColorClick(object sender, EventArgs e)
        {
            if (WordCloudFontColorsDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _models.WordCloudModel.FontColors.Remove(_models.WordCloudModel.FontColors[WordCloudFontColorsDataGrid.SelectedIndex]);
            WordCloudFontColorsDataGrid.SelectedIndex = -1;
        }

        private void WordCloudStart(object sender, EventArgs e)
        {
            if (_models.WordCloudModel.FontColors.Count == 0)
            {
                return;
            }
            WordCloudWindow window = new WordCloudWindow(_models.CommonModel, _models.WordCloudModel);
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
