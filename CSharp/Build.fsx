#r @"packages/FAKE.4.64.4/tools/FakeLib.dll"

open Fake

Target "Build" <| fun _ ->
    !! "**/BookingApi.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

RunTargetOrDefault "Build"
