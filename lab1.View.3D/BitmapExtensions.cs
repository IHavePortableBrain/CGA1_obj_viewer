using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Drawing
{
    public static class BitmapExtensions
    {
        public static void DrawDdaLine(this Bitmap image, int xStart, int yStart, int xEnd, int yEnd)
        {
            //xStart = Math.Max(0, xStart);
            //xEnd = Math.Min(image.Width, xEnd);
            //yStart = Math.Max(0, yStart);
            //yEnd = Math.Min(image.Width, yEnd);
            int dx = xEnd - xStart,
                dy = yEnd - yStart,
                stepCount = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy); // clip with Math.Max min and remove if condition; foreach + partioner
            float
                x = xStart,
                y = yStart,
                xInc = dx / (float)stepCount,
                yInc = dy / (float)stepCount;

            for (int i = 0; i < stepCount; i++)
            {
                x += xInc;
                y += yInc;

                if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
                    image.SetPixel((int)x, (int)y, Color.Black); // todo color from params
            }
        }

        public static void DrawDdaLineUnsafe(this Bitmap image, int xStart, int yStart, int xEnd, int yEnd)
        {
            int dx = xEnd - xStart,
                dy = yEnd - yStart,
                stepCount = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy); // clip with Math.Max min and remove if condition; foreach + partioner
            float
                x = xStart,
                y = yStart,
                xInc = dx / (float)stepCount,
                yInc = dy / (float)stepCount;

            unsafe
            {
                var width = image.Width;
                var height = image.Height;
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                for (int i = 0; i < stepCount; i++)
                //Partitioner.Create(0, stepCount);
                //Parallel.For(0, stepCount, i =>
                {
                    var xx = x + i * xInc;
                    var yy = y + i * yInc;

                    uint* pixel = (uint*)(bitmapData.Scan0 + (int)xx * bytesPerPixel + (int)yy * bitmapData.Stride);
                    if (xx >= 0 && xx < width && yy >= 0 && yy < height)
                    {
                        pixel[0] = 0xff_00_00_00;
                    }
                }//);
                image.UnlockBits(bitmapData);
            }
        }

        public static void DrawDdaLinesUnsafe(this Bitmap image, IEnumerable<(int xStart, int yStart, int xEnd, int yEnd)> lines)
        {
            var width = image.Width;
            var height = image.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.PixelFormat);

            //var options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            var partitioner = Partitioner.Create(lines);
            Parallel.ForEach(partitioner, line =>
            {
                var (xStart, yStart, xEnd, yEnd) = line;
                int dx = xEnd - xStart,
                    dy = yEnd - yStart,
                    stepCount = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy); // clip with Math.Max min and remove if condition; foreach + partioner
                float
                    x = xStart,
                    y = yStart,
                    xInc = dx / (float)stepCount,
                    yInc = dy / (float)stepCount;

                unsafe
                {
                    for (int i = 0; i < stepCount; i++)
                    {
                        x += xInc;
                        y += yInc;

                        uint* pixel = (uint*)(bitmapData.Scan0 + (int)x * bytesPerPixel + (int)y * bitmapData.Stride);
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            pixel[0] = 0xff_00_00_00;
                        }
                    }
                }
            });
            image.UnlockBits(bitmapData);
        }

        public static void ProcessUsingLockbits(this Bitmap processedBitmap)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * processedBitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int oldBlue = pixels[currentLine + x];
                    int oldGreen = pixels[currentLine + x + 1];
                    int oldRed = pixels[currentLine + x + 2];

                    // calculate new pixel value
                    pixels[currentLine + x] = (byte)0;
                    pixels[currentLine + x + 1] = (byte)0;
                    pixels[currentLine + x + 2] = (byte)0;
                    pixels[currentLine + x + 3] = (byte)255;
                }
            }

            // copy modified bytes back
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            processedBitmap.UnlockBits(bitmapData);
        }
    }
}
