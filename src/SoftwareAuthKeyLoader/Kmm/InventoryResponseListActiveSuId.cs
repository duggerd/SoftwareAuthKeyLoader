using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class InventoryResponseListActiveSuId : KmmBody
    {
        public InventoryType InventoryType { get; private set; }

        public bool ActiveSuId { get; private set; }

        public bool KeyAssigned { get; private set; }

        public SuId SuId { get; private set; }

        public Status Status { get; private set; }

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

        public InventoryResponseListActiveSuId(byte[] contents)
        {
            Parse(contents);
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void Parse(byte[] contents)
        {
            if (contents.Length != 10)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 10, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            /* inventory type */
            InventoryType = (InventoryType)contents[0];

            /* inventory instruction */
            ActiveSuId = Convert.ToBoolean(contents[1] & 0x01);
            KeyAssigned = Convert.ToBoolean(contents[1] & 0x02);

            /* suid */
            byte[] suId = new byte[7];
            Array.Copy(contents, 2, suId, 0, 7);
            SuId = new SuId(suId);

            /* status */
            Status = (Status)contents[9];
        }

        public override string ToString()
        {
            return string.Format("[ActiveSuId: {0}, KeyAssigned: {1}, SuId: {2}, Status: {3} (0x{4:X2})]", ActiveSuId, KeyAssigned, SuId.ToString(), Status.ToString(), (byte)Status);
        }
    }
}
