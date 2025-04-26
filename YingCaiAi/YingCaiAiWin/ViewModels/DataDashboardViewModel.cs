using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace YingCaiAiWin.ViewModels
{
    public class DataDashboardViewModel : INotifyPropertyChanged
    {
        private DateTime _lastUpdateTime;
        private decimal _totalBalance;
        private string _totalBalanceRate;
        private decimal _totalExpense;
        private string _totalExpenseRate;
        private decimal _totalSavings;
        private string _totalSavingsRate;
        private decimal _cardBalance;
        private string _cardNumber;
        private string _cardExpiry;
        private ChartValues<double> _totalBalanceData;
        private ChartValues<double> _totalExpenseData;
        private ChartValues<double> _totalSavingsData;
        private SeriesCollection _outlaySeriesCollection;
        private string[] _outlayLabels;
        private ObservableCollection<AnalyticsItem> _analyticsItems;
        private ObservableCollection<Transaction> _latestTransactions;

        public DataDashboardViewModel()
        {
            // 初始化模拟数据
            InitializeData();
        }

        private void InitializeData()
        {
            // 设置最后更新时间为当前时间
            LastUpdateTime = DateTime.Now;

            // 设置基本数据
            TotalBalance = 19750;
            TotalBalanceRate = "+11.94%";
            TotalExpense = 11375;
            TotalExpenseRate = "+19.91%";
            TotalSavings = 8400;
            TotalSavingsRate = "+21.17%";
            CardBalance = 19750;
            CardNumber = "2357 1649 4251 0380";
            CardExpiry = "2024";

            // 初始化图表数据
            InitializeChartData();

            // 初始化分析数据
            InitializeAnalyticsData();

            // 初始化最新交易数据
            InitializeTransactionData();
        }

        private void InitializeChartData()
        {
            // 总余额趋势数据
            TotalBalanceData = new ChartValues<double>
            {
                15000, 16200, 17500, 16800, 18200, 19750
            };

            // 总支出趋势数据
            TotalExpenseData = new ChartValues<double>
            {
                8500, 9200, 10500, 9800, 10800, 11375
            };

            // 总储蓄趋势数据
            TotalSavingsData = new ChartValues<double>
            {
                5000, 5500, 6200, 7000, 7800, 8400
            };

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

        private void InitializeAnalyticsData()
        {
            AnalyticsItems = new ObservableCollection<AnalyticsItem>
            {
                new AnalyticsItem { Name = "基础设施", Value = 85 },
                new AnalyticsItem { Name = "人工智能", Value = 75 },
                new AnalyticsItem { Name = "电子设备", Value = 65 },
                new AnalyticsItem { Name = "框架材料", Value = 60 },
                new AnalyticsItem { Name = "完成工作", Value = 55 },
                new AnalyticsItem { Name = "硬件", Value = 50 },
                new AnalyticsItem { Name = "框架", Value = 45 },
                new AnalyticsItem { Name = "瓷砖安装", Value = 40 },
                new AnalyticsItem { Name = "屋顶", Value = 35 },
                new AnalyticsItem { Name = "设备安装", Value = 30 },
                new AnalyticsItem { Name = "绝缘", Value = 25 },
                new AnalyticsItem { Name = "内部装修", Value = 20 }
            };
        }

        private void InitializeTransactionData()
        {
            LatestTransactions = new ObservableCollection<Transaction>
            {
                new Transaction
                {
                    Description = "AI模型训练费用",
                    Date = DateTime.Now.AddDays(-1),
                    Amount = -3201.34,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "购买服务器设备",
                    Date = DateTime.Now.AddDays(-3),
                    Amount = -5647.16,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "软件授权退款",
                    Date = DateTime.Now.AddDays(-5),
                    Amount = 568.11,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C9A7"))
                },
                new Transaction
                {
                    Description = "云服务费用",
                    Date = DateTime.Now.AddDays(-7),
                    Amount = -1250.00,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B8B"))
                },
                new Transaction
                {
                    Description = "AI服务收入",
                    Date = DateTime.Now.AddDays(-10),
                    Amount = 8500.00,
                    AmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C9A7"))
                }
            };
        }

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                _lastUpdateTime = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            set
            {
                _totalBalance = value;
                OnPropertyChanged();
            }
        }

        public string TotalBalanceRate
        {
            get => _totalBalanceRate;
            set
            {
                _totalBalanceRate = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalExpense
        {
            get => _totalExpense;
            set
            {
                _totalExpense = value;
                OnPropertyChanged();
            }
        }

        public string TotalExpenseRate
        {
            get => _totalExpenseRate;
            set
            {
                _totalExpenseRate = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalSavings
        {
            get => _totalSavings;
            set
            {
                _totalSavings = value;
                OnPropertyChanged();
            }
        }

        public string TotalSavingsRate
        {
            get => _totalSavingsRate;
            set
            {
                _totalSavingsRate = value;
                OnPropertyChanged();
            }
        }

        public decimal CardBalance
        {
            get => _cardBalance;
            set
            {
                _cardBalance = value;
                OnPropertyChanged();
            }
        }

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                _cardNumber = value;
                OnPropertyChanged();
            }
        }

        public string CardExpiry
        {
            get => _cardExpiry;
            set
            {
                _cardExpiry = value;
                OnPropertyChanged();
            }
        }

        public ChartValues<double> TotalBalanceData
        {
            get => _totalBalanceData;
            set
            {
                _totalBalanceData = value;
                OnPropertyChanged();
            }
        }

        public ChartValues<double> TotalExpenseData
        {
            get => _totalExpenseData;
            set
            {
                _totalExpenseData = value;
                OnPropertyChanged();
            }
        }

        public ChartValues<double> TotalSavingsData
        {
            get => _totalSavingsData;
            set
            {
                _totalSavingsData = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection OutlaySeriesCollection
        {
            get => _outlaySeriesCollection;
            set
            {
                _outlaySeriesCollection = value;
                OnPropertyChanged();
            }
        }

        public string[] OutlayLabels
        {
            get => _outlayLabels;
            set
            {
                _outlayLabels = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> YFormatter { get; set; } = value => value.ToString("C0");

        public ObservableCollection<AnalyticsItem> AnalyticsItems
        {
            get => _analyticsItems;
            set
            {
                _analyticsItems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Transaction> LatestTransactions
        {
            get => _latestTransactions;
            set
            {
                _latestTransactions = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}