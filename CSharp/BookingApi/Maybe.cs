using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public static class Maybe
    {
        public static bool IsNothing<T>(this IMaybe<T> source)
        {
            return source.Accept(new IsNothingVisitor<T>());
        }

        private class IsNothingVisitor<T> : IMaybeVisitor<T, bool>
        {
            public bool VisitNothing => true;

            public bool VisitJust(T just)
            {
                return false;
            }
        }

        public static bool IsJust<T>(this IMaybe<T> source)
        {
            return !source.IsNothing();
        }

        public static T GetValueOrDefault<T>(this IMaybe<T> source, T defaultValue)
        {
            return source.Accept(new FromMaybeVisitor<T>(defaultValue));
        }

        private class FromMaybeVisitor<T> : IMaybeVisitor<T, T>
        {
            private readonly T defaultValue;

            public FromMaybeVisitor(T defaultValue)
            {
                this.defaultValue = defaultValue;
            }

            public T VisitNothing => this.defaultValue;

            public T VisitJust(T just)
            {
                return just;
            }
        }

        // Functor
        public static IMaybe<TResult> Select<T, TResult>(
            this IMaybe<T> source,
            Func<T, TResult> selector)
        {
            return source.Accept(new SelectMaybeVisitor<T, TResult>(selector));
        }

        private class SelectMaybeVisitor<T, TResult> :
            IMaybeVisitor<T, IMaybe<TResult>>
        {
            private readonly Func<T, TResult> selector;

            public SelectMaybeVisitor(Func<T, TResult> selector)
            {
                this.selector = selector;
            }

            public IMaybe<TResult> VisitNothing
            {
                get { return new Nothing<TResult>(); }
            }

            public IMaybe<TResult> VisitJust(T just)
            {
                return new Just<TResult>(this.selector(just));
            }
        }

        // Monad
        public static IMaybe<TResult> SelectMany<T, TResult>(
            this IMaybe<T> source,
            Func<T, IMaybe<TResult>> selector)
        {
            return source.Accept(
                new SelectManyMaybeVisitor<T, TResult>(selector));
        }

        private class SelectManyMaybeVisitor<T, TResult> :
            IMaybeVisitor<T, IMaybe<TResult>>
        {
            private readonly Func<T, IMaybe<TResult>> selector;

            public SelectManyMaybeVisitor(Func<T, IMaybe<TResult>> selector)
            {
                this.selector = selector;
            }

            public IMaybe<TResult> VisitNothing
            {
                get { return new Nothing<TResult>(); }
            }

            public IMaybe<TResult> VisitJust(T just)
            {
                return this.selector(just);
            }
        }

        public static IMaybe<TResult> SelectMany<T, U, TResult>(
            this IMaybe<T> source,
            Func<T, IMaybe<U>> k,
            Func<T, U, TResult> s)
        {
            return source
                .SelectMany(x => k(x)
                    .SelectMany(y => new Just<TResult>(s(x, y))));
        }
    }
}
