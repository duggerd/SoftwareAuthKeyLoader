using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class InventoryCommandListActiveSuId : KmmBody
    {
        public override MessageId MessageId
        {
            get
            {
                return MessageId.InventoryCommand;
            }
        }

        public InventoryType InventoryType
        {
            get
            {
                return InventoryType.ListActiveSuId;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.Immediate;
            }
        }

        public InventoryCommandListActiveSuId()
        {
        }

        public override byte[] ToBytes()
        {
            byte[] contents = new byte[1];

            /* inventory type */
            contents[0] = (byte)InventoryType;

            return contents;
        }

        protected override void Parse(byte[] contents)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("[InventoryType: {0} (0x{1:X2})]", InventoryType.ToString(), (byte)InventoryType);
        }
    }
}
