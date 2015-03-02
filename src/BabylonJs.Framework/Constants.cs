using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework
{
    public enum BillboardMode
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7
    }

    public static class StlConstants
    {
        public const string Solid = "solid";
        public const string FacetNormal = "facet normal";
        public const string OuterLoop = "outer loop";
        public const string Vertex = "vertex";
        public const string EndLoop = "endloop";
        public const string EndFacet = "endfacet";
        public const string EndSolid = "endsolid";
    }
}
