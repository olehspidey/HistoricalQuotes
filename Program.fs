// Learn more about F# at http://fsharp.org

open System
open PolygonHistorical
open Models

[<EntryPoint>]
let main _ =
    let httpClient = new System.Net.Http.HttpClient()
    let totalDays = int((DateTime.Now.AddDays -1.0 - DateTime.Now.AddYears -3).TotalDays)

    for i = 1 to totalDays do
        let mutable totalDayList = (GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-i)), 0L, option.Some httpClient) |> Async.RunSynchronously).Results
        let mutable paginatedHistorical = GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-i)), 0L, option.Some httpClient) |> Async.RunSynchronously
        
        while paginatedHistorical.Results.Length > 10 do
            let lastTime = List.last(totalDayList).Time

            paginatedHistorical <- GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-i)), lastTime, option.Some httpClient) |> Async.RunSynchronously
            
            totalDayList <- List.append totalDayList paginatedHistorical.Results
            Console.WriteLine("Found {0}. Total: {1}", new DateTime(lastTime / 100L + 621355968000000000L), totalDayList.Length)

            if totalDayList.Length = 0 then printfn "Yes"
        |> ignore
    0 // return an integer exit code
