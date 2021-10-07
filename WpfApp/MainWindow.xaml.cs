using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using VisualServoCore;
using VisualServoCore.Vision;
using VisualServoCore.Communication;
using VisualServoCore.Controller;

namespace WpfApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {

        // Primary processes
        private IDisposable _stream;
        private DummyCommunication _server;
        private ColorBasedController _controller;
        private BGRStream _video;
        private DataLogger<double> _log;

        // Temporary datas and flags
        private string _initDir;
        private int _visionSelectedIndex;
        private bool _logOn;
        private bool _recOn;
        private double _steer;
        private double _gain;
        private int _focusWidth;
        private int _maxWidth;
        private int _maxDistance;
        private readonly OpenCvSharp.Size _size = new(1280, 960);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            var cacheFile = "cache";
            try
            {
                using var sr = new StreamReader(cacheFile);
                _initDir = sr.ReadLine();
                _visionSelectedIndex = int.Parse(sr.ReadLine());
                _logOn = sr.ReadLine() is "LogOn" ? true : false;
                _recOn = sr.ReadLine() is "RecOn" ? true : false;
                _focusWidth = int.Parse(sr.ReadLine());
                _maxWidth = int.Parse(sr.ReadLine());
                _maxDistance = int.Parse(sr.ReadLine());
            }
            catch
            {
                _initDir = "C:";
                _visionSelectedIndex = 0;
                _gain = 1.0;
                _focusWidth = 1500;
                _maxWidth = 3000;
                _maxDistance = 8000;
            }
            SourceCombo.SelectedIndex = _visionSelectedIndex;
            LogCheck.IsChecked = _logOn;
            RecCheck.IsChecked = _recOn;
            GainText.Text = _gain.ToString();
            FocusWidthText.Text = _focusWidth.ToString();
            MaxWidthText.Text = _maxWidth.ToString();
            MaxDistanceText.Text = _maxDistance.ToString();
            if (!(bool)LogCheck.IsChecked)
                RecCheck.IsEnabled = false;

            Closed += (sender, args) =>
            {
                GC.Collect();
                _stream?.Dispose();
                using var sw = new StreamWriter(cacheFile);
                sw.WriteLine(_initDir);
                sw.WriteLine(_visionSelectedIndex);
                sw.WriteLine(_logOn ? "LogOn" : "LogOff");
                sw.WriteLine(_recOn ? "RecOn" : "RecOff");
                sw.WriteLine(_maxWidth);
                sw.WriteLine(_maxDistance);
            };
        }

        private void VehicleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_server is null)
            {
                VehicleButton.IsEnabled = false;
                Task.Run(() =>
                {
                    _server = new();
                    Dispatcher.Invoke(() =>
                    {
                        VehicleButton.IsEnabled = true;
                        VehicleButton.Background = Brushes.Red;
                    });
                    while (true)
                    {
                        try
                        {
                            Thread.Sleep(50);
                            _server?.Send(_steer);
                        }
                        catch
                        {
                            _server?.Dispose();
                            _server = null;
                            Dispatcher.Invoke(() => VehicleButton.Background = Brushes.CornflowerBlue);
                            break;
                        }
                    }
                });
            }
            else
            {
                _server?.Dispose();
                _server = null;
                VehicleButton.Background = Brushes.CornflowerBlue;
            }
        }

        private void VisionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_video is null)
            {
                try
                {
                    var gain = double.Parse(GainText.Text);
                    var focusWidth = int.Parse(FocusWidthText.Text);
                    var maxDistance = int.Parse(MaxDistanceText.Text);
                    var maxWidth = int.Parse(MaxWidthText.Text);
                    if (gain < 0) throw new();
                    if (maxDistance <= 0) throw new();
                    if (maxWidth <= 0) throw new();
                    if (focusWidth <= 0) throw new();
                    if (focusWidth > maxWidth) throw new();
                    _gain = gain;
                    _focusWidth = focusWidth;
                    _maxDistance = maxDistance;
                    _maxWidth = maxWidth;
                    _logOn = (bool)LogCheck.IsChecked;
                    _recOn = (bool)RecCheck.IsChecked;
                    _log = _logOn ? new(_recOn ? _size : null) : null;
                    _visionSelectedIndex = SourceCombo.SelectedIndex;
                }
                catch
                {
                    return;
                }
                _controller = new(_gain, _maxWidth, _maxDistance, _focusWidth);
                switch (SourceCombo.SelectedIndex)
                {
                    case 0:
                        var cofd = new CommonOpenFileDialog()
                        {
                            Title = "動画ファイルを選択",
                            InitialDirectory = _initDir,
                            IsFolderPicker = false,
                        };
                        if (cofd.ShowDialog() is CommonFileDialogResult.Ok)
                        {
                            _initDir = Path.GetDirectoryName(cofd.FileName);
                            _video = new(cofd.FileName);
                            _stream = _video.Connect()
                                .Subscribe(frame =>
                                {
                                    var view = frame.Clone();
                                    if (_recOn) _log?.Write(frame);
                                    var obj = _controller.Run(view);
                                    var radar = _controller.GetGroundCoordinateResults();
                                    _steer = obj.Steer;
                                    _log?.Write(obj);
                                    ProcessUserThread(view, radar);
                                });
                            VisionButton.Background = Brushes.Red;
                        }
                        cofd.Dispose();
                        break;
                    case 1:
                        _video = new(0);
                        _stream = _video.Connect()
                            .Subscribe(frame =>
                            {
                                var view = frame.Clone();
                                if (_recOn) _log?.Write(frame);
                                var obj = _controller.Run(view);
                                var radar = _controller.GetGroundCoordinateResults();
                                _steer = obj.Steer;
                                _log?.Write(obj);
                                ProcessUserThread(view, radar);
                            });
                        VisionButton.Background = Brushes.Red;
                        break;
                }
            }
            else
            {
                _log?.Dispose();
                _stream?.Dispose();
                _video?.Disconnect();
                _video = null;
                VisionButton.Background = Brushes.CornflowerBlue;
                GC.Collect();
            }
        }

        private void ProcessUserThread(Mat view, Mat radar)
        {
            Dispatcher.Invoke(() =>
            {
                LeftImage.Source = radar.ToBitmapSource();
                RightImage.Source = view.ToBitmapSource();
                SendCommandLabel.Content = $"{_steer:f1} deg";
            });
        }

        private void LogCheck_Checked(object sender, RoutedEventArgs e)
        {
            RecCheck.IsEnabled = true;
        }

        private void LogCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            RecCheck.IsEnabled = false;
        }

        private void RecCheck_Checked(object sender, RoutedEventArgs e)
        {
            LogCheck.IsEnabled = false;
        }

        private void RecCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            LogCheck.IsEnabled = true;
        }
    }
}
