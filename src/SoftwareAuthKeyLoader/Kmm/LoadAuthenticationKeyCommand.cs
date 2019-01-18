using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class LoadAuthenticationKeyCommand : KmmBody
    {
        public bool TargetSpecificSuId { get; private set; }

        public SuId SuId { get; private set; }

        public AlgorithmId InnerAlgorithmId { get; private set; }

        public byte[] Key { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.LoadAuthenticationKeyCommand;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.Immediate;
            }
        }

        public LoadAuthenticationKeyCommand(bool targetSpecificSuId, SuId suId, byte[] key)
        {
            if (suId == null)
            {
                throw new ArgumentNullException("suId");
            }

            if (key.Length != 16)
            {
                throw new ArgumentOutOfRangeException("key", string.Format("length mismatch - expected 16, got {0} - {1}", key.Length.ToString(), BitConverter.ToString(key)));
            }

            TargetSpecificSuId = targetSpecificSuId;
            SuId = suId;
            InnerAlgorithmId = AlgorithmId.AES128;
            Key = key;
        }

        public override byte[] ToBytes()
        {
            int length = 14 + Key.Length;

            byte[] contents = new byte[length];

            /* DECRYPTION INSTRUCTION BLOCK */

            /* decryption instruction format */
            contents[0] = 0x00;

            /* outer algorithm id */
            contents[1] = (byte)AlgorithmId.Clear;

            /* key id */
            contents[2] = 0x00;
            contents[3] = 0x00;

            /* AUTHENTICATION BLOCK */

            /* authentication instruction */
            BitArray authenticationInstruction = new BitArray(8, false);
            authenticationInstruction.Set(0, TargetSpecificSuId);
            authenticationInstruction.CopyTo(contents, 4);

            /* suid */
            byte[] suId = SuId.ToBytes();
            Array.Copy(suId, 0, contents, 5, suId.Length);

            /* inner algoritm id */
            contents[12] = (byte)InnerAlgorithmId;

            /* key length */
            contents[13] = (byte)Key.Length;

            /* key data */
            Array.Copy(Key, 0, contents, 14, Key.Length);

            return contents;
        }

        protected override void Parse(byte[] contents)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("[TargetSpecificSuId: {0}, SuId: {1}, InnerAlgorithmId: {2} 0x{3:X2}, Key (hex): {4}]", TargetSpecificSuId, SuId.ToString(), InnerAlgorithmId.ToString(), (byte)InnerAlgorithmId, BitConverter.ToString(Key));
        }
    }
}
