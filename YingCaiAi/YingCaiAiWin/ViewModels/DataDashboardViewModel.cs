using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using NPOI.Util;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiWin.ViewModels
{
    public partial class DataDashboardViewModel :  ViewModel
    {
        [ObservableProperty]
        private DateTime _lastUpdateTime;

        [ObservableProperty]
        private decimal _totalBalance;
        [ObservableProperty]
        private string _totalBalanceRate;
        [ObservableProperty]
        private decimal _totalExpense;
        [ObservableProperty]
        private string _totalExpenseRate;
        [ObservableProperty]
        private decimal _totalSavings;
        [ObservableProperty]
        private string _totalSavingsRate;
        [ObservableProperty]
        private decimal _cardBalance;

  
        [ObservableProperty]
        private ChartValues<double> _totalBalanceData;
        [ObservableProperty]
        private ChartValues<double> _totalExpenseData;
        [ObservableProperty]
        private ChartValues<double> _totalSavingsData;
        [ObservableProperty]
        private SeriesCollection _outlaySeriesCollection;
        [ObservableProperty]
        private string[] _outlayLabels;
        [ObservableProperty]
        private List<Transaction> _latestTransactions = [];

        private readonly ITrainingDataService _trainingDataService;
        private readonly DispatcherTimer _timer;
        private readonly HttpClient _httpClient = new HttpClient();

        [ObservableProperty]
        private ObservableCollection<ISeries> _cpuSeries  =[];

        [ObservableProperty]
        private List<ISeries> _gpuSeries  = [new LineSeries<double> { Name = "GPU 0", Values = new List<double>() { } }, new LineSeries<double> { Name = "GPU 1", Values = new List<double>() }];

  
        [ObservableProperty]
        public LiveChartsCore.SkiaSharpView.Axis[] _xAxes = { new LiveChartsCore.SkiaSharpView.Axis { LabelsRotation = 0,Name="time", Labels =new List<string> { "1","2","3","4","5","6", "1", "2", "3", "4", "5", "6", "1", "2", "3", "4", "5", "6", "1", "2", "3", "4", "5", "6" } } };

        [ObservableProperty]
        public LiveChartsCore.SkiaSharpView.Axis [] _yAxes  = { new LiveChartsCore.SkiaSharpView.Axis { LabelsRotation = 0, MinLimit = 0, MaxLimit =100 } };

        public DataDashboardViewModel(ITrainingDataService trainingDataService)
        {
            _trainingDataService = trainingDataService;
            // 初始化模拟数据
            CpuSeries.Add(new LineSeries<double> { Name = "CPU", Values = new ObservableCollection<double>() });

          
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); 
            _timer.Tick += async (s, e) => await RefreshData();
            _timer.Start();
            InitializeData();
        }

        private async void InitializeData()
        {
            // 设置最后更新时间为当前时间
            LastUpdateTime = DateTime.Now;
            var data = await _trainingDataService.GetBigDataSum();
            var dataList = data.Data as List<BigDataSum> ?? new List<BigDataSum>();
            // 总余额趋势数据
            TotalBalanceData = new ChartValues<double>(dataList.Select(s => s.CustomerTotal));

            // 总支出趋势数据
            TotalExpenseData = new ChartValues<double>(dataList.Select(s => s.TrainTotal));

            // 总储蓄趋势数据
            TotalSavingsData = new ChartValues<double>(dataList.Select(s => s.DocumentTotal));
            // 设置基本数据
            TotalBalance = (decimal)dataList.LastOrDefault().CustomerTotal;
         
            TotalExpense = (decimal)dataList.LastOrDefault().TrainTotal;
        
            TotalSavings = (decimal)dataList.LastOrDefault().DocumentTotal;
   
            CardBalance = 1975;

            // 初始化图表数据
            InitializeChartData();


            // 初始化最新交易数据
            InitializeTransactionData();
        }

        private async void InitializeChartData()
        {
           
             var data = await _trainingDataService.GetAiSumAsync();
            var dataList = data.Data as List<BigDataSum> ?? new List<BigDataSum>();
            // 支出趋势图表数据
            OutlaySeriesCollection = new SeriesCollection
            {
                
                new ColumnSeries
                {
                    Title = "AI记录",
                    Values = new ChartValues<double>(dataList.Select(s => s.TrainTotal)),
                    PointGeometry = null,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87CEEB")),
                    Fill = new SolidColorBrush(Color.FromArgb(125, 135, 206, 235))
                }
            };

            // X轴标签
            OutlayLabels = dataList.Select(s=> DateTime.Parse(s.StatDate).Day.ToString() ).ToArray();


        }

     

        private async void InitializeTransactionData()
        {

            var data = await _trainingDataService.GetBigDataToDo();
            var dataList = data.Data as List<BigDataSum> ?? new List<BigDataSum>();

            LatestTransactions = new List<Transaction>()
            {
                new Transaction
                {
                    Description = "待审核知识",
                    Amount = dataList.FirstOrDefault().DocumentTotal,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "待审核话术",
                    Amount = dataList.FirstOrDefault().TrainTotal,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "待联系客户",
                    Amount = dataList.FirstOrDefault().CustomerTotal,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "待训练话术",
                    Amount = dataList.FirstOrDefault().TrainToTotal,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C9A7"))
                },
                new Transaction
                {
                    Description = "待入库知识",
                    Amount = dataList.FirstOrDefault().DocumentsToTotal,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C9A7"))
                }
            };
        }



     

        public Func<double, string> YFormatter { get; set; } = value => value.ToString("0");

      

        private async Task RefreshData()
        {
            try
            {
                var date = DateTime.Now.Second;

                var da = XAxes[0].Labels;
                da.RemoveAt(0);
                da.Add(date.ToString());
               
                XAxes=[new LiveChartsCore.SkiaSharpView.Axis { LabelsRotation = 5, Labels = da, Position = LiveChartsCore.Measure.AxisPosition.Start,Name = "time" }];

                var response = await _httpClient.GetAsync("http://113.105.116.171:8000/system/status");
                var content = await response.Content.ReadAsStringAsync();
                var status = JsonSerializer.Deserialize<SystemStatusData>(content);

            
                    // 更新 CPU 曲线
                    var cpuSeries =CpuSeries.Copy();
                    var cpuValues = (ObservableCollection<double>)cpuSeries[0].Values;
                    cpuValues.Add(status.cpu.memory_used_GB);
                    if (cpuValues.Count > 30) cpuValues.RemoveAt(0);

                     CpuSeries = cpuSeries;

                   // 更新 GPU 曲线
                  var  gpu1value=  (List<double>)GpuSeries[0].Values;
                   var gpu2value =  (List<double>)GpuSeries[1].Values;

                
                gpu1value.Add(status.gpus[0].utilization_percent);
                gpu2value.Add(status.gpus[1].utilization_percent);
                if (gpu1value.Count > 30) {
                    gpu1value.RemoveAt(0);
                    gpu2value.RemoveAt(0);
                }

                    
               
               GpuSeries =  [new LineSeries<double> { Name = "GPU 0", Values = gpu1value,  }, new LineSeries<double> { Name = "GPU 1", Values = gpu2value }  ]; 


            }
            catch (Exception ex)
            {
                
            }
        }
    }

    public class AnalyticsItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class Transaction
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public Brush AmountColor { get; set; }
    }

    // 接口响应数据类
    public class SystemStatusData
    {
        public CpuInfo cpu { get; set; }
        public GpuInfo[] gpus { get; set; }
    }
    public class CpuInfo
    {
        public double cpu_percent { get; set; }
        public double memory_used_GB { get; set; }

        public double memory_total_GB { get; set; }
    }
    public class GpuInfo
    {
        public int index { get; set; }
        public string name { get; set; }
        public double utilization_percent { get; set; }

        public double memory_used_GB { get; set; }

        public double memory_total_GB { get; set; }
    }
}