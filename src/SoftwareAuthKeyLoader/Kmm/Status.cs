using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public enum Status : byte
    {
        CommandWasPerformed,
        CommandCouldNotBePerformed,
        ItemDoesNotExist,
        InvalidMessageId,
        InvalidChecksumOrMac,
        OutOfMemory,
        CouldNotDecryptMessage,
        InvalidMessageNumber,
        InvalidKeyId,
        InvalidAlgorithmId,
        InvalidMfId,
        ModuleFailure,
        MiAllZeros,
        Keyfail,
        InvalidWacnIdOrSystemId,
        InvalidSubscriberId,
        Unknown = 0xFF
    }
}
