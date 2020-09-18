using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.CompilerServices;
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
                //Parallel.For(0, stepCount, i => - if u do this take care about x and y become iteration local
                {
                    x += xInc;
                    y += yInc;

                    uint* pixel = (uint*)(bitmapData.Scan0 + (int)x * bytesPerPixel + (int)y * bitmapData.Stride);
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        pixel[0] = 0xff_00_00_00;
                    }
                }
                image.UnlockBits(bitmapData);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RasterizeTriangle(this Bitmap image, float[] zBuffer, Vector4[] t, Color color)
        {
            var width = image.Width;
            var height = image.Height;
            if (t[0].X < 0 || t[0].X >= width || t[0].Y < 0 || t[0].Y >= height ||
                t[1].X < 0 || t[1].X >= width || t[1].Y < 0 || t[1].Y >= height ||
                t[2].X < 0 || t[2].X >= width || t[2].Y < 0 || t[2].Y >= height ||
                t[0].Z < 0 || t[0].Z > 1000 || // todo check + refactor
                t[1].Z < 0 || t[1].Z > 1000 ||
                t[2].Z < 0 || t[2].Z > 1000) return;

            if (t[0].Y == t[1].Y && t[0].Y == t[2].Y) return; // i dont care about degenerate triangles

            //t.Sort((Vector4 v1, Vector4 v2) => (int)(v1.Y - v2.Y)); // todo maybe buble sort
            if (t[0].Y > t[1].Y) t[0].Swap(ref t[1]);
            if (t[0].Y > t[2].Y) t[0].Swap(ref t[2]);
            if (t[1].Y > t[2].Y) t[1].Swap(ref t[2]);

            var total_height = t[2].Y - t[0].Y;
            for (int i = 0; i < total_height; i++)
            {
                bool second_half = i > t[1].Y - t[0].Y || t[1].Y == t[0].Y;
                float segment_height = second_half ? t[2].Y - t[1].Y : t[1].Y - t[0].Y;
                float alpha = i / total_height;
                float beta = (i - (second_half ? t[1].Y - t[0].Y : 0)) / segment_height; // be careful: with above conditions no division by zero here
                Vector4 A = t[0] + (t[2] - t[0]) * alpha;
                Vector4 B = second_half ? t[1] + (t[2] - t[1]) * beta : t[0] + (t[1] - t[0]) * beta;
                if (A.X > B.X) A.Swap(ref B);

                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                unsafe
                {
                    // draw horizontal line
                    for (int j = (int)A.X; j <= B.X; j++)
                    {
                        var x = j;
                        var y = t[0].Y + i;

                        float phi = (float)(x - A.X) / (float)(B.X - A.X + 1);
                        float z = A.Z + (B.Z - A.Z) * phi;

                        if (z < zBuffer[(int)y * image.Width + x] &&  x >= 0 && x < width && y >= 0 && y < height)
                        {
                            zBuffer[(int)y * image.Width + x] = z;
                            uint* pixel = (uint*)(bitmapData.Scan0 + x * bytesPerPixel + (int)y * bitmapData.Stride);
                            pixel[0] = 0xff_00_00_00;
                        }
                    }
                }
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
    }
}
