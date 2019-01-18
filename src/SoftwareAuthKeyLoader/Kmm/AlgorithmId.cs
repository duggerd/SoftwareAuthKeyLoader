using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public enum AlgorithmId : byte
    {
        Clear = 0x80,
        AES128 = 0x85
    }
}
