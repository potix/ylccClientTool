using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ylccProtocol;

namespace ylccClientTool
{
    /// <summary>
    /// GroupingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupingWindow : Window
    {
        private readonly YlccProtocol _protocol = new YlccProtocol();
        private CommonModel _commonModel;
        private GroupingModel _groupingModel;
        private CancellationTokenSource _cancelSource;
        private CancellationToken _cancelToken;
        private Collection<TextBox> _textBoxes = new Collection<TextBox>();
        private bool _isCloseRequested = false;

        public GroupingWindow(CommonModel commonModel, GroupingModel groupingModel)
        {
            InitializeComponent();

            _commonModel = commonModel;
            _groupingModel = groupingModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = groupingModel.Width + 20;
            Height = groupingModel.Height + 20;
        }

        public async void Start()
        {
            int boxWidth = (_groupingModel.Width - ((_groupingModel.GroupingChoices.Count + 1) * _groupingModel.Padding)) / _groupingModel.GroupingChoices.Count;
            int LabelBoxHeight = _groupingModel.FontSize + _groupingModel.Padding;
            int ChoiceBoxHeight = (_groupingModel.FontSize * 3) + _groupingModel.Padding;
            int MessageBoxHeight = _groupingModel.Height - LabelBoxHeight - ChoiceBoxHeight - (_groupingModel.Padding * 2);
            int posX = _groupingModel.Padding;
            for (int idx = 0; idx < _groupingModel.GroupingChoices.Count; idx += 1)
            {
                _renderLabelBox(boxWidth, LabelBoxHeight, posX, _groupingModel.Padding, idx);
                _renderChoiceBox(boxWidth, ChoiceBoxHeight, posX, _groupingModel.Padding + LabelBoxHeight, idx);
                _textBoxes.Add(_renderMessageBox(boxWidth, MessageBoxHeight, posX, _groupingModel.Padding + LabelBoxHeight + ChoiceBoxHeight, idx));
                posX += boxWidth + _groupingModel.Padding;
            }
            _cancelSource = new CancellationTokenSource();
            _cancelToken = _cancelSource.Token;
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                Collection<ylccProtocol.GroupingChoice> choices = new Collection<ylccProtocol.GroupingChoice>();
                foreach (var choiceItem in _groupingModel.GroupingChoices)
                {
                    int idx = _groupingModel.GroupingChoices.IndexOf(choiceItem);
                    choices.Add(_protocol.BuildGroupingCoice((idx + 1).ToString(), choiceItem.Text));
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                StartGroupingActiveLiveChatRequest startGroupingActiveLiveChatRequest = _protocol.BuildStartGroupingActiveLiveChatRequest(_commonModel.VideoId, _commonModel.TargetValue.Target, choices);
                StartGroupingActiveLiveChatResponse startGroupingActiveLiveChatResponse = await client.StartGroupingActiveLiveChatAsync(startGroupingActiveLiveChatRequest, cancellationToken: _cancelToken);
                if (startGroupingActiveLiveChatResponse.Status.Code != Code.Success && startGroupingActiveLiveChatResponse.Status.Code != Code.InProgress)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + startGroupingActiveLiveChatResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    WindowClose();
                    return;
                }
                PollGroupingActiveLiveChatRequest pollGroupingActiveLiveChatRequest = _protocol.BuildPollGroupingActiveLiveChatRequest(startGroupingActiveLiveChatResponse.GroupingId);
                AsyncServerStreamingCall<PollGroupingActiveLiveChatResponse> call = client.PollGroupingActiveLiveChat(pollGroupingActiveLiveChatRequest, cancellationToken: _cancelToken);
                while (await call.ResponseStream.MoveNext())
                {
                    PollGroupingActiveLiveChatResponse response = call.ResponseStream.Current;
                    if (response.Status.Code != Code.Success)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("通信エラー\n");
                        sb.Append("URI:" + _commonModel.Uri + "\n");
                        sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                        sb.Append("Reason:" + response.Status.Message + "\n");
                        MessageBox.Show(sb.ToString());
                        WindowClose();
                        return;
                    }
                    if (response.GroupingActiveLiveChatMessage != null)
                    {
                        // delete custom emoji
                        string noCustomDisplayMessage = System.Text.RegularExpressions.Regex.Replace(response.GroupingActiveLiveChatMessage.ActiveLiveChatMessage.DisplayMessage, ":[^:]+?:", "").Trim();
                        if (noCustomDisplayMessage == "")
                        {
                            continue;
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append(noCustomDisplayMessage);
                        sb.Append(" (");
                        sb.Append(response.GroupingActiveLiveChatMessage.ActiveLiveChatMessage.AuthorDisplayName);
                        sb.Append(")");
                        sb.Append("\n");
                        _textBoxes[response.GroupingActiveLiveChatMessage.GroupIdx].Text = sb.ToString() + _textBoxes[response.GroupingActiveLiveChatMessage.GroupIdx].Text;
                    }
                }
                await channel.ShutdownAsync();
                WindowClose();
            }
            catch (Grpc.Core.RpcException ex)
            {
                if (!_cancelToken.IsCancellationRequested)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + ex.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    WindowClose();
                    return;
                }
            }
            finally
            {
                _cancelSource.Dispose();
            }
        }

        private void _renderLabelBox(int width, int height, int posX, int posY, int idx)
        {
            TextBox textBox = new TextBox();
            textBox.Text = (idx + 1).ToString() + ".";
            textBox.FontSize = _groupingModel.FontSize;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(posX, posY, 0, 0);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Background = new SolidColorBrush(mColor);
            dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.Width = width;
            textBox.Height = height;
            textBox.TextWrapping = TextWrapping.Wrap;
            ViewGrid.Children.Add(textBox);
        }

        private void _renderChoiceBox(int width, int height, int posX, int posY, int idx)
        {
            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, "Text");
            textBox.DataContext = _groupingModel.GroupingChoices[idx];
            textBox.FontSize = _groupingModel.FontSize;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(posX, posY, 0, 0);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Background = new SolidColorBrush(mColor);
            dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.Width = width;
            textBox.Height = height;
            textBox.TextWrapping = TextWrapping.Wrap;
            ViewGrid.Children.Add(textBox);
        }

        private TextBox _renderMessageBox(int width, int height, int posX, int posY, int idx)
        {
            TextBox textBox = new TextBox();
            textBox.Text = "";
            textBox.FontSize = _groupingModel.FontSize;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(posX, posY, 0, 0);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Background = new SolidColorBrush(mColor);
            dColor = System.Drawing.ColorTranslator.FromHtml(_groupingModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.Width = width;
            textBox.Height = height;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.IsReadOnly = true;
            ViewGrid.Children.Add(textBox);
            return textBox;
        }

        private void WindowClose()
        {
            _isCloseRequested = true;
            Close();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!_isCloseRequested && _cancelSource != null && !_cancelToken.IsCancellationRequested)
            {
                _cancelSource.Cancel();
            }
        }
    }
}
