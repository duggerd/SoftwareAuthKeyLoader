using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class SuId
    {
        public int WacnId { get; private set; }

        public int SystemId { get; private set; }

        public int UnitId { get; private set; }

        public SuId(int wacnId, int systemId, int unitId)
        {
            if (WacnId < 0 || WacnId > 0xFFFFF)
            {
                throw new ArgumentOutOfRangeException("wacnId");
            }

            if (SystemId < 0 || SystemId > 0xFFF)
            {
                throw new ArgumentOutOfRangeException("systemId");
            }

            if (UnitId < 0 || UnitId > 0xFFFFFF)
            {
                throw new ArgumentOutOfRangeException("unitId");
            }

            WacnId = wacnId;
            SystemId = systemId;
            UnitId = unitId;
        }

        public SuId(byte[] contents)
        {
            Parse(contents);
        }

        public byte[] ToBytes()
        {
            byte[] contents = new byte[7];

            contents[0] = (byte)((WacnId >> 12) & 0xFF);
            contents[1] = (byte)((WacnId >> 4) & 0xFF);
            contents[2] |= (byte)((WacnId << 4) & 0xF0);
            contents[2] |= (byte)((SystemId >> 8) & 0x0F);
            contents[3] = (byte)(SystemId & 0xFF);
            contents[4] = (byte)((UnitId >> 16) & 0xFF);
            contents[5] = (byte)((UnitId >> 8) & 0xFF);
            contents[6] = (byte)(UnitId & 0xFF);

            return contents;
        }

        public void Parse(byte[] contents)
        {
            if (contents.Length != 7)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 7, got {0} - {0}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            WacnId |= (contents[0] & 0xFF) << 12;
            WacnId |= (contents[1] & 0xFF) << 4;
            WacnId |= (contents[2] >> 4) & 0x0F;
            SystemId |= (contents[2] & 0x0F) << 8;
            SystemId |= contents[3] & 0xFF;
            UnitId |= (contents[4] & 0xFF) << 16;
            UnitId |= (contents[5] & 0xFF) << 8;
            UnitId |= contents[6] & 0xFF;
        }

        public override string ToString()
        {
            return string.Format("[WacnId: 0x{0:X}, SystemId: 0x{1:X}, UnitId: 0x{2:X}]", WacnId, SystemId, UnitId);
        }
    }
}
