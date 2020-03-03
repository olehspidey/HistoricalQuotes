﻿module PolygonHistorical

open System
open System.Net.Http
open Newtonsoft.Json
open Models

let BaseUrl = "https://api.polygon.io"
let ApiKey = "EUa_zmDAaJFc4cftqR2rww9wOhJbXdDJpX4anv"

let FormatDateToPolygon(date: DateTime) = 
    String.Format("{0}-{1}-{2}",
        date.Year.ToString(),
        (if date.Month > 10 then date.Month.ToString() else "0" + date.Month.ToString()),
        (if date.Day > 10 then date.Day.ToString() else "0" + date.Day.ToString()))

let GetHistoricalAsync(symbol: string, date: DateTime, offset: int64) =
    async {
        let httClient = new HttpClient()
        let fullUrl = 
            String.Format("{0}/{1}/{2}/{3}?apiKey={4}{5}&limit=10000",
                BaseUrl,
                "v2/ticks/stocks/nbbo",
                symbol,
                FormatDateToPolygon date,
                ApiKey,
                (if offset = int64(0) then "" else "&timestamp=" + offset.ToString()))
        let! gotResult = httClient.GetAsync fullUrl |> Async.AwaitTask
        
        if not gotResult.IsSuccessStatusCode 
            then printfn "Invalid response. Status code: %s" (gotResult.StatusCode.ToString())

        let! jsonResult = gotResult.Content.ReadAsStringAsync() |> Async.AwaitTask
        let qoutes = JsonConvert.DeserializeObject<HistoricalQuoteResponse> jsonResult
        
        //return System.Threading.Tasks.Task.FromResult(qoutes)
        return qoutes
    }