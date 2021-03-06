using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjParser.Types
{
    public class VertexTexture : IType
    {
        public const int MinimumDataLength = 3;
        public const string Prefix = "vt";

        public double X { get; set; }
        public double Y { get; set; }
        public double? W { get; set; } = 0;

        public int Index { get; set; }

		public void LoadFrom(string[] data)
        {
            if (data.Length < MinimumDataLength)
                throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, nameof(data));

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException("Data prefix must be '" + Prefix + "'", nameof(data));

            bool success;

            success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double x);
            if (!success) throw new ArgumentException("Could not parse X parameter as double");

            success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double y);
            if (!success) throw new ArgumentException("Could not parse Y parameter as double");

            if (data.Length > MinimumDataLength)
            {
                success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double w);
                if (!success) throw new ArgumentException("Could not parse W parameter as double");
                W = w;
            }

            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{Prefix} {X} {Y}" + (W.HasValue ? $" {W}" : string.Empty);
        }
    }
}
