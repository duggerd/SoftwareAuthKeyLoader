using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class DeleteAuthenticationKeyResponse : KmmBody
    {
        public Status Status { get; private set; }

        public int NumKeysDeleted { get; private set; }

        public SuId SuId { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.DeleteAuthenticationKeyResponse;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public DeleteAuthenticationKeyResponse(byte[] contents)
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

            /* suid */
            byte[] suId = new byte[7];
            Array.Copy(contents, 0, suId, 0, 7);
            SuId = new SuId(suId);

            /* number of keys deleted */
            NumKeysDeleted |= (contents[7] & 0xFF) << 8;
            NumKeysDeleted |= contents[8] & 0xFF;

            /* status */
            Status = (Status)contents[9];
        }

        public override string ToString()
        {
            return string.Format("[SuId: {0}, NumKeysDeleted: {1}, Status: {2} (0x{3:X2})]", SuId.ToString(), NumKeysDeleted, Status.ToString(), (byte)Status);
        }
    }
}
