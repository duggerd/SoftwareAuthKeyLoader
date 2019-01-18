using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class KmmFrame
    {
        public KmmBody KmmBody { get; private set; }

        public KmmFrame(KmmBody kmmBody)
        {
            if (kmmBody == null)
            {
                throw new ArgumentNullException("kmmBody");
            }

            KmmBody = kmmBody;
        }

        public KmmFrame(byte[] contents)
        {
            Parse(contents);
        }

        public byte[] ToBytes()
        {
            byte[] body = KmmBody.ToBytes();

            int length = 24 + body.Length;

            byte[] contents = new byte[length];

            /* version */
            contents[0] = 0x00;

            /* mfid */
            contents[1] = 0x00;

            /* algorithm id */
            contents[2] = (byte)AlgorithmId.Clear;

            /* key id */
            contents[3] = 0x00;
            contents[4] = 0x00;

            /* message indicator */
            contents[5] = 0x00;
            contents[6] = 0x00;
            contents[7] = 0x00;
            contents[8] = 0x00;
            contents[9] = 0x00;
            contents[10] = 0x00;
            contents[11] = 0x00;
            contents[12] = 0x00;
            contents[13] = 0x00;

            /* KMM */

            /* message id */
            contents[14] = (byte)KmmBody.MessageId;

            /* message length */
            int messageLength = 7 + body.Length;
            contents[15] = (byte)((messageLength >> 8) & 0xFF);
            contents[16] = (byte)(messageLength & 0xFF);

            /* message format */
            BitArray messageFormat = new BitArray(8, false);
            messageFormat.Set(7, Convert.ToBoolean(((byte)KmmBody.ResponseKind & 0x02) >> 1));
            messageFormat.Set(6, Convert.ToBoolean((byte)KmmBody.ResponseKind & 0x01));
            messageFormat.CopyTo(contents, 17);

            /* destination rsi */
            contents[18] = 0xFF;
            contents[19] = 0xFF;
            contents[20] = 0xFF;

            /* source rsi */
            contents[21] = 0xFF;
            contents[22] = 0xFF;
            contents[23] = 0xFF;

            /* message body */
            Array.Copy(body, 0, contents, 24, body.Length);

            return contents;
        }

        private void Parse(byte[] contents)
        {
            if (contents.Length <= 17)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected at least 17, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            byte messageId = contents[14];

            int messageLength = 0;
            messageLength |= (contents[15] & 0xFF) << 8;
            messageLength |= contents[16] & 0xFF;

            int messageBodyLength = messageLength - 7;
            byte[] messageBody = new byte[messageBodyLength];
            Array.Copy(contents, 24, messageBody, 0, messageBodyLength);

            if ((MessageId)messageId == MessageId.InventoryResponse)
            {
                if (messageBody.Length > 0)
                {
                    InventoryType inventoryType = (InventoryType)messageBody[0];

                    if (inventoryType == InventoryType.ListActiveSuId)
                    {
                        KmmBody kmmBody = new InventoryResponseListActiveSuId(messageBody);
                        KmmBody = kmmBody;
                    }
                    else
                    {
                        throw new Exception(string.Format("unknown inventory response type: 0x{0:X2}", (byte)inventoryType));
                    }
                }
                else
                {
                    throw new Exception("inventory response length zero");
                }
            }
            else if ((MessageId)messageId == MessageId.NegativeAcknowledgement)
            {
                KmmBody kmmBody = new NegativeAcknowledgement(messageBody);
                KmmBody = kmmBody;
            }
            else if ((MessageId)messageId == MessageId.LoadAuthenticationKeyResponse)
            {
                KmmBody kmmBody = new LoadAuthenticationKeyResponse(messageBody);
                KmmBody = kmmBody;
            }
            else if ((MessageId)messageId == MessageId.DeleteAuthenticationKeyResponse)
            {
                KmmBody kmmBody = new DeleteAuthenticationKeyResponse(messageBody);
                KmmBody = kmmBody;
            }
            else
            {
                throw new Exception(string.Format("unknown kmm - message id: 0x{0:X2}", messageId));
            }
        }

        public override string ToString()
        {
            return string.Format("[MessageId: {0} (0x{1:X2})]", KmmBody.MessageId.ToString(), (byte)KmmBody.MessageId);
        }
    }
}
