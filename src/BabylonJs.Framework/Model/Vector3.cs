using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Model
{
    public class Vector3
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float[] ToArray()
        {
            return new float[3] { this.X, this.Y, this.Z };
        }

        public override bool Equals(object obj)
        {
            var otherVector3 = (obj as Vector3);

            if (otherVector3 == null)
            {
                return false;
            }

            return otherVector3.X == this.X && otherVector3.Y == this.Y && otherVector3.Z == this.Z;
        }

        public override int GetHashCode()
        {
            return new { this.X, this.Y, this.Z }.GetHashCode();
        }

        public void Normalise()
        {
            var length = Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            
            this.X = Convert.ToSingle(this.X / length);
            this.Y = Convert.ToSingle(this.Y / length);
            this.Z = Convert.ToSingle(this.Z / length);
        }

        public static Vector3 FromString(string vectorString)
        {
            var split = vectorString.Trim().Split(' ');

            return new Vector3(
                    Convert.ToSingle(split[0].Trim()), 
                    Convert.ToSingle(split[1].Trim()), 
                    Convert.ToSingle(split[2].Trim()));
        }

        private static readonly Vector3 _Zero = new Vector3(0, 0, 0);

        public static Vector3 Zero
        {
            get
            {
                return _Zero;
            }
        }
    }
}
