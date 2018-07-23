module Ploeh.Samples.BookingApi.UnitTests.Tests

open System
open Xunit
open Hedgehog
open Swensen.Unquote
open Ploeh.Samples.BookingApi
open Reservations
open Booking
open Ploeh.Samples.BookingApi.UnitTests

[<Fact>]
let ``tryAccept reservation in the past`` () = Property.check <| property {
    let! capacity = Range.linear 0 Int32.MaxValue |> Gen.int
    let! reservation = Gen.reservation
    let stub = function
        | IsReservationInFuture (_, next) -> next false
        | ReadReservations (_, next) -> next []
        | Create (_, next) -> next 0
    
    let actual = tryAccept capacity reservation |> iter stub

    test <@ Option.isNone actual @> }

[<Fact>]
let ``tryAccept reservation when capacity is insufficient`` () = Property.check <| property {
    let! i = Range.linear 0 Int32.MaxValue |> Gen.int
    let! reservations = Gen.reservation |> Gen.list (Range.linear 0 100)
    let! reservation = Gen.reservation
    let stub = function
        | IsReservationInFuture (_, next) -> next true
        | ReadReservations (_, next) -> next reservations
        | Create (_, next) -> next 0
    let reservedSeats = List.sumBy (fun r -> r.Quantity) reservations
    let capacity = reservedSeats + reservation.Quantity - i

    let actual = tryAccept capacity reservation |> iter stub

    test <@ Option.isNone actual @> }

[<Fact>]
let ``tryAccept happy path`` () = Property.check <| property {
    let! i = Range.linear 1 Int32.MaxValue |> Gen.int
    let! reservations = Gen.reservation |> Gen.list (Range.linear 0 100)
    let! reservation = Gen.reservation
    let! expected = Range.linearBounded () |> Gen.int
    let stub = function
        | IsReservationInFuture (_, next) -> next true
        | ReadReservations (_, next) -> next reservations
        | Create (r, next) -> test <@ r.IsAccepted @>; next expected
    let reservedSeats = List.sumBy (fun r -> r.Quantity) reservations
    let capacity = reservedSeats + reservation.Quantity + i

    let actual = tryAccept capacity reservation |> iter stub

    Some expected =! actual }