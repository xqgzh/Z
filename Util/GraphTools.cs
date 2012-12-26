using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Z.Util
{
    /// <summary>
    /// 图形工具类
    /// </summary>
    public class GraphTools
    {
        ///<summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="originalImagePath">源图路径（物理路径）</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式</param>     
        public static string MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, EnumPicMode mode = EnumPicMode.Cut)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case EnumPicMode.HW://指定高宽缩放（可能变形）                 
                    break;
                case EnumPicMode.W://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case EnumPicMode.H://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case EnumPicMode.Cut://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            System.Drawing.Image bitmap = new Bitmap(towidth, toheight);
            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);
            string outthumbnailPath = string.Empty;
            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                outthumbnailPath = thumbnailPath;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();

            }
            return outthumbnailPath;
        }
    }

    /// <summary>
    /// 图片缩略模式
    /// </summary>
    public enum EnumPicMode
    {
        /// <summary>
        /// 指定高宽缩放（可能变形)
        /// </summary>
        HW,
        /// <summary>
        /// 指定宽，高按比例    
        /// </summary>
        W,
        /// <summary>
        /// 指定高，宽按比例 
        /// </summary>
        H,
        /// <summary>
        /// 指定高宽裁减（不变形）  
        /// </summary>
        Cut,
    }
}
