using Grpc.Net.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
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
    /// VoteWindow.xaml の相互作用ロジック
    /// </summary>

    public class VoteResult : INotifyPropertyChanged
    {
        private int count;
        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
                OnPropertyChanged("Count");
            }
        }

        private double rate;
        public double Rate
        {
            get
            {
                return this.rate;
            }
            set
            {
                this.rate = value;
                OnPropertyChanged("Rate");
            }
        }

        private string rateStr;
        public string RateStr
        {
            get
            {
                return this.rateStr;
            }
            set
            {
                this.rateStr = value;
                OnPropertyChanged("RateStr");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class VoteWindow : Window, INotifyPropertyChanged
    {

        private readonly YlccProtocol _protocol = new YlccProtocol();
        private readonly int _minutes = 60;
        private CommonModel _commonModel;
        private VoteModel _voteModel;
        private int _maxCols;
        private int _boxWidth;
        private int _boxHeight;
        private string _voteId;
        private System.Timers.Timer _timer;

        private ObservableCollection<VoteResult> _voteResults = new ObservableCollection<VoteResult>();

        private int total;
        public int Total
        {
            get
            {
                return this.total;
            }
            set
            {
                this.total = value;
                OnPropertyChanged("Total");
            }
        }

        private int countDown;
        public int CountDown
        {
            get
            {
                return this.countDown;
            }
            set
            {
                this.countDown = value;
                OnPropertyChanged("CountDown");
            }
        }

        public VoteWindow(CommonModel commonModel, VoteModel voteModel)
        {
            InitializeComponent();
            CountDownLabel.DataContext = this;

            _commonModel = commonModel;
            _voteModel = voteModel;

            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(commonModel.WindowBackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            _maxCols = 4;
            if (voteModel.VoteChoices.Count <= 2)
            {
                _maxCols = 2;
            }
            else if (voteModel.VoteChoices.Count <= 3)
            {
                _maxCols = 3;
            }
            else if (voteModel.VoteChoices.Count <= 4)
            {
                _maxCols = 2;
            }
            else if (voteModel.VoteChoices.Count <= 9)
            {
                _maxCols = 3;
            }
            _boxWidth = 0;
            _boxHeight = 0;
            int rows = ((voteModel.VoteChoices.Count - 1) / _maxCols) + 1;
            foreach (var choiceItem in voteModel.VoteChoices)
            {
                string[] liners = choiceItem.Text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                int height = ((liners.Length + 4) * voteModel.FontSize) + (voteModel.Padding * 2);
                if (_boxHeight < height)
                {
                    _boxHeight = height;
                }
                foreach (var liner in liners)
                {
                    int width = (liner.Length * voteModel.FontSize) + (voteModel.Padding * 2);
                    if (_boxWidth < width)
                    {
                        _boxWidth = width;
                    }
                }
            }
            int windowWidth = (_boxWidth * _maxCols) + (voteModel.Padding * (_maxCols - 1)) + (voteModel.Padding * 2);
            int windowHeight = (_boxHeight * rows) + (voteModel.Padding * (rows - 1)) + (voteModel.Padding * 2);
            Width = windowWidth + voteModel.Padding;
            Height = windowHeight + voteModel.Padding + 100;
        }

        public async void Start()
        {
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                Collection<ylccProtocol.VoteChoice> choices = new Collection<ylccProtocol.VoteChoice>();
                foreach (var voteChoice in _voteModel.VoteChoices)
                {
                    int idx = _voteModel.VoteChoices.IndexOf(voteChoice);
                    choices.Add(_protocol.BuildVoteCoice((idx + 1).ToString(), voteChoice.Text));
                }
                OpenVoteRequest openVoteRequest = _protocol.BuildOpenVoteRequest(_commonModel.VideoId, _commonModel.TargetValue.Target, _voteModel.Duration * _minutes, choices);
                OpenVoteResponse openVoteResponse = await client.OpenVoteAsync(openVoteRequest);
                if (openVoteResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("Reason:" + openVoteResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    Close();
                    return;
                }
                _voteId = openVoteResponse.VoteId;
                CountDown = _voteModel.Duration * 60;
                Total = 0;
                for (int i = 0; i < _voteModel.VoteChoices.Count; i += 1)
                {
                    _voteResults.Add(new VoteResult() { Count = 0, Rate = 0, RateStr = "" });
                }
                _render();
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += (s, e) => {
                    if (CountDown > 0)
                    {
                        CountDown -= 1;
                    }
                };
                _timer.Start();
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + _commonModel.Uri + "\n");
                sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
                Close();
                return;
            }

        }

        private async void UpdateDurationClick(object sender, EventArgs e)
        {
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                UpdateVoteDurationRequest updateVoteDurationRequest = _protocol.BuildUpdateVoteDurationRequest(_voteId, _voteModel.Duration * _minutes);
                UpdateVoteDurationResponse updateVoteDurationResponse = await client.UpdateVoteDurationAsync(updateVoteDurationRequest);
                if (updateVoteDurationResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("VoteId:" + _voteId + "\n");
                    sb.Append("Reason:" + updateVoteDurationResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    Close();
                    return;
                }
                CountDown = _voteModel.Duration * 60;
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + _commonModel.Uri + "\n");
                sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                sb.Append("VoteId:" + _voteId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
                Close();
                return;
            }
        }

        private async void GetResultClick(object sender, EventArgs e)
        {
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                GetVoteResultRequest getVoteResultRequest = _protocol.BuildGetVoteResultRequest(_voteId);
                GetVoteResultResponse getVoteResultResponse = await client.GetVoteResultAsync(getVoteResultRequest);
                if (getVoteResultResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("VoteId:" + _voteId + "\n");
                    sb.Append("Reason:" + getVoteResultResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    Close();
                    return;
                }
                Total = getVoteResultResponse.Total;
                if (Total == 0)
                {
                    return;
                }
                int idx = 0;
                foreach (VoteCount count in getVoteResultResponse.Counts)
                {
                    VoteResult result = _voteResults[idx];
                    result.Count = count.Count;
                    result.Rate = Math.Ceiling((double)count.Count * 100.0 / (double)Total * 10) / 10;
                    result.RateStr = result.Rate.ToString() + "%";
                    idx += 1;
                }
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + _commonModel.Uri + "\n");
                sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                sb.Append("VoteId:" + _voteId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
                Close();
                return;
            }
        }

        private async void WindowClosing(object sender, CancelEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            try
            {
                if (_commonModel.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_commonModel.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                CloseVoteRequest closeVoteRequest = _protocol.BuildCloseVoteRequest(_voteId);
                CloseVoteResponse closeVoteResponse = await client.CloseVoteAsync(closeVoteRequest);
                if (closeVoteResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _commonModel.Uri + "\n");
                    sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                    sb.Append("VoteId:" + _voteId + "\n");
                    sb.Append("Reason:" + closeVoteResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                }
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + _commonModel.Uri + "\n");
                sb.Append("VideoId:" + _commonModel.VideoId + "\n");
                sb.Append("VoteId:" + _voteId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
            }
        }

        private void _render()
        {
            int idx = 0;
            foreach (VoteChoice choice in _voteModel.VoteChoices)
            {
                VoteResult result = _voteResults[idx];
                int rowPos = idx / _maxCols;
                int colPos = idx % _maxCols;
                _renderBackgroudBox(rowPos, colPos);
                _renderIndexBox(idx, rowPos, colPos);
                _renderResultBox(result, rowPos, colPos);
                _renderChoiceBox(choice, rowPos, colPos);
                idx += 1;
            }
        }
        private void _renderBackgroudBox(int rowPos, int colPos)
        {
            Border border = new Border();
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_voteModel.BoxBorderColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            border.BorderBrush = new SolidColorBrush(mColor);
            border.BorderThickness = new Thickness(5, 5, 5, 5);
            border.CornerRadius = new CornerRadius(10);
            dColor = System.Drawing.ColorTranslator.FromHtml(_voteModel.BoxBackgroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            border.Background = new SolidColorBrush(mColor);
            border.Width = _boxWidth;
            border.Height = _boxHeight;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.VerticalAlignment = VerticalAlignment.Top;
            border.Margin = new Thickness((_boxWidth * colPos) + (_voteModel.Padding * colPos) + _voteModel.Padding, (_boxHeight * rowPos) + (_voteModel.Padding * rowPos) + _voteModel.Padding, 0, 0);
            MainGrid.Children.Add(border);
        }

        private void _renderIndexBox(int idx, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.Text = (idx + 1).ToString() + ".";
            textBox.FontSize = _voteModel.FontSize * 1.5;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness((_boxWidth * colPos) + (_voteModel.Padding * colPos) + _voteModel.Padding, (_boxHeight * rowPos) + (_voteModel.Padding * rowPos) + _voteModel.Padding + 4, 0, 0);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_voteModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.Width = _boxWidth;
            textBox.Height = _boxHeight;
            textBox.IsReadOnly = true;
            MainGrid.Children.Add(textBox);
        }

        private void _renderResultBox(VoteResult result, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, "RateStr");
            textBox.DataContext = result;
            textBox.FontSize = _voteModel.FontSize * 1.5;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness((_boxWidth * colPos) + (_voteModel.Padding * colPos) + _voteModel.Padding, (_boxHeight * rowPos) + (_voteModel.Padding * rowPos) + _voteModel.Padding - 8, 0, 0);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_voteModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Bottom;
            textBox.Width = _boxWidth;
            textBox.Height = _boxHeight;
            textBox.IsReadOnly = true;
            MainGrid.Children.Add(textBox);
        }

        private void _renderChoiceBox(VoteChoice choice, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, "Text");
            textBox.DataContext = choice;
            textBox.FontSize = _voteModel.FontSize;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness((_boxWidth * colPos) + (_voteModel.Padding * colPos) + _voteModel.Padding, (_boxHeight * rowPos) + (_voteModel.Padding * rowPos) + _voteModel.Padding, 0, 0);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(_voteModel.BoxForegroundColor);
            mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.Width = _boxWidth;
            textBox.Height = _boxHeight;
            MainGrid.Children.Add(textBox);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
