using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public static class ReservationsMaybe
    {
        public static IReservationsProgram<IMaybe<TResult>> SelectMany<T, TResult>(
            this IReservationsProgram<IMaybe<T>> source,
            Func<T, IReservationsProgram<IMaybe<TResult>>> selector)
        {
            return source.SelectMany(x => x.Accept(new SelectManyMaybeVisitor<T, TResult>(selector)));
        }

        private class SelectManyMaybeVisitor<T, TResult> :
            IMaybeVisitor<T, IReservationsProgram<IMaybe<TResult>>>
        {
            private readonly Func<T, IReservationsProgram<IMaybe<TResult>>> selector;

            public SelectManyMaybeVisitor(Func<T, IReservationsProgram<IMaybe<TResult>>> selector)
            {
                this.selector = selector;
            }

            public IReservationsProgram<IMaybe<TResult>> VisitNothing
            {
                get
                {
                    return new Pure<IMaybe<TResult>>(new Nothing<TResult>());
                }
            }

            public IReservationsProgram<IMaybe<TResult>> VisitJust(T just)
            {
                return this.selector(just);
            }
        }

        public static IReservationsProgram<IMaybe<TResult>> SelectMany<T, U, TResult>(
            this IReservationsProgram<IMaybe<T>> source,
            Func<T, IReservationsProgram<IMaybe<U>>> k,
            Func<T, U, TResult> s)
        {
            return source
                .SelectMany(x => k(x)
                    .SelectMany(y => new Pure<IMaybe<TResult>>(new Just<TResult>(s(x, y)))));
        }

        public static IReservationsProgram<IMaybe<TResult>> Select<T, TResult>(
            this IReservationsProgram<IMaybe<T>> source,
            Func<T, TResult> selector)
        {
            return source.SelectMany(x => new Pure<IMaybe<TResult>>(new Just<TResult>(selector(x))));
        }
    }
}
