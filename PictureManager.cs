using System.Collections.Generic;
using DansCSharpLibrary.Serialization;

namespace ClassicGraphics
{
    class PictureManager
    {
        List<PPPair> pics;

        internal PictureManager()
        {
            pics = new List<PPPair>();
        }

        internal void Add(string n)
        {
            Picture p = BinarySerialization.ReadFromBinaryFile<Picture>(n);
            pics.Add(new PPPair(n, p));
        }
        internal void Add(string n, Picture p)
        {
            pics.Add(new PPPair(n, p));
        }

        internal Picture Get(string n)
        {
            for (int i = 0; i < pics.Count; i++)
            {
                if (pics[i].path == n)
                {
                    return pics[i].pic;
                }
            }
            return null;
        }
    }



    class PPPair
    {
        public string path;
        public Picture pic;

        internal PPPair(string pa, Picture pi)
        {
            path = pa;
            pic = pi;
        }
    }
}
