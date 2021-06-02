using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// WordCloudWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WordCloudWindow : Window
    {
        private readonly YlccProtocol _protocol = new YlccProtocol();
        private CommonModel _commonModel;
        private WordCloudModel _wordCloudModel;
        private CancellationTokenSource _cancelSource;
        private CancellationToken _cancelToken;
        private bool _isClosed = false;
        private bool _isCloseRequested = false;


        public WordCloudWindow(CommonModel commonModel, WordCloudModel wordCloudModel)
        {
            InitializeComponent();

            _commonModel = commonModel;
            _wordCloudModel = wordCloudModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            Width = wordCloudModel.Width + 40;
            Height = wordCloudModel.Height + 40;

            WordCloudImage.Width = wordCloudModel.Width + 20;
            WordCloudImage.Height = wordCloudModel.Height + 20;
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
                StartCollectionWordCloudMessagesRequest startCollectionWordCloudMessagesRequest = _protocol.BuildStartCollectionWordCloudMessagesRequest(_commonModel.VideoId);
                StartCollectionWordCloudMessagesResponse startCollectionWordCloudMessagesResponse = await client.StartCollectionWordCloudMessagesAsync(startCollectionWordCloudMessagesRequest, cancellationToken: _cancelToken);
                if (startCollectionWordCloudMessagesResponse.Status.Code != Code.Success && startCollectionWordCloudMessagesResponse.Status.Code != Code.InProgress)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + startCollectionWordCloudMessagesResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    this.Close();
                    return;
                }
                while (!_isClosed)
                {
                    GetWordCloudRequest getWordCloudRequest = _protocol.BuildGetWordCloudRequest(
                        _commonModel.VideoId,
                        _commonModel.TargetValue.Target,
                        _wordCloudModel.MessageLimit,
                        _wordCloudModel.Width,
                        _wordCloudModel.Height,
                        _wordCloudModel.FontMaxSize,
                        _wordCloudModel.FontMinSize,
                        _wordCloudModel.GetFontColors(),
                        _wordCloudModel.GetBackgroundColor());
                    // Debug.Print("request:" + getWordCloudRequest.ToString());
                    GetWordCloudResponse getWordCloudResponse = await client.GetWordCloudAsync(getWordCloudRequest, cancellationToken:_cancelToken);
                    if (getWordCloudResponse.Status.Code != Code.Success && getWordCloudResponse.Status.Code != Code.InProgress)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("通信エラー\n");
                        sb.Append("URI:" + _commonModel.Uri + "\n");
                        sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                        sb.Append("Reason:" + getWordCloudResponse.Status.Message + "\n");
                        MessageBox.Show(sb.ToString());
                        this.Close();
                        return;
                    }
                    if (getWordCloudResponse.Status.Code == Code.InProgress)
                    {
                        await Task.Delay(5000);
                        continue;
                    }
                    if (getWordCloudResponse.MimeType == "image/png")
                    {
                        SetPngImage(getWordCloudResponse.Data.ToByteArray());
                    }
                    await Task.Delay(5000);
                }
                await channel.ShutdownAsync();
                WindowClose();
            }
            catch (Exception e)
            {
                if (!_cancelToken.IsCancellationRequested)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + e.Message + "\n");
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

        private void SetPngImage(byte[] pngImageData)
        {
            using (var mem = new MemoryStream(pngImageData))
            {
                mem.Position = 0;
                BitmapSource bitmapSource = BitmapFrame.Create(mem, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                WordCloudImage.Source = bitmapSource;
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
            _isClosed = true;
        }
    }
}
