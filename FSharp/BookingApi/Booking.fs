module Ploeh.Samples.BookingApi.Booking

open Ploeh.Samples.BookingApi
open Reservations

let isReservationInFuture r = Free (IsReservationInFuture (r, Pure))

let readReservations d = Free (ReadReservations (d, Pure))

let create r = Free (Create (r, Pure))

// int -> Reservation -> ReservationsProgram<int option>
let tryAccept capacity reservation = reservations {
    let! isInFuture = isReservationInFuture reservation

    if not isInFuture
    then return None
    else    
        let! reservations = readReservations reservation.Date
        let reservedSeats = List.sumBy (fun r -> r.Quantity) reservations

        if (capacity < reservedSeats + reservation.Quantity)
        then return None
        else
            let! reservationId = create { reservation with IsAccepted = true }
            return Some reservationId }