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

                lock (image)
                {
                    if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
                        image.SetPixel((int)x, (int)y, Color.Black); // todo color from params
                }
            }
        }
    }
}
