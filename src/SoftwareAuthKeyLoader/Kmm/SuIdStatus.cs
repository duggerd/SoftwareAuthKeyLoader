using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class SuIdStatus
    {
        public SuId SuId { get; private set; }

        public bool KeyAssigned { get; private set; }

        public bool ActiveSuId { get; private set; }

        public SuIdStatus(byte[] contents)
        {
            Parse(contents);
        }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public void Parse(byte[] contents)
        {
            if (contents.Length != 8)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 7, got {0} - {0}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            /* suid */
            byte[] suId = new byte[7];
            Array.Copy(contents, 0, suId, 0, 7);
            SuId = new SuId(suId);

            /* k status */
            KeyAssigned = Convert.ToBoolean(contents[7] & 0x01);
            ActiveSuId = Convert.ToBoolean(contents[7] & 0x02);
        }

        public override string ToString()
        {
            return string.Format("[SuId: {0}, KeyAssigned: {1}, ActiveSuId: {2}]", SuId.ToString(), KeyAssigned, ActiveSuId);
        }
    }
}
