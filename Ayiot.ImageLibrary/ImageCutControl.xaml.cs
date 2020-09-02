using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ayiot.ImageLibrary
{
    /// <summary>
    /// ResizeImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageCutControl : UserControl
    {
        public ImageCutControl()
        {
            InitializeComponent();
            //this.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            //也可以直接在XAML中使用Background="#00FFFFFF"
        }

        #region 私有属性
        /// <summary>  
        /// 鼠标位置  
        /// </summary>  
        private MouseLocationEnum MouseLocation = MouseLocationEnum.None;
        /// <summary>  
        /// 鼠标行为  
        /// </summary>  
        private MouseActionEx MouseAction { get; set; }
        /// <summary>  
        /// 拖拽前鼠标按下位置  
        /// </summary>  
        private Point MousePostion;
        /// <summary>  
        /// 拖拽前控件位置  
        /// </summary>  
        private Point ResizePostion;
        #endregion

        #region 公共属性
        /// <summary>
        /// 自动获取截图的内容；供后续调用
        /// </summary>
        public BitmapSource CutBitmapSource
        {
            get
            {
                BitmapSource cutBitmapSource = null;
                try
                {
                    if (!(ImageClipControl.Source is BitmapSource bitmapImg) || ImageClipControl.Clip == null)
                        return null;
                    cutBitmapSource = ImageHelper.GetCutImage(bitmapImg,
                        new Int32Rect((int)(ImageClipControl.Clip.Bounds.X / Zoom),
                            (int)(ImageClipControl.Clip.Bounds.Y / Zoom),
                            (int)(ImageClipControl.Clip.Bounds.Width / Zoom),
                            (int)(ImageClipControl.Clip.Bounds.Height / Zoom)));
                }
                catch (Exception e)
                {
                }
                return cutBitmapSource;
            }
        }
        #endregion

        #region 依赖属性

        #region 锁定比例
        private static double SourceHeight = 0;
        private static double SourceWidth = 0;
        public bool KeepRate
        {
            get { return (bool)GetValue(KeepRateProperty); }
            set { SetValue(KeepRateProperty, value); }
        }
        public static readonly DependencyProperty KeepRateProperty = DependencyProperty.Register("KeepRate",
            typeof(bool), typeof(ImageCutControl), new PropertyMetadata(false));
        #endregion

        #region 源图
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageCutControl),
            new PropertyMetadata(default(ImageSource), new PropertyChangedCallback(SourcePropertyChangedCallbak)));
        private static void SourcePropertyChangedCallbak(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is ImageCutControl icc)
            {
                icc.Zoom = 1;
                ImageSource imageSource = (ImageSource)arg.NewValue;
                BitmapImage bitmapImage = (BitmapImage)imageSource;
                SourceHeight = ImageHelper.GetPixel(bitmapImage.Height, bitmapImage.DpiY);
                SourceWidth = ImageHelper.GetPixel(bitmapImage.Width, bitmapImage.DpiX);
                icc.CutGridControl.Width = SourceWidth;
                icc.CutGridControl.Height = SourceHeight;
                icc.ImageClipControl.Source = (ImageSource)arg.NewValue;
                icc.ResetClip();
            }
        }
        #endregion

        #region 切图比例
        public double Rate
        {
            get { return (double)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }
        public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate",
            typeof(double), typeof(ImageCutControl), new PropertyMetadata(1.0,
            new PropertyChangedCallback(RatePropertyChangedCallbak)));
        private static void RatePropertyChangedCallbak(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is ImageCutControl icc)
            {
                if (icc.Source != null)
                    icc.ResetClip();
            }
        }
        #endregion

        #region 缩放比例
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom",
            typeof(double), typeof(ImageCutControl), new PropertyMetadata(1.0,
            new PropertyChangedCallback(ZoomPropertyChangedCallbak)));
        private static void ZoomPropertyChangedCallbak(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is ImageCutControl icc && icc.Source != null)
            {
                double newZoom = (double)arg.NewValue;
                double oldZoom = (double)arg.OldValue;
                icc.CutGridControl.Width = SourceWidth * newZoom;
                icc.CutGridControl.Height = SourceHeight * newZoom;
                icc.SetCutBorder(newZoom, oldZoom);
            }
        }

        private void SetCutBorder(double newZoom, double oldZoom)
        {
            var pointX = ResizeBorderControl.Margin.Left / oldZoom * newZoom;
            var pointY = ResizeBorderControl.Margin.Top / oldZoom * newZoom;
            var width = ResizeBorderControl.ActualWidth / oldZoom * newZoom;
            var height = ResizeBorderControl.ActualHeight / oldZoom * newZoom;
            RectangleGeometry rt = new RectangleGeometry(new Rect(pointX, pointY, width, height));
            ImageClipControl.Clip = rt;
            ResizeBorderControl.Width = width;
            ResizeBorderControl.Height = height;
            ResizeBorderControl.Margin = new Thickness(pointX, pointY, 0, 0);
        }
        #endregion

        private void ResetClip()
        {
            Size cutSize = ImageHelper.GetSize(new Size(CutGridControl.Width, CutGridControl.Height), Rate);
            var pointX = (CutGridControl.Width - cutSize.Width) / 2;
            var pointY = (CutGridControl.Height - cutSize.Height) / 2;
            ResizeBorderControl.Width = cutSize.Width;
            ResizeBorderControl.Height = cutSize.Height;
            ResizeBorderControl.Margin = new Thickness(pointX, pointY, 0, 0);
            RectangleGeometry rt = new RectangleGeometry(new Rect(pointX, pointY, cutSize.Width, cutSize.Height));
            ImageClipControl.Clip = rt;
        }

        #region 选择框外围透明度

        public double CutCoverOpacity
        {
            get { return (double)GetValue(CutCoverOpacityProperty); }
            set { SetValue(CutCoverOpacityProperty, value); }
        }

        public static readonly DependencyProperty CutCoverOpacityProperty =
            DependencyProperty.Register("CutCoverOpacity", typeof(double), typeof(ImageCutControl), new PropertyMetadata(0.5, new PropertyChangedCallback((sender, arg) =>
          {
              if (sender != null && sender is ImageCutControl icc)
              {
                  icc.ImageBackControl.Opacity = (double)arg.NewValue;
              }
          })));

        #endregion

        #endregion

        #region 私有事件
        private void CutGridControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseAction = MouseActionEx.None;
        }
        private void CutGridControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (CutGridControl.IsMouseOver)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //鼠标相对空间区域位置  
                    Point mousePoint = Mouse.GetPosition(this.CutGridControl);
                    Point resizePoint = this.ResizeBorderControl.TransformToAncestor(this.CutGridControl).Transform(new Point(0, 0));
                    if (this.MouseAction == MouseActionEx.Resize)
                    {
                        //裁剪区域理论位置  
                        CutRect rect = CalculatedArea(mousePoint, resizePoint);
                        if (rect != null)
                        {
                            if (rect.X < 0 || rect.Y < 0 || rect.Width < 0 || rect.Height < 0 || rect.Width > this.CutGridControl.ActualWidth || rect.Height > this.CutGridControl.ActualHeight || rect.X + rect.Width > this.CutGridControl.ActualWidth || rect.Y + rect.Height > this.CutGridControl.ActualHeight)
                                return;
                            ResizeBorderControl.Width = rect.Width;
                            ResizeBorderControl.Height = rect.Height;
                            ResizeBorderControl.SetValue(MarginProperty, new Thickness(rect.X, rect.Y, 0, 0));
                            RectangleGeometry rt = new RectangleGeometry(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
                            this.ImageClipControl.Clip = rt;
                        }
                    }
                    else if (this.MouseAction == MouseActionEx.DragMove)//拖动  
                    {
                        double Left = ResizePostion.X + (mousePoint.X - MousePostion.X);
                        double Top = ResizePostion.Y + (mousePoint.Y - MousePostion.Y);
                        Left = Left < 0 ? 0 : Left;
                        Top = Top < 0 ? 0 : Top;
                        if (Top + ResizeBorderControl.ActualHeight > CutGridControl.ActualHeight)
                            Top = CutGridControl.ActualHeight - ResizeBorderControl.ActualHeight;
                        if (Left + ResizeBorderControl.ActualWidth > CutGridControl.ActualWidth)
                            Left = CutGridControl.ActualWidth - ResizeBorderControl.ActualWidth;
                        this.ResizeBorderControl.SetValue(MarginProperty, new Thickness(Left, Top, 0, 0));
                        RectangleGeometry rt = new RectangleGeometry(new Rect(Left, Top, ResizeBorderControl.ActualWidth, ResizeBorderControl.ActualHeight));
                        ImageClipControl.Clip = rt;
                    }
                }
                else CalPostion();
            }
        }
        private void CalPostion()
        {
            this.MousePostion = Mouse.GetPosition(this.CutGridControl);
            this.ResizePostion = this.ResizeBorderControl.TransformToAncestor(this.CutGridControl).Transform(new Point(0, 0));
            if (MousePostion.X > ResizePostion.X - 1 && MousePostion.X <= ResizePostion.X + 6 &&
                MousePostion.Y >= ResizePostion.Y - 1 && MousePostion.Y < ResizePostion.Y + 6)
            {
                MouseLocation = MouseLocationEnum.LeftUp;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNWSE;
            }
            else if (MousePostion.X >= ResizePostion.X + ResizeBorderControl.ActualWidth - 6 && MousePostion.X <= ResizePostion.X + ResizeBorderControl.ActualWidth + 1 &&
                MousePostion.Y > ResizePostion.Y - 1 && MousePostion.Y <= ResizePostion.Y + 6)
            {
                MouseLocation = MouseLocationEnum.RightUp;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNESW;
            }
            else if (MousePostion.X > ResizePostion.X - 1 && MousePostion.X <= ResizePostion.X + 6 &&
                MousePostion.Y >= ResizePostion.Y + ResizeBorderControl.ActualHeight - 6 && MousePostion.Y < ResizePostion.Y + ResizeBorderControl.ActualHeight + 1)
            {
                MouseLocation = MouseLocationEnum.LeftDown;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNESW;
            }
            else if (MousePostion.X >= ResizePostion.X + ResizeBorderControl.ActualWidth - 6 && MousePostion.X < ResizePostion.X + ResizeBorderControl.ActualWidth + 1 &&
                MousePostion.Y >= ResizePostion.Y + ResizeBorderControl.ActualHeight - 6 && MousePostion.Y < ResizePostion.Y + ResizeBorderControl.ActualHeight + 1)
            {
                MouseLocation = MouseLocationEnum.RightDown;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNWSE;
            }

            else if (MousePostion.X > ResizePostion.X - 1 && MousePostion.X < ResizePostion.X + 2 &&
                MousePostion.Y > ResizePostion.Y + 6 && MousePostion.Y < ResizePostion.Y + ResizeBorderControl.ActualHeight - 6)
            {
                MouseLocation = MouseLocationEnum.Left;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeWE;
            }
            else if (MousePostion.X > ResizePostion.X + 6 && MousePostion.X < ResizePostion.X + ResizeBorderControl.ActualWidth - 6 &&
                MousePostion.Y > ResizePostion.Y - 1 && MousePostion.Y < ResizePostion.Y + 2)
            {
                MouseLocation = MouseLocationEnum.Up;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNS;
            }
            else if (MousePostion.X > ResizePostion.X + ResizeBorderControl.ActualWidth - 1 && MousePostion.X < ResizePostion.X + ResizeBorderControl.ActualWidth + 2 &&
                MousePostion.Y > ResizePostion.Y + 6 && MousePostion.Y < ResizePostion.Y + ActualHeight - 6)
            {
                MouseLocation = MouseLocationEnum.Right;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeWE;
            }
            else if (MousePostion.X > ResizePostion.X + 6 && MousePostion.X < ResizePostion.X + ResizeBorderControl.ActualWidth - 6 &&
                MousePostion.Y > ResizePostion.Y + ResizeBorderControl.ActualHeight - 1 && MousePostion.Y < ResizePostion.Y + ResizeBorderControl.ActualHeight + 2)
            {
                MouseLocation = MouseLocationEnum.Down;
                MouseAction = MouseActionEx.Resize;
                Cursor = Cursors.SizeNS;
            }
            else if (!(MousePostion.X > ResizePostion.X - 1 && MousePostion.X < ResizePostion.X + ResizeBorderControl.ActualWidth + 1 &&
                MousePostion.Y > ResizePostion.Y - 1 && MousePostion.Y < ResizePostion.Y + ResizeBorderControl.ActualHeight + 1))
            {
                MouseAction = MouseActionEx.None;
                MouseLocation = MouseLocationEnum.None;
                Cursor = Cursors.Arrow;
            }
            else
            {
                MouseAction = MouseActionEx.DragMove;
                MouseLocation = MouseLocationEnum.None;
                Cursor = Cursors.SizeAll;
            }
        }
        private void CutGridControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseAction = MouseActionEx.None;
            Cursor = Cursors.Arrow;
        }
        private CutRect CalculatedArea(Point mousePoint, Point resizePoint)
        {
            double resizeWidth = this.ResizeBorderControl.ActualWidth;
            double resizeHeight = this.ResizeBorderControl.ActualHeight;
            Point standardPoint, targetPoint;
            double targetWidth, targetHeight;
            switch (MouseLocation)
            {
                case MouseLocationEnum.Left:
                    {
                        standardPoint = new Point(resizePoint.X + resizeWidth, resizePoint.Y + resizeHeight / 2);//右中部位置  
                        targetWidth = standardPoint.X - mousePoint.X;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : resizeHeight;
                        targetPoint = new Point(standardPoint.X - targetWidth, standardPoint.Y - targetHeight / 2);
                    }
                    break;
                case MouseLocationEnum.Right:
                    {
                        standardPoint = new Point(resizePoint.X, resizePoint.Y + resizeHeight / 2);//左中部位置  
                        targetWidth = mousePoint.X - standardPoint.X;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : resizeHeight;
                        targetPoint = new Point(standardPoint.X, standardPoint.Y - targetHeight / 2);
                    }
                    break;
                case MouseLocationEnum.Up:
                    {
                        standardPoint = new Point(resizePoint.X + resizeWidth / 2, resizePoint.Y + resizeHeight);//下中部位置  
                        targetHeight = standardPoint.Y - mousePoint.Y;
                        targetWidth = KeepRate ? resizeWidth - (resizeHeight - targetHeight) * Rate : resizeWidth;
                        targetPoint = new Point(standardPoint.X - targetWidth / 2, standardPoint.Y - targetHeight);
                    }
                    break;
                case MouseLocationEnum.Down:
                    {
                        standardPoint = new Point(resizePoint.X + resizeWidth / 2, resizePoint.Y);//上中部位置  
                        targetHeight = mousePoint.Y - standardPoint.Y;
                        targetWidth = KeepRate ? resizeWidth - (resizeHeight - targetHeight) * Rate : resizeWidth;
                        targetPoint = new Point(standardPoint.X - targetWidth / 2, standardPoint.Y);
                    }
                    break;
                case MouseLocationEnum.LeftUp:
                    {
                        standardPoint = new Point(resizePoint.X + resizeWidth, resizePoint.Y + resizeHeight);//右下部位置  
                        targetWidth = standardPoint.X - mousePoint.X;
                        targetHeight = standardPoint.Y - mousePoint.Y;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : targetHeight;
                        targetPoint = new Point(standardPoint.X - targetWidth, standardPoint.Y - targetHeight);
                    }
                    break;
                case MouseLocationEnum.RightDown:
                    {
                        standardPoint = new Point(resizePoint.X, resizePoint.Y);//左上部位置  
                        targetWidth = mousePoint.X - standardPoint.X;
                        targetHeight = mousePoint.Y - standardPoint.Y;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : targetHeight;
                        targetPoint = new Point(standardPoint.X, standardPoint.Y);
                    }
                    break;
                case MouseLocationEnum.RightUp:
                    {
                        standardPoint = new Point(resizePoint.X, resizePoint.Y + resizeHeight);//左下部位置  
                        targetWidth = mousePoint.X - standardPoint.X;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : standardPoint.Y - mousePoint.Y;
                        targetPoint = new Point(standardPoint.X, standardPoint.Y - targetHeight);
                    }
                    break;
                case MouseLocationEnum.LeftDown:
                    {
                        standardPoint = new Point(resizePoint.X + resizeWidth, resizePoint.Y);//右上部位置  
                        targetWidth = standardPoint.X - mousePoint.X;
                        targetHeight = KeepRate ? resizeHeight - (resizeWidth - targetWidth) / Rate : mousePoint.Y - standardPoint.Y;
                        targetPoint = new Point(standardPoint.X - targetWidth, standardPoint.Y);
                    }
                    break;
                default:
                    return null;
            }
            return new CutRect { X = targetPoint.X, Y = targetPoint.Y, Width = targetWidth, Height = targetHeight };
        }
        #endregion
    }
}
