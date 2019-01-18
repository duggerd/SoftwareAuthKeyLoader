using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public class NegativeAcknowledgement : KmmBody
    {
        public MessageId AcknowledgedMessageId { get; private set; }

        public Status Status { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.NegativeAcknowledgement;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public NegativeAcknowledgement(byte[] contents)
        {
            Parse(contents);
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void Parse(byte[] contents)
        {
            if (contents.Length != 2)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 2, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents))); 
            }

            /* acknowledged message id */
            AcknowledgedMessageId = (MessageId)contents[0];

            /* status */
            Status = (Status)contents[1];
        }

        public override string ToString()
        {
            return string.Format("[AcknowledgedMessageId: {0} (0x{1:X2}), Status: {2} (0x{3:X2})]", AcknowledgedMessageId.ToString(), (byte)AcknowledgedMessageId, Status.ToString(), (byte)Status);
        }
    }
}
