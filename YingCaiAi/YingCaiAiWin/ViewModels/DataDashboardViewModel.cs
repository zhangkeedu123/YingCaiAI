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