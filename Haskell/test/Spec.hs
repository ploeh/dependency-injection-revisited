{-# LANGUAGE ViewPatterns #-}
import Data.Maybe
import Test.Framework (defaultMain, Test)
import Test.Framework.Providers.QuickCheck2 (testProperty)
import Test.QuickCheck
import Test.QuickCheck.Instances ()
import Control.Monad.Trans.Free (iter, foldFreeT)
import Control.Monad.Trans.Maybe (runMaybeT)
import Control.Monad.Trans.Writer.Lazy
import BookingApi (ReservationsInstruction(..), Reservation(..), tryAccept)

newtype ArbReservation = ArbReservation { getReservation :: Reservation }
  deriving (Show, Eq)

instance Arbitrary ArbReservation where
  arbitrary = do
    (d, e, n, Positive q, b) <- arbitrary
    return $ ArbReservation $ Reservation d e n q b

main :: IO ()
main = defaultMain tests

tests :: [Test]
tests =
  [ testProperty "tryAccept reservation in the past" $ \
      (Positive capacity) (ArbReservation reservation)
      ->
      let stub (IsReservationInFuture _ next) = next False
          stub (ReadReservations _ next) = next []
          stub (Create _ next) = next 0

          actual = iter stub $ runMaybeT $ tryAccept capacity reservation
          
      in  isNothing actual
    ,
    testProperty "tryAccept reservation when capacity is insufficient" $ \
      (Positive i)
      (fmap getReservation -> reservations)
      (ArbReservation reservation)
      ->
      let stub (IsReservationInFuture _ next) = next True
          stub (ReadReservations _ next) = next reservations
          stub (Create _ next) = next 0
          reservedSeats = sum $ reservationQuantity <$> reservations
          capacity = reservedSeats + reservationQuantity reservation - i

          actual = iter stub $ runMaybeT $ tryAccept capacity reservation

      in  isNothing actual
    ,
    testProperty "tryAccept, happy path" $ \
      (NonNegative i)
      (fmap getReservation -> reservations)
      (ArbReservation reservation)
      expected
      ->
      let spy (IsReservationInFuture _ next) = return $ next True
          spy (ReadReservations _ next) = return $ next reservations
          spy (Create r next) = tell [r] >> return (next expected)
          reservedSeats = sum $ reservationQuantity <$> reservations
          capacity = reservedSeats + reservationQuantity reservation + i

          (actual, observedReservations) =
            runWriter $ foldFreeT spy $ runMaybeT $ tryAccept capacity reservation

      in  Just expected == actual &&
          [True] == (reservationIsAccepted <$> observedReservations)
  ]