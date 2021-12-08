using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    abstract class IO_Operations
    {
        public abstract (byte[] R, byte[] G, byte[] B) ReadImage();
    }

    class IO_bmp : IO_Operations
    {
        public override (byte[] R, byte[] G, byte[] B) ReadImage()
        {
            throw new NotImplementedException();
        }
    }
}
