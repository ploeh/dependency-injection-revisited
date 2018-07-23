{-# LANGUAGE DeriveFunctor #-}
module BookingApi where

import Data.Time.Clock
import Control.Monad
import Control.Monad.Trans.Class
import Control.Monad.Trans.Maybe
import Control.Monad.Trans.Free

-- Domain model

data Reservation = Reservation
  { reservationDate :: UTCTime
  , reservationEmail :: String
  , reservationName :: String
  , reservationQuantity :: Int
  , reservationIsAccepted :: Bool }
  deriving (Show, Eq)

-- Instruction set

data ReservationsInstruction next =
    IsReservationInFuture Reservation (Bool -> next)
  | ReadReservations UTCTime ([Reservation] -> next)
  | Create Reservation (Int -> next)
  deriving Functor

-- Program

type ReservationsProgram = Free ReservationsInstruction

-- Lifts

isReservationInFuture :: MonadTrans t =>
                         Reservation -> t ReservationsProgram Bool
isReservationInFuture r = lift $ liftF $ IsReservationInFuture r id

readReservations :: MonadTrans t =>
                    UTCTime -> t ReservationsProgram [Reservation]
readReservations date = lift $ liftF $ ReadReservations date id

create :: MonadTrans t => Reservation -> t ReservationsProgram Int
create r = lift $ liftF $ Create r id

-- Maître'D

tryAccept :: Int -> Reservation -> MaybeT ReservationsProgram Int
tryAccept capacity reservation = do
  guard =<< isReservationInFuture reservation

  reservations <- readReservations $ reservationDate reservation
  let reservedSeats = sum $ reservationQuantity <$> reservations
  guard $ reservedSeats + reservationQuantity reservation <= capacity

  create $ reservation { reservationIsAccepted = True }
