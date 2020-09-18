namespace System.Numerics
{
    public static class Matrix4x4Extensions
    {
        public static Matrix4x4 CreateRotation(this Vector3 vector, float radian)
        {
            var cos = (float)Math.Cos(radian);
            var sin = (float)Math.Sin(radian);
            var x = vector.X;
            var y = vector.Y;
            var z = vector.Z;
            return new Matrix4x4(
                    cos + (1 - cos) * x * x, (1 - cos) * x * y - sin * z, (1 - cos) * x * z + sin * y, 0,
                    (1 - cos) * y * x + sin * z, cos + (1 - cos) * y * y, (1 - cos) * y * z - sin * x, 0,
                    (1 - cos) * z * x - sin * y, (1 - cos) * z * y + sin * x, cos + (1 - cos) * z * z, 0,
                    0, 0, 0, 1);
        }
    }
}
