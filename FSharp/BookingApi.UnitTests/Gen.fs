module Ploeh.Samples.BookingApi.UnitTests.Gen

open System
open Hedgehog
open Ploeh.Samples.BookingApi.Reservations

let reservation = gen {
    let! ticks =
        Range.linear DateTime.MinValue.Ticks DateTime.MaxValue.Ticks
        |> Gen.int64
    let! offset = Range.linear -12 12 |> Gen.int |> Gen.map float
    let d = new DateTimeOffset (ticks, TimeSpan.FromHours offset)

    let! name = Gen.alphaNum |> Gen.string (Range.linear 1 50)
    let! email = Gen.alphaNum |> Gen.string (Range.linear 1 50)
    let! quantity = Range.linear 1 100 |> Gen.int
    let! isAccepted = Gen.bool

    return {
        Date = d;
        Name = name;
        Email = email;
        Quantity = quantity;
        IsAccepted = isAccepted } }

