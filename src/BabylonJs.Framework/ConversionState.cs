using BabylonJs.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework
{
    public class ConversionState
    {
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Maps vertices to indices
        /// </summary>
        public Dictionary<Vector3, int> Index { get; set; }

        public List<int> Indices { get; set; }

        public List<Vector3> Vertices { get; set; }

        public List<float> Normals { get; set; }

        public string Name { get; set; }

        public ConversionState()
        {
            this.CurrentIndex = 0;
            this.Indices = new List<int>();
            this.Vertices = new List<Vector3>();
            this.Normals = new List<float>();
            this.Index = new Dictionary<Vector3, int>();
        }
    }
}
