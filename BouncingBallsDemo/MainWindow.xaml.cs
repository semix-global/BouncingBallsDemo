using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BouncingBallsDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SKMatrix viewMatrix = SKMatrix.MakeIdentity();
        private Point lastPoint;
        public SKTypeface Font;
        public BouncingBalls bouncingBalls;

        public MainWindow()
        {
            InitializeComponent();
            bouncingBalls = new BouncingBalls();
            skContainer.PaintSurface += SkContainer_PaintSurface;
            skContainer.MouseWheel += SkContainer_MouseWheel;
            skContainer.MouseMove += SkContainer_MouseMove;
            skContainer.MouseDown += SkContainer_MouseDown;
            skContainer.MouseUp += SkContainer_MouseUp;
            // 获取宋体在字体集合中的下标
            var index = SKFontManager.Default.FontFamilies.ToList().IndexOf("微软雅黑");
            // 创建宋体字形
            Font = SKFontManager.Default.GetFontStyles(index).CreateTypeface(0);
            // _ = Task.Run(() =>
            // {
            //     while (true)
            //     {
            //         try
            //         {
            //             Dispatcher.Invoke(() => { skContainer.InvalidateVisual(); });
            //             SpinWait.SpinUntil(() => false, 1000 / 60);
            //         }
            //         catch
            //         {
            //             break;
            //         }
            //     }
            // });
        }


        private void SkContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取滚轮滚动的增量
            float delta = e.Delta;

            // 计算缩放因子
            float scaleFactor = delta > 0 ? 1.1f : 0.9f;

            // 获取当前鼠标位置
            Point point = e.GetPosition(skContainer);

            // 更新视图矩阵
            viewMatrix = viewMatrix.PostConcat(SKMatrix.MakeScale(scaleFactor, scaleFactor, (float)point.X, (float)point.Y));

            // 重新绘制
            skContainer.InvalidateVisual();
        }

        private void SkContainer_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果鼠标左键被按下，则进行移动操作
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(skContainer);
                SKPoint delta = new SKPoint((float)(point.X - lastPoint.X), (float)(point.Y - lastPoint.Y));

                viewMatrix = viewMatrix.PostConcat(SKMatrix.MakeTranslation(delta.X, delta.Y));

                lastPoint = point;

                skContainer.InvalidateVisual();
            }
        }

        private void SkContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 记录鼠标按下时的位置
            lastPoint = e.GetPosition(skContainer);
        }

        private void SkContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 清空记录的位置
            lastPoint = new Point();
        }

        private void SkContainer_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            // 应用视图矩阵
            canvas.Concat(ref viewMatrix);

            // 创建绘制参数和矩形对象
            var paint = new SKPaint { Color = SKColors.LightBlue };
            var rect = new SKRect();

            // 芯片中间的间隔
            const int dX = 15;
            const int dY = 15;

            // 视野显示宽度高度
            const int viewShowWidthPx = 30;
            const int viewShowHeightPx = 30;

            // 开始绘制
            canvas.Clear(SKColors.White); // 清空画布

            for (var row = 0; row < 1000; row++)
            {
                for (var col = 0; col < 1000; col++)
                {
                    // 计算矩形的坐标
                    var xView = col * viewShowWidthPx;
                    var yView = row * viewShowHeightPx;
                    var xCell1 = xView + dX;
                    var yCell1 = yView + dY;
                    var xCell2 = xView + viewShowWidthPx;
                    var yCell2 = yView + viewShowHeightPx;

                    // 设置矩形的位置
                    rect.Left = xCell1;
                    rect.Top = yCell1;
                    rect.Right = xCell2;
                    rect.Bottom = yCell2;

                    // 绘制矩形
                    canvas.DrawRect(rect, paint);
                }
            }

            // 完成绘制
            canvas.Flush();
        }
    }
}