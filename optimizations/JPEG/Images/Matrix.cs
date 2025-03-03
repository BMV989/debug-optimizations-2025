using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images;

public class Matrix
{
    public readonly int Width;
    public readonly int Height;
    public readonly Pixel[,] Pixels;

    public Matrix(int height, int width)
    {
        Width = width;
        Height = height;
        Pixels = new Pixel[height, width];
    }

    public static unsafe explicit operator Matrix(Bitmap bmp)
    {
        var width = bmp.Width - bmp.Width % 8;
        var height = bmp.Height - bmp.Height % 8;
        var matrix = new Matrix(height, width);
        var buff = bmp.LockBits(new Rectangle(0, 0, width, height), 
            ImageLockMode.ReadOnly, bmp.PixelFormat);

        try
        {
            for (var h = 0; h < height; h++)
            {
                var pixelPtr = (byte*)buff.Scan0 + h * buff.Stride;
                for (var w = 0; w < width; w++)
                {
                    var blue = *pixelPtr++;
                    var green = *pixelPtr++;
                    var red = *pixelPtr++;
                    matrix.Pixels[h, w] = new Pixel(red, green, blue, PixelFormat.RGB);
                }
            }

            return matrix;
        }
        finally
        {
            bmp.UnlockBits(buff);
        }
    }

    public static unsafe explicit operator Bitmap(Matrix matrix)
    {
        var width = matrix.Width;
        var height = matrix.Height;
    
        var bmp = new Bitmap(matrix.Width, matrix.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        var buff = bmp.LockBits(new Rectangle(0, 0, width, height), 
            ImageLockMode.WriteOnly, bmp.PixelFormat);

        try
        {
            for (var h = 0; h < height; h++)
            {
                var pixelPtr = (byte*)buff.Scan0 + h * buff.Stride;
                for (var w = 0; w < width; w++)
                {
                    var pixel = matrix.Pixels[h, w];
                    
                    *pixelPtr = ToByte(pixel.B);
                    pixelPtr++;
                    
                    *pixelPtr = ToByte(pixel.G);
                    pixelPtr++;
                    
                    *pixelPtr = ToByte(pixel.R);
                    pixelPtr++;
                }
            }

            return bmp;
        }
        finally
        {
            bmp.UnlockBits(buff);
        }
    }


    private static byte ToByte(double d) => (byte)Math.Clamp(d, byte.MinValue, byte.MaxValue);
}