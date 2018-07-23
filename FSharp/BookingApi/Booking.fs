module Ploeh.Samples.BookingApi.Booking

open Ploeh.Samples.BookingApi
open Reservations
open ReservationsOption

let isReservationInFuture r = Free (IsReservationInFuture (r, Some >> Pure))

let readReservations d = Free (ReadReservations (d, Some >> Pure))

let create r = Free (Create (r, Some >> Pure))

// bool -> ReservationsProgram<unit option>
let guard = function
    | true -> Pure (Some ())
    | false -> Pure None

// int -> Reservation -> ReservationsProgram<int option>
let tryAccept capacity reservation = reservationsOption {
    do! ReservationsOption.bind guard <| isReservationInFuture reservation

    let! reservations = readReservations reservation.Date
    let reservedSeats = List.sumBy (fun r -> r.Quantity) reservations
    do! guard (reservedSeats + reservation.Quantity <= capacity)

    return! create { reservation with IsAccepted = true } }
