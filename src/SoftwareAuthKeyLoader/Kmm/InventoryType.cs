using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public enum InventoryType : byte
    {
        ListActiveSuId = 0xF7,
        ListSuIdItems = 0xF8
    }
}
