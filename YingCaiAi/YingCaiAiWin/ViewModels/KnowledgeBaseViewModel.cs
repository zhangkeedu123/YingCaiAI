using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui;
using YingCaiAiModel;
using YingCaiAiService.IService;
using YingCaiAiWin.Helpers;
using YingCaiAiWin.Views.Pages;

namespace YingCaiAiWin.ViewModels
{

    public partial class KnowledgeBaseViewModel : ViewModel
    {
        private bool _isInitialized = false;
        private ObservableCollection<UserInfo> _allUsers;
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private Visibility _openedFilePathVisibility = Visibility.Collapsed;
            [ObservableProperty]
    private string _openedFilePath = string.Empty;

        FileHelper _fileHelper=new FileHelper() ;
        public KnowledgeBaseViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
            {

                InitializeViewModel();
            }

        }
        private void InitializeViewModel()
        {
              LoadSampleData();
              _isInitialized = true;
        }

        [RelayCommand]
        private void OnAddUsers()
        {
            _isInitialized = true;
        }

        [RelayCommand]
        public void OnOpenFile()
        {
            OpenedFilePathVisibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "支持的文件 (*.doc;*.docx;*.xls;*.xlsx;*.txt)|*.doc;*.docx;*.xls;*.xlsx;*.txt",
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(openFileDialog.FileName))
            {
                _fileHelper.FileReadAll(openFileDialog.FileName);

            }

            OpenedFilePath = openFileDialog.FileName;
            OpenedFilePathVisibility = Visibility.Visible;
        }


        private void LoadSampleData()
        {
            // 创建一些假数据用于展示
            _allUsers = new ObservableCollection<UserInfo>
            {
                new UserInfo
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "系统管理员",
                    Email = "admin@example.com",
                    Department = "IT部门",
                    Position = "系统管理员",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    LastLogin = DateTime.Now.AddHours(-2),

                },
                new UserInfo
                {
                    Id = 2,
                    Username = "manager",
                    FullName = "张经理",
                    Email = "manager@example.com",
                    Department = "人事部",
                    Position = "部门经理",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-25),
                    LastLogin = DateTime.Now.AddDays(-1),

                },
                new UserInfo
                {
                    Id = 3,
                    Username = "user1",
                    FullName = "李四",
                    Email = "user1@example.com",
                    Department = "市场部",
                    Position = "市场专员",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    LastLogin = DateTime.Now.AddDays(-3),

                },
                new UserInfo
                {
                    Id = 4,
                    Username = "user2",
                    FullName = "王五",
                    Email = "user2@example.com",
                    Department = "财务部",
                    Position = "会计",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    LastLogin = DateTime.Now.AddDays(-2),

                },
                new UserInfo
                {
                    Id = 5,
                    Username = "guest",
                    FullName = "访客用户",
                    Email = "guest@example.com",
                    Department = "外部",
                    Position = "访客",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    LastLogin = DateTime.Now.AddDays(-2),

                },
                new UserInfo
                {
                    Id = 6,
                    Username = "developer",
                    FullName = "张开发",
                    Email = "dev@example.com",
                    Department = "IT部门",
                    Position = "开发工程师",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-18),
                    LastLogin = DateTime.Now.AddDays(-1),

                },
                new UserInfo
                {
                    Id = 7,
                    Username = "tester",
                    FullName = "李测试",
                    Email = "test@example.com",
                    Department = "IT部门",
                    Position = "测试工程师",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    LastLogin = DateTime.Now.AddHours(-5),

                },
                new UserInfo
                {
                    Id = 8,
                    Username = "marketing",
                    FullName = "王营销",
                    Email = "marketing@example.com",
                    Department = "市场部",
                    Position = "营销经理",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-22),
                    LastLogin = DateTime.Now.AddDays(-2),

                },
                new UserInfo
                {
                    Id = 9,
                    Username = "finance",
                    FullName = "赵财务",
                    Email = "finance@example.com",
                    Department = "财务部",
                    Position = "财务主管",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-28),
                    LastLogin = DateTime.Now.AddDays(-1),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                },
                new UserInfo
                {
                    Id = 10,
                    Username = "disabled",
                    FullName = "禁用账户",
                    Email = "disabled@example.com",
                    Department = "人事部",
                    Position = "前员工",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    LastLogin = DateTime.Now.AddDays(-30),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                },
                new UserInfo
                {
                    Id = 11,
                    Username = "disabled",
                    FullName = "禁用账222户",
                    Email = "disabled@example.com",
                    Department = "人事部",
                    Position = "前员工",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    LastLogin = DateTime.Now.AddDays(-30),
                    //Roles = new List<Role> { new Role { Id = 3, Name = "普通用户" } }
                }
            };
        }
    }
}
