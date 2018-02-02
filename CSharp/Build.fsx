#r @"packages/FAKE.4.64.4/tools/FakeLib.dll"

open Fake
open Fake.Testing

Target "Build" <| fun _ ->
    !! "**/BookingApi.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

Target "Test" <| fun _ ->
    !! "*/bin/Release/Ploeh.Samples.*Tests.dll"
    |> xUnit2 (fun p -> { p with Parallel = ParallelMode.All })

"Build"
==> "Test"

RunTargetOrDefault "Test"
