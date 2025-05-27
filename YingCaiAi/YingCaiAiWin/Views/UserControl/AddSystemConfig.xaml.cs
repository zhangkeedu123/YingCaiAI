using DocumentFormat.OpenXml.Office2013.Drawing.Chart;
using HandyControl.Controls;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using YingCaiAiModel;
using YingCaiAiService.IService;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// AddSystemConfig.xaml 的交互逻辑
    /// </summary>
    public partial class AddSystemConfig : ContentDialog
    {
        public Documents DocumentsEdit { get; set; }

        private IDocumentsService _service;
        private static readonly Regex _validRegex = new Regex("^[a-zA-Z0-9]+$");
        private bool obj = true;
        public AddSystemConfig(ContentPresenter? contentPresenter, Documents document, IDocumentsService service)
            : base(contentPresenter)
        {
            _service = service;
            InitializeComponent();
            Title = "编辑/添加信息";
            this.PrimaryButtonText = "保存";
            CloseButtonText = "关闭";
            DocumentsEdit = document;
            DataContext = this;
            Load();
        }

        private async void Load()
        {
            Filename.Text = DocumentsEdit.Filename;
            Content.Text = DocumentsEdit.Content;
        }

        protected async override void OnButtonClick(ContentDialogButton button)
        {


            if (button == ContentDialogButton.Close)
            {
                base.OnButtonClick(button);
                return;
            }
            if (obj == false)
            {
                return;
            }
            obj = false;
            if (string.IsNullOrWhiteSpace(Filename.Text.Trim()))
            {
                ErrorInfoBar.Message = "类别不能为空！请重新输入!";
                ErrorInfoBar.IsOpen = true;

                return;
            }
            if (string.IsNullOrWhiteSpace(Content.Text.Trim()))
            {
                ErrorInfoBar.Message = "内容不能为空！请重新输入!";
                ErrorInfoBar.IsOpen = true;
                return;
            }
            if (DocumentsEdit.Id == null || DocumentsEdit.Id < 1)
            {
                var temp = await _service.GetByFileNameAsync(Filename.Text.Trim());
                if (temp != null && temp.Id > 0)
                {
                    ErrorInfoBar.Message = "已存在相同类名！";
                    ErrorInfoBar.IsOpen = true;
                    return;

                }
                DocumentsEdit.Status = 5;
                DocumentsEdit.CreatedAt = DateTime.Now;
            }

            DocumentsEdit.Filename = Filename.Text.Trim();
            DocumentsEdit.Content = Content.Text.Trim();

            if (DocumentsEdit.Id != null && DocumentsEdit.Id > 0)
            {
                var flag = await _service.UpdateAsync(DocumentsEdit);
                if (flag.Status)
                {
                    Growl.Success("操作成功");
                }
                else
                {
                    Growl.Error("操作失败！");
                }
            }
            else
            {

                var list = new List<Documents>
                {
                    DocumentsEdit
                };
                var flag = await _service.AddListAsync(list);
                if (flag.Status)
                {
                    Growl.Success("操作成功");
                }
                else
                {
                    Growl.Error("操作失败！");
                }

            }

            await Task.Delay(2000);
            Growl.Clear();

            obj = true;
            base.OnButtonClick(button);
            return;


        }
    }
}
