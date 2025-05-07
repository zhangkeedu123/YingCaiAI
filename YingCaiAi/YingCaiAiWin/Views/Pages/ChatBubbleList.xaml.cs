using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YingCaiAiWin.Views.Pages
{
    /// <summary>
    /// ChatBubbleList.xaml 的交互逻辑
    /// </summary>
    public partial class ChatBubbleList : UserControl
    {
        private Border _loadingBubble;
        public ChatBubbleList()
        {
            InitializeComponent();
        }

        public void AddMessage(string text, bool isUser)
        {
        

            var textBox = new TextBox
            {
                Text = text,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Foreground = Brushes.Black,
                Cursor = Cursors.IBeam
            };

            var border = new Border
            {
                Background = isUser
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f4f5f5"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f2f5ff")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(3),
                Margin = new Thickness(3),
                MaxWidth = 250,
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Child = textBox
            };

            MessagePanel.Children.Add(border);
            
        }
     

        /// <summary>
        /// 添加一个“占位气泡”，里面显示加载动画
        /// </summary>
        /// <returns></returns>
        public void AddLoadingBubble()
        {
            var loadingText = new TextBlock
            {
                Text = "正在思考",
                Foreground = Brushes.Gray,
                FontSize = 14,
                Margin = new Thickness(0, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var textBlock = new TextBlock
            {
                FontSize = 16,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.Gray,
                Text = ".", // 初始值
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            var animation = new StringAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromSeconds(1.5),
                RepeatBehavior = RepeatBehavior.Forever
            };

            animation.KeyFrames.Add(new DiscreteStringKeyFrame(".", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            animation.KeyFrames.Add(new DiscreteStringKeyFrame("..", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
            animation.KeyFrames.Add(new DiscreteStringKeyFrame("...", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            animation.KeyFrames.Add(new DiscreteStringKeyFrame("", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));

            Storyboard.SetTarget(animation, textBlock);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TextBlock.TextProperty));

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
            //// 小圆圈动画
            //var loadingRing = new System.Windows.Shapes.Ellipse
            //{
            //    Width = 12,
            //    Height = 12,
            //    Fill = Brushes.Gray,
            //    Margin = new Thickness(2, 0, 0, 0),
            //    VerticalAlignment = VerticalAlignment.Center
            //};

            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(loadingText);
            stack.Children.Add(textBlock);

            _loadingBubble = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f2f5ff")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(6),
                Margin = new Thickness(3),
                MaxWidth = 250,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = stack
            };

            MessagePanel.Children.Add(_loadingBubble);
          
        }

        /// <summary>
        /// 替换为真实回答内容
        /// </summary>
        /// <param name="answerText"></param>
        public void ReplaceLoadingBubble(string answerText)
        {
            if (_loadingBubble != null)
            {
                var textBox = new TextBox
                {
                    Text = answerText,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    Foreground = Brushes.Black,
                    Cursor = Cursors.IBeam
                };

                _loadingBubble.Child = textBox;
                _loadingBubble = null; // 清理引用，防止误用
            }
        }
    }

}
