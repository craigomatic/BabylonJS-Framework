using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Model
{
    public class BabylonFile
    {
        public IEnumerable<BabylonCamera> Cameras { get; set; }

        public IEnumerable<BabylonMesh> Meshes { get; set; }
    }
}
