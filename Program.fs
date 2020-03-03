// Learn more about F# at http://fsharp.org

open System
open PolygonHistorical

[<EntryPoint>]
let main argv =
    let totalDays = int((DateTime.Now.AddDays -1.0 - DateTime.Now.AddYears -3).TotalDays)

    for i = 1 to totalDays do
        let mutable resultQuotes = GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-i)), 0L) |> Async.RunSynchronously

        while resultQuotes.Results.Length <> 0 do
            let lastTime = List.last(resultQuotes.Results).Time 

            resultQuotes <- GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-i)), lastTime) |> Async.RunSynchronously
            Console.WriteLine("Found {0}", new DateTime(lastTime / 100L + 621355968000000000L))

        if resultQuotes.Results.Length = 0 then printfn "Yes"
    0 // return an integer exit code
