using System;
using Microsoft.Xna.Framework;

namespace ClassicGraphics
{
    [Serializable]
    internal class Picture
    {
        public float[,][] c;

        public Picture()
        {
            c = new float[1,1][];
            c[0,0] = new float[] { 1F, 1F, 1F };
        }

        public Picture(float[,][] col)
        {
            c = col;
        }
    }
}
