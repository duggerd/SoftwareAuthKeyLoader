using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public enum MessageId : byte
    {
        InventoryCommand = 0x0D,
        InventoryResponse = 0x0E,
        NegativeAcknowledgement = 0x16,
        LoadAuthenticationKeyCommand = 0x28,
        LoadAuthenticationKeyResponse = 0x29,
        DeleteAuthenticationKeyCommand = 0x2A,
        DeleteAuthenticationKeyResponse = 0x2B
    }
}
