using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public static class ReservationsProgram
    {
        // Functor
        public static IReservationsProgram<TResult> Select<T, TResult>(
            this IReservationsProgram<T> source,
            Func<T, TResult> selector)
        {
            return source.SelectMany(x => new Pure<TResult>(selector(x)));
        }

        // Monad
        public static IReservationsProgram<TResult> SelectMany<T, TResult>(
            this IReservationsProgram<T> source,
            Func<T, IReservationsProgram<TResult>> selector)
        {
            return source.Accept(
                new SelectManyReservationsProgramVisitor<T, TResult>(
                    selector));
        }

        private class SelectManyReservationsProgramVisitor<T, TResult> :
            IReservationsProgramVisitor<T, IReservationsProgram<TResult>>
        {
            private readonly Func<T, IReservationsProgram<TResult>> selector;

            public SelectManyReservationsProgramVisitor(
                Func<T, IReservationsProgram<TResult>> selector)
            {
                this.selector = selector;
            }

            public IReservationsProgram<TResult> VisitFree(
                IReservationsInstruction<IReservationsProgram<T>> i)
            {
                return new Free<TResult>(i.Select(p => p.SelectMany(selector)));
            }

            public IReservationsProgram<TResult> VisitPure(T x)
            {
                return selector(x);
            }
        }

        // Lifts
        public static IReservationsProgram<IMaybe<bool>> IsReservationInFuture(Reservation reservation)
        {
            return new Free<IMaybe<bool>>(
                new IsReservationInFuture<IReservationsProgram<IMaybe<bool>>>(
                    reservation,
                    x => new Pure<IMaybe<bool>>(new Just<bool>(x))));
        }

        public static IReservationsProgram<IMaybe<IReadOnlyCollection<Reservation>>> ReadReservations(
            DateTimeOffset date)
        {
            return new Free<IMaybe<IReadOnlyCollection<Reservation>>>(
                new ReadReservations<IReservationsProgram<IMaybe<IReadOnlyCollection<Reservation>>>>(
                    date,
                    x => new Pure<IMaybe<IReadOnlyCollection<Reservation>>>(new Just<IReadOnlyCollection<Reservation>>(x))));
        }

        public static IReservationsProgram<IMaybe<int>> Create(Reservation reservation)
        {
            return new Free<IMaybe<int>>(
                new Create<IReservationsProgram<IMaybe<int>>>(
                    reservation,
                    x => new Pure<IMaybe<int>>(new Just<int>(x))));
        }

        public static IReservationsProgram<IMaybe<Unit>> Guard(bool b)
        {
            if (b)
                return new Pure<IMaybe<Unit>>(new Just<Unit>(Unit.Instance));
            else
                return new Pure<IMaybe<Unit>>(new Nothing<Unit>());
        }

        public static IReservationsProgram<IMaybe<Unit>> Do(Action action)
        {
            action();
            return new Pure<IMaybe<Unit>>(new Just<Unit>(Unit.Instance));
        }
    }
}
