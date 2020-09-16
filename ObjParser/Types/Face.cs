using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ObjParser.Types
{
    public class Face : IType
    {
        public const int MinimumDataLength = 4;
        public const string Prefix = "f";

        public string UseMtl { get; set; }
        public List<int> UnobservedEdgeStartIndecies { get; set; }
        public int[] VertexIndicies { get; set; }
        public int?[] TextureIndicies { get; set; }
        public int?[] NormalIndicies { get; set; }

        public void LoadFrom(string[] data)
        {
            if (data.Length < MinimumDataLength)
                throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, nameof(data));

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException("Data prefix must be '" + Prefix + "'", nameof(data));            

            int vertexCount = data.Count() - 1;
            VertexIndicies = new int[vertexCount];
            TextureIndicies = new int?[vertexCount];
            NormalIndicies = new int?[vertexCount];

            bool success;

            for (int i = 0; i < vertexCount; i++)
            {
                string[] parts = data[i + 1].Split('/');

                success = int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int vertexIndex);
                if (!success) throw new ArgumentException("Could not parse parameter as int");
                vertexIndex -= 1; // Obj indexing starts from 1. Array indexing starts from 0;
                VertexIndicies[i] = vertexIndex;

                if (parts.Count() > 1)
                {
                    success = int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out vertexIndex);
                    vertexIndex -= 1;
                    if (success) {
                        TextureIndicies[i] = vertexIndex;
                    }

                    if (parts.Count() > 2)
                    {
                        success = int.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out vertexIndex);
                        vertexIndex -= 1;
                        if (success)
                        {
                            NormalIndicies[i] = vertexIndex;
                        }
                    }
                }
            }
        }

        // HACKHACK this will write invalid files if there are no texture vertices in
        // the faces, need to identify that and write an alternate format
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(Prefix);

            for (int i = 0; i < VertexIndicies.Count(); i++)
            {
                var texture = TextureIndicies[i];
                var normal = NormalIndicies[i];
                var append = texture.HasValue || normal.HasValue ? $"/{texture}/{normal}" : string.Empty;
                b.Append($" {VertexIndicies[i]}{append}");
            }

            return b.ToString();
        }
    }
}
