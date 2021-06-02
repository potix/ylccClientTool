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
using System.Drawing;
using ylccProtocol;
using Grpc.Net.Client;
using Grpc.Core;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ylccClientTool
{
    /// <summary>
    /// WatchMessagesWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WatchMessagesWindow : Window
    {
        private CommonModel _commonModel;
        private WatchMessagesModel _watchMessagesModel;
        private YlccProtocol _protocol = new YlccProtocol();
        private CancellationTokenSource _cancelSource;
        private CancellationToken _cancelToken;
        private Thread _updateThread;
        private BlockingCollection<ActiveLiveChatMessage> _queue = new BlockingCollection<ActiveLiveChatMessage>();
        private bool _isCloseRequested = false;

        public WatchMessagesWindow(CommonModel commonModel, WatchMessagesModel watchMessagesModel)
        {
            InitializeComponent();
            _commonModel = commonModel;
            _watchMessagesModel = watchMessagesModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = watchMessagesModel.MediaWidth + 40;
            Height = watchMessagesModel.MediaHeight + 40;

            WatchMessagesMediaElement.Width = watchMessagesModel.MediaWidth;
            WatchMessagesMediaElement.Height = watchMessagesModel.MediaHeight;

            dColor = System.Drawing.ColorTranslator.FromHtml(watchMessagesModel.LabelForeground);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            AuthorLabel.Foreground = new SolidColorBrush(mColor);
            AuthorLabel.FontSize = watchMessagesModel.LabelFontSize;
            MessageLabel.Foreground = new SolidColorBrush(mColor);
            MessageLabel.FontSize = watchMessagesModel.LabelFontSize;
        }

        public async void Start()
        {
            _updateThread = new Thread(WindowUpdate);
            _updateThread.Start();

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
                        foreach (WatchMessage watchMessage in _watchMessagesModel.WatchMessages)
                        {
                            if (watchMessage.Active && activeLiveChatMessage.DisplayMessage.Contains(watchMessage.Message))
                            {
                                _queue.Add(activeLiveChatMessage);
                                watchMessage.Author = activeLiveChatMessage.AuthorDisplayName;
                                watchMessage.Active = false;
                                break;
                            }
                        }
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

        private void WindowUpdate()
        {
            while(true)
            {
                ActiveLiveChatMessage activeLiveChatMessage = _queue.Take();
                if (activeLiveChatMessage == null) {
                    break;
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (!(_watchMessagesModel.MediaFile == null || _watchMessagesModel.MediaFile == ""))
                    {
                        WatchMessagesMediaElement.Source = new Uri(_watchMessagesModel.MediaFile);
                        WatchMessagesMediaElement.Volume = _watchMessagesModel.Volume / 100;
                    }
                    AuthorLabel.Content = activeLiveChatMessage.AuthorDisplayName;
                    MessageLabel.Content = activeLiveChatMessage.DisplayMessage;
                }));
                Thread.Sleep(5000);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (!(_watchMessagesModel.MediaFile == null || _watchMessagesModel.MediaFile == ""))
                    {
                        WatchMessagesMediaElement.Source = null;
                    }
                    AuthorLabel.Content = "";
                    MessageLabel.Content = "";
                }));
            }
        }
        private void WindowClose()
        {
            _isCloseRequested = true;
            this.Close();
        }
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            _queue.Add(null);
            if (!_isCloseRequested && _cancelSource != null && !_cancelToken.IsCancellationRequested) { 
                _cancelSource.Cancel();
            }
        }

    }
}
