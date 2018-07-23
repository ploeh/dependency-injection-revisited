module Ploeh.Samples.BookingApi.Reservations

open System

type Reservation = {
    Date : DateTimeOffset;
    Email : string;
    Name : string;
    Quantity : int;
    IsAccepted : bool }

type ReservationsInstruction<'a> =
| IsReservationInFuture of (Reservation * (bool -> 'a))
| ReadReservations of (DateTimeOffset * (Reservation list -> 'a))
| Create of (Reservation * (int -> 'a))

let private mapI f = function
    | IsReservationInFuture (x, next) -> IsReservationInFuture (x, next >> f)
    | ReadReservations (x, next) -> ReadReservations (x, next >> f)
    | Create (x, next) -> Create (x, next >> f)

type ReservationsProgram<'a> =
| Free of ReservationsInstruction<ReservationsProgram<'a>>
| Pure of 'a

let rec bind f = function
| Free x -> x |> mapI (bind f) |> Free
| Pure x -> f x

type ReservationsBuilder () =
    member this.Bind (x, f) = bind f x
    member this.Return x = Pure x
    member this.ReturnFrom x = x
    member this.Zero () = Pure ()

let reservations = ReservationsBuilder ()

let rec iter f = function
    | Pure x -> x
    | Free p -> f p |> iter f
