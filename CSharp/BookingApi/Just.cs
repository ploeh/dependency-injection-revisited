using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public sealed class Just<T> : IMaybe<T>
    {
        private readonly T value;

        public Just(T value)
        {
            this.value = value;
        }

        public TResult Accept<TResult>(IMaybeVisitor<T, TResult> visitor)
        {
            return visitor.VisitJust(this.value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Just<T>;
            if (other == null)
                return base.Equals(obj);

            return object.Equals(this.value, other.value);
        }

        public override int GetHashCode()
        {
            if (this.value == null)
                return 0;

            return this.value.GetHashCode();
        }
    }
}
