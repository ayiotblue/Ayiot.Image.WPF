using Ayiot.ImageLibrary;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AyiotImageTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ResetSlider_click(object sender, RoutedEventArgs e)
        {
            this.ZoomSlider.Value = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ZoomSlider.Value = 1;
            this.CutImageRate.SelectedIndex = 1;
        }

        private void ImageCutControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JPG图片|*.jpg",
                DefaultExt = ".jpg",
                AddExtension = true
            };
            if (dialog.ShowDialog() == true)
            {
                var cutSource = ImageCutControl.CutBitmapSource;
                JpegBitmapEncoder jpgE = new JpegBitmapEncoder();
                jpgE.Frames.Add(BitmapFrame.Create(cutSource));
                using (Stream stream = File.Create(dialog.FileName))
                {
                    jpgE.QualityLevel = 100;
                    jpgE.Save(stream);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectFolderTextBlock.Text = dialog.FileName;
            }
        }

        private void SelectFolderTextBlock_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string path = selectFolderTextBlock.Text;
            var imgs = Directory.GetFiles(path, "*.jpg");
            List<ImageInfo> list = new List<ImageInfo>();
            foreach (var item in imgs)
            {
                list.Add(new ImageInfo
                {
                    File = item,
                    Name = Path.GetFileNameWithoutExtension(item),
                });
            }
            DirListBox.ItemsSource = list;
        }
    }
}
