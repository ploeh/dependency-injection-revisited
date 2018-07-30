module Ploeh.Samples.BookingApi.ReservationsOption

open Ploeh.Samples.BookingApi.Reservations

// ('a -> ReservationsProgram<'b option>) -> ReservationsProgram<'a option> ->
//     ReservationsProgram<'b option>
let bind f x = reservations {
    let! x' = x
    match x' with
    | Some x'' -> return! f x''
    | None -> return None }
    
type ReservationsOptionBuilder () =
    member this.Bind (x, f) = bind f x
    member this.Return x = Pure (Some x)
    member this.ReturnFrom x = x
    member this.Zero () = Pure (Some ())

let reservationsOption = ReservationsOptionBuilder ()
