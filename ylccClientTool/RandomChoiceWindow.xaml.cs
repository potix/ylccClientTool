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
    /// RandomChoiceWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RandomChoiceWindow : Window
    {
        private readonly YlccProtocol _protocol = new YlccProtocol();
        private CommonModel _commonModel;
        private RandomChoiceModel _randomChoiceModel;
        private CancellationTokenSource _cancelSource;
        private CancellationToken _cancelToken;
        private object _lock = new object();
        private bool _getMessageRequest;
        private Random _rand = new Random();
        private bool _isCloseRequested = false;

        public RandomChoiceWindow(CommonModel commonModel, RandomChoiceModel randomChoiceModel)
        {
            InitializeComponent();

            _commonModel = commonModel;
            _randomChoiceModel = randomChoiceModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = randomChoiceModel.MediaWidth + 40;
            Height = randomChoiceModel.MediaHeight + 40 + 100;

            RandomChoiceMediaElement.Width = randomChoiceModel.MediaWidth;
            RandomChoiceMediaElement.Height = randomChoiceModel.MediaHeight;

            dColor = System.Drawing.ColorTranslator.FromHtml(randomChoiceModel.LabelForeground);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            AuthorLabel.Foreground = new SolidColorBrush(mColor);
            AuthorLabel.FontSize = randomChoiceModel.LabelFontSize;
            MessageLabel.Foreground = new SolidColorBrush(mColor);
            MessageLabel.FontSize = randomChoiceModel.LabelFontSize;
        }

        public async void Start()
        {
            _cancelSource = new CancellationTokenSource();
            _cancelToken = _cancelSource.Token;
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                StartCollectionActiveLiveChatRequest startCollectionActiveLiveChatRequest = _protocol.BuildStartCollectionActiveLiveChatRequest(_commonModel.VideoId);
                StartCollectionActiveLiveChatResponse startCollectionActiveLiveChatResponse = await client.StartCollectionActiveLiveChatAsync(startCollectionActiveLiveChatRequest, cancellationToken: _cancelToken);
                if (startCollectionActiveLiveChatResponse.Status.Code != Code.Success && startCollectionActiveLiveChatResponse.Status.Code != Code.InProgress)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + startCollectionActiveLiveChatResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    WindowClose();
                    return;
                }
                PollActiveLiveChatRequest pollActiveLiveChatRequest = _protocol.BuildPollActiveLiveChatRequest(_commonModel.VideoId);
                AsyncServerStreamingCall<PollActiveLiveChatResponse> call = client.PollActiveLiveChat(pollActiveLiveChatRequest, cancellationToken: _cancelToken);
                while (await call.ResponseStream.MoveNext())
                {
                    PollActiveLiveChatResponse response = call.ResponseStream.Current;
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
                    if (response.ActiveLiveChatMessages == null)
                    {
                        continue;
                    }
                    Collection<ActiveLiveChatMessage> candidateMessages = new Collection<ActiveLiveChatMessage>();
                    foreach (ActiveLiveChatMessage activeLiveChatMessage in response.ActiveLiveChatMessages)
                    {
                        if (_commonModel.TargetValue.Target == Target.OwnerModerator && !activeLiveChatMessage.AuthorIsChatModerator)
                        {
                            continue;
                        }
                        if (_commonModel.TargetValue.Target == Target.OwnerModeratorSponsor && !(activeLiveChatMessage.AuthorIsChatModerator || activeLiveChatMessage.AuthorIsChatSponsor))
                        {
                            continue;
                        }
                        // delete cutom emoji
                        activeLiveChatMessage.DisplayMessage = System.Text.RegularExpressions.Regex.Replace(activeLiveChatMessage.DisplayMessage, ":[^:]+?:", "").Trim();
                        if (activeLiveChatMessage.DisplayMessage == "")
                        {
                            continue;
                        }
                        candidateMessages.Add(activeLiveChatMessage);
                    }
                    if (candidateMessages.Count == 0)
                    {
                        continue;
                    }
                    var getMessageRequest = false;
                    lock (_lock)
                    {
                        getMessageRequest = _getMessageRequest;
                        _getMessageRequest = false;
                    }
                    if (getMessageRequest)
                    {
                        int idx = _rand.Next(candidateMessages.Count);
                        WindowUpdate(candidateMessages[idx]);
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
                }
            }
            finally
            {
                _cancelSource.Dispose();
            }
        }

        private void WindowUpdate(ActiveLiveChatMessage activeLiveChatMessage)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (!(_randomChoiceModel.MediaFile == null || _randomChoiceModel.MediaFile == ""))
                {
                    RandomChoiceMediaElement.Source = new Uri(_randomChoiceModel.MediaFile);
                    RandomChoiceMediaElement.Volume = _randomChoiceModel.Volume / 100;
                }
                AuthorLabel.Content = activeLiveChatMessage.AuthorDisplayName;
                MessageLabel.Content = activeLiveChatMessage.DisplayMessage;
            }));
        }

        private void GetMessage(object sender, EventArgs e)
        {
            if (!(_randomChoiceModel.MediaFile == null || _randomChoiceModel.MediaFile == ""))
            {
                RandomChoiceMediaElement.Source = null;
            }
            AuthorLabel.Content = "";
            MessageLabel.Content = "";
            lock (_lock)
            {
                _getMessageRequest = true;
            }
        }
        private void WindowClose()
        {
            _isCloseRequested = true;
            this.Close();
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
