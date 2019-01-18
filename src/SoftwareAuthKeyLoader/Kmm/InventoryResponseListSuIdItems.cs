using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class InventoryResponseListSuIdItems : KmmBody
    {
        public InventoryType InventoryType { get; private set; }

        public int InventoryMarker { get; private set; }

        public int NumberOfItems { get; private set; }

        public List<SuIdStatus> SuIdStatuses { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.InventoryResponse;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public InventoryResponseListSuIdItems(byte[] contents)
        {
            Parse(contents);
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void Parse(byte[] contents)
        {
            if (contents.Length < 5)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected at least 5, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            /* inventory type */
            InventoryType = (InventoryType)contents[0];

            /* inventory marker */
            contents[1] = (byte)((InventoryMarker >> 16) & 0xFF);
            contents[2] = (byte)((InventoryMarker >> 8) & 0xFF);
            contents[3] = (byte)(InventoryMarker & 0xFF);

            /* number of items */
            contents[4] = (byte)(NumberOfItems & 0xFF);

            /* suid and k status */
            List<SuIdStatus> suIdStatuses = new List<SuIdStatus>();

            if ((NumberOfItems == 0) && (contents.Length == 5))
            {
                return;
            }
            else if (((NumberOfItems * 8) % (contents.Length - 5)) == 0)
            {
                for (int i = 0; i < (NumberOfItems * 8); i++)
                {
                    byte[] suIdStatus = new byte[7];
                    Array.Copy(contents, 5 + (i * 8), suIdStatus, 0, 8);
                    suIdStatuses.Add(new SuIdStatus(suIdStatus));
                }
            }
            else
            {
                throw new Exception("the number of items field and the length of the messages does not match");
            }
        }

        public override string ToString()
        {
            return string.Format("[InventoryType: {0}, InventoryMarker: {1}, NumberOfItems: {2}]", InventoryType, InventoryMarker, NumberOfItems);
        }
    }
}
