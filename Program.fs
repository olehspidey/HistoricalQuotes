// Learn more about F# at http://fsharp.org

open System
open PolygonHistorical
open Models
open CsvHelper
open System.IO
open System.Net.Http
open System.Threading.Tasks

let WriteToFile (data, path : string) = 
    use writer = new StreamWriter(path)
    use csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture)

    data
    |> csvWriter.WriteRecordsAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously

let ConvertUnitDateToDateTime (unixTime: int64) = 
    new DateTime(unixTime / 100L + 621355968000000000L)

let ConvertDateToString (date: DateTime) = 
    date.ToString("dd-MM-yyyy")

let TryCreateDirectory (path: string) = 
    if not (Directory.Exists(path)) then 
        Directory.CreateDirectory(path) |> ignore
    path

let Proccess (day: int, stock: string, httpClient: HttpClient option) = 
    let mutable paginatedHistorical = 
        GetHistoricalAsync(stock, DateTime.Now.AddDays(float(-day)), 0L, httpClient) 
        |> Async.RunSynchronously
    let mutable totalDayList = (paginatedHistorical).Results

    while paginatedHistorical.Results.Length > 10 do
        let lastTime = List.last(totalDayList).Time

        paginatedHistorical <- GetHistoricalAsync("AAPL", DateTime.Now.AddDays(float(-day)), lastTime, httpClient) |> Async.RunSynchronously
        totalDayList <- List.append totalDayList paginatedHistorical.Results
        Console.WriteLine("Found {0}. Total: {1}", lastTime |> ConvertUnitDateToDateTime, totalDayList.Length)

    Console.WriteLine("Searched for {0} day", day)

    if totalDayList.Length > 0 then
        WriteToFile(
            totalDayList,
            sprintf "%s.csv" ((((Directory.GetCurrentDirectory(), "AAPL")
            |> Path.Combine
            |> TryCreateDirectory), List.last(totalDayList).Time |> ConvertUnitDateToDateTime |> ConvertDateToString)
                    |> Path.Combine))
    else
        Console.WriteLine(sprintf "No historical for %s" (DateTime.Now.AddDays(float(-day)) |> string))
    Console.WriteLine("Writed to file")

[<EntryPoint>]
let main _ =
    let httpClient = new HttpClient()
    let totalDays = int((DateTime.Now.AddDays -1.0 - DateTime.Now.AddYears -1).TotalDays)
    let watcher = new System.Diagnostics.Stopwatch()

    watcher.Start()

    for day = 1 to totalDays + 1 do
       let task = new Task((fun _ -> (day, "AAPL", option.None) |> Proccess), Threading.CancellationToken.None)

       task.Start()
    watcher.Stop()
    printf "Total time %s" (watcher.Elapsed.ToString())
    Console.ReadLine() |> ignore
    0 // return an integer exit code
