using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthello_AC
{
    class Size
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Size(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
}
