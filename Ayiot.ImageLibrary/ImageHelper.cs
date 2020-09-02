/*
 title
    图片裁剪类（帮助类）
 desc
    认知信息：保存图片
                     
    行为职责：保存图片到文件
                     获取裁剪后图片大小
 createtime
    2013/8/23 17:06
 creator
    ycl
 modify
    ayiotblue
 ---------------------------------------------------------------------
 * 1)
 * 2)
 * 3)
 * ……
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;


public class ImageHelper
{
    /// <summary>
    /// 获取选中比例的大小
    /// </summary>
    /// <param name="oriSize"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    public static Size GetSize(Size oriSize, double rate)
    {
        Size newsize = new Size();
        if (oriSize.Width / oriSize.Height > rate)
        {
            newsize.Height = oriSize.Height;
            newsize.Width = newsize.Height * rate;
        }
        else
        {
            newsize.Width = oriSize.Width;
            newsize.Height = newsize.Width / rate;
        }
        return newsize;
    }

    /// <summary>
    /// 获取指定大小的ImageSource
    /// </summary>
    /// <param name="bitImg">元素ImageSource</param>
    /// <param name="rect">裁剪区域</param>
    /// <returns>返回指定位置的位图</returns>
    public static BitmapSource GetCutImage(BitmapSource bitImg, Int32Rect rect)
    {
        PixelFormat format = bitImg.Format;
        int stride = bitImg.Format.BitsPerPixel * rect.Width / 8;
        byte[] data = new byte[rect.Height * stride];
        bitImg.CopyPixels(rect, data, stride, 0);
        BitmapSource bitsrc = BitmapSource.Create(rect.Width, rect.Height, bitImg.DpiX, bitImg.DpiY, format, null, data, stride);
        return bitsrc;
    }
    /// <summary>
    /// 获取放大到指定比例并填充黑边的ImageSource
    /// </summary>
    /// <param name="bitImg">元素ImageSource</param>
    /// <returns>返回填充黑框的位图</returns>
    public static BitmapSource GetZoomImage(BitmapSource bitImg, double rate)
    {
        //白边高度
        int whiteHight = 0;
        //白边宽度
        int whiteWidth = 0;
        int resWidth = GetPixel(bitImg.Width, bitImg.DpiX);
        int resHeight = GetPixel(bitImg.Height, bitImg.DpiY);
        int lastWidth = 0;
        int lastHeigh = 0;

        int stride = 0;

        List<byte> pixData = new List<byte>();
        //原图比例
        double resRate = (double)resWidth / (double)resHeight;
        //宽度为基准
        if (resRate > rate)
        {
            lastWidth = resWidth;
            lastHeigh = (int)(lastWidth / rate);
            whiteWidth = lastWidth;
            whiteHight = (int)((lastHeigh - resHeight) / 2);

            stride = (int)(bitImg.Format.BitsPerPixel * lastWidth / 8);

            //添加白边数据
            int whiteStride = (int)(whiteHight * stride);
            byte[] whiteBytes = new byte[whiteStride];
            for (int i = 0; i < whiteBytes.Length; i++)
            {
                whiteBytes[i] = 215;
            }
            pixData.AddRange(whiteBytes.ToList());
            //添加原图数据
            byte[] data = new byte[(int)(resHeight * stride)];
            Int32Rect rect = new Int32Rect(0, 0, resWidth, resHeight);
            bitImg.CopyPixels(rect, data, stride, 0);
            pixData.AddRange(data.ToList());
            ////添加下方白边数据
            pixData.AddRange(whiteBytes.ToList());
            lastWidth = resWidth;
            lastHeigh = 2 * whiteHight + resHeight;
        }
        else
        {
            lastHeigh = resHeight;
            lastWidth = (int)(resHeight * rate);
            whiteHight = lastHeigh;
            whiteWidth = (int)((lastWidth - resWidth) / 2);
            int whiteStride = (int)(bitImg.Format.BitsPerPixel * whiteWidth / 8);

            int resStride = (int)(bitImg.Format.BitsPerPixel * resWidth / 8);
            //添加空白
            byte[] whiteBytes = new byte[whiteStride];
            for (int j = 0; j < whiteBytes.Length; j++)
            {
                whiteBytes[j] = 215;
            }
            for (int i = 0; i < (int)lastHeigh; i++)
            {
                pixData.AddRange(whiteBytes.ToList());
                //添加原图数据
                byte[] data = new byte[resStride];
                Int32Rect rect = new Int32Rect(0, i, resWidth, 1);
                bitImg.CopyPixels(rect, data, resStride, 0);
                pixData.AddRange(data.ToList());
                //添加空白
                pixData.AddRange(whiteBytes.ToList());
            }
            stride = 2 * whiteStride + resStride;
            lastWidth = 2 * whiteWidth + resWidth;
            lastHeigh = resHeight;

        }
        BitmapSource bitsrc = BitmapSource.Create(lastWidth, lastHeigh, bitImg.DpiX, bitImg.DpiY, PixelFormats.Bgr32, null, pixData.ToArray(), stride);
        return bitsrc;
    }
    public static byte[] GetBitmapBytes(BitmapSource bitImg)
    {
        byte[] bytes = new byte[0];
        JpegBitmapEncoder encoder = new JpegBitmapEncoder
        {
            QualityLevel = 100
        };
        using (MemoryStream stream = new MemoryStream())
        {
            encoder.Frames.Add(BitmapFrame.Create(bitImg));
            encoder.Save(stream);
            bytes = stream.ToArray();
            stream.Close();
        }
        return bytes;
    }

    /// <summary>
    /// 将Bitmapsource保存为jepg格式的图片文件
    /// </summary>
    /// <param name="bsrc">需保存的Bitmapsource</param>
    /// <param name="filename">保存的文件名</param>
    public static void SaveToJpgFile(BitmapSource bsrc, string filename)
    {
        JpegBitmapEncoder jpgE = new JpegBitmapEncoder();
        jpgE.Frames.Add(BitmapFrame.Create(bsrc));
        using (Stream stream = File.Create(filename))
        {
            jpgE.QualityLevel = 100;
            jpgE.Save(stream);
        }
    }
    public static void SaveToJpgFile(System.Drawing.Image img, string fileName)
    {
        if (!File.Exists(fileName))
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img, img.Width, img.Height);
            bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            bmp.Dispose();
            img.Dispose();
        }
    }
    public static void SaveToJpgFile(string source, string desc)
    {
        System.Drawing.Image img = System.Drawing.Image.FromFile(source);
        SaveToJpgFile(img, desc);
    }
    /// <summary>
    /// Wpf度量转换为像素
    /// </summary>
    /// <param name="wpfd">wpf度量</param>
    /// <param name="dpi"></param>
    /// <returns></returns>
    public static int GetPixel(double wpfd, double dpi)
    {
        return (int)(wpfd / 96 * dpi);
    }
}
