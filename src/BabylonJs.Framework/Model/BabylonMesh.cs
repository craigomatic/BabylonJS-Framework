using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Model
{
    public class BabylonMesh
    {
        public string Name { get; set; }

        public float[] Position { get; set; }

        public float[] Rotation { get; set; }

        public float[] Scaling { get; set; }

        public bool InfiniteDistance { get; set; }

        public bool IsVisible { get; set; }

        public bool IsEnabled { get; set; }

        public bool Pickable { get; set; }

        public bool ApplyFog { get; set; }

        public int AlphaIndex { get; set; }

        public BillboardMode BillboardMode { get; set; }

        public bool ReceiveShadows { get; set; }

        public bool CheckCollisions { get; set; }

        public float[] Positions { get; set; }

        public float[] Normals { get; set; }

        public float[] Uvs { get; set; }

        public int[] Indices { get; set; }
    }
}
