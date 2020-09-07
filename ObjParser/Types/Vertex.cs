using System;
using System.Globalization;
using System.Numerics;

namespace ObjParser.Types
{
    public class Vertex : IType
    {
        public const int MinimumDataLength = 4;
        public const string Prefix = "v";

        public Vector4 Vector { get; set; }

        public int Index { get; set; }

		public void LoadFrom(string[] data)
        {
            if (data.Length < MinimumDataLength)
                throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, nameof(data));

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException("Data prefix must be '" + Prefix + "'", nameof(data));

            bool success;

            success = float.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float x);
            if (!success) throw new ArgumentException("Could not parse X parameter as double");

            success = float.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float y);
            if (!success) throw new ArgumentException("Could not parse Y parameter as double");

            success = float.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float z);
            if (!success) throw new ArgumentException("Could not parse Z parameter as double");

            float w = 1;
            if (data.Length > MinimumDataLength)
            {
                success = float.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out w);
                if (!success) throw new ArgumentException("Could not parse W parameter as double");
            }
            Vector = new Vector4(x, y, z, w);
        }

        public override string ToString()
        {
            return $"{Prefix} {Vector.X} {Vector.Y} {Vector.Z} {Vector.W}";
        }
    }
}
