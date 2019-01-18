using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader.Kmm
{
    public abstract class KmmBody
    {
        public abstract MessageId MessageId { get; }

        public abstract ResponseKind ResponseKind { get; }

        public abstract byte[] ToBytes();

        protected abstract void Parse(byte[] contents);

        public override abstract string ToString();
    }
}
