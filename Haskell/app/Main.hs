module Main where

import Data.Time.Clock
import Control.Monad.Trans.Free (iterM, Free)
import Control.Monad.Trans.Maybe (runMaybeT)
import BookingApi (ReservationsInstruction(..), Reservation(..), tryAccept)

isReservationInFuture :: Reservation -> IO Bool
isReservationInFuture r = do
  now <- getCurrentTime
  return $ now < reservationDate r

-- Hard-coding most of it...
interpret :: Free ReservationsInstruction a -> IO a
interpret = iterM go
  where go (IsReservationInFuture r next) = isReservationInFuture r >>= next
        go (ReadReservations _ next) = next []
        go (Create _ next) = next 42

main :: IO ()
main = do
  now <- getCurrentTime
  let tomorrow = nominalDay `addUTCTime` now
  let r = Reservation tomorrow "mark@example.com" "Mark Seemann" 4 False
  print =<< interpret (runMaybeT $ tryAccept 10 r)
