using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private CommonModel _commonModel;
        private RandomChoiceModel _randomChoiceModel;
        private YlccProtocol protocol = new YlccProtocol();
        private CancellationTokenSource cancelSource;
        private CancellationToken cancelToken;


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

            WatchMessagesMediaElement.Width = randomChoiceModel.MediaWidth;
            WatchMessagesMediaElement.Height = randomChoiceModel.MediaHeight;

            dColor = System.Drawing.ColorTranslator.FromHtml(randomChoiceModel.LabelForeground);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            AuthorLabel.Foreground = new SolidColorBrush(mColor);
            AuthorLabel.FontSize = randomChoiceModel.LabelFontSize;
            MessageLabel.Foreground = new SolidColorBrush(mColor);
            MessageLabel.FontSize = randomChoiceModel.LabelFontSize;
        }

        public async void Start()
        {
            cancelSource = new CancellationTokenSource();
            cancelToken = cancelSource.Token;
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                StartCollectionActiveLiveChatRequest startCollectionActiveLiveChatRequest = protocol.BuildStartCollectionActiveLiveChatRequest(_commonModel.VideoId);
                StartCollectionActiveLiveChatResponse startCollectionActiveLiveChatResponse = await client.StartCollectionActiveLiveChatAsync(startCollectionActiveLiveChatRequest, cancellationToken: cancelToken);
                if (startCollectionActiveLiveChatResponse.Status.Code != Code.Success && startCollectionActiveLiveChatResponse.Status.Code != Code.InProgress)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + startCollectionActiveLiveChatResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    this.Close();
                    return;
                }
                PollActiveLiveChatRequest pollActiveLiveChatRequest = protocol.BuildPollActiveLiveChatRequest(_commonModel.VideoId);
                AsyncServerStreamingCall<PollActiveLiveChatResponse> call = client.PollActiveLiveChat(pollActiveLiveChatRequest, cancellationToken: cancelToken);
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
                        this.Close();
                        break;
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


                        // XXXXXX

                    }
                }
                await channel.ShutdownAsync();
            }
            catch (Grpc.Core.RpcException ex)
            {
                if (!cancelToken.IsCancellationRequested)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + ex.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    this.Close();
                }
            }
            finally
            {
                cancelSource.Dispose();
            }
        }

        private void getMessage()
        {
            if (!(_randomChoiceModel.MediaFile == null || _randomChoiceModel.MediaFile == ""))
            {
                WatchMessagesMediaElement.Source = null;
            }
            AuthorLabel.Content = "";
            MessageLabel.Content = "";



        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!cancelToken.IsCancellationRequested)
            {
                cancelSource.Cancel();
            }
        }

    }
}
