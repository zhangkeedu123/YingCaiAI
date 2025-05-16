using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
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
        public DataDashboardViewModel(ITrainingDataService trainingDataService)
        {
            _trainingDataService = trainingDataService;
            // 初始化模拟数据
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
           // TotalBalanceRate = "+11.94%";
            TotalExpense = (decimal)dataList.LastOrDefault().TrainTotal;
            //TotalExpenseRate = "+19.91%";
            TotalSavings = (decimal)dataList.LastOrDefault().DocumentTotal;
            //TotalSavingsRate = "+21.17%";
            CardBalance = 1975;

            // 初始化图表数据
            InitializeChartData();


            // 初始化最新交易数据
            InitializeTransactionData();
        }

        private async void InitializeChartData()
        {
           
            // 支出趋势图表数据
            OutlaySeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "上月支出",
                    Values = new ChartValues<double> { 2000, 2500, 3000, 3500, 2800, 2200, 2700, 3200, 3800, 4200, 3700, 3000, 2500, 2800, 3200, 3600, 4000, 4200, 3800, 3500, 3200, 2800, 2500, 2200, 2000, 1800, 2200, 2500, 2800, 3000 },
                    PointGeometry = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C92F4")),
                    Fill = new SolidColorBrush(Color.FromArgb(50, 26, 115, 232))
                },
                new LineSeries
                {
                    Title = "当月支出",
                    Values = new ChartValues<double> { 1800, 2200, 2600, 3000, 3400, 3800, 4200, 4600, 4200, 3800, 3400, 3000, 2600, 2200, 1800, 2200, 2600, 3000, 3400, 3800, 4200, 4600, 4200, 3800, 3400, 3000, 2600, 2200, 1800, 2000 },
                    PointGeometry = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF85A2")),
                    Fill = new SolidColorBrush(Color.FromArgb(50, 255, 75, 139))
                }
            };

            // X轴标签
            OutlayLabels = new string[30];
            for (int i = 0; i < 30; i++)
            {
                OutlayLabels[i] = (i + 1).ToString();
            }
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



     

        public Func<double, string> YFormatter { get; set; } = value => value.ToString("C0");

 
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
}