namespace Models

open Newtonsoft.Json

type HistoricalQuote = {
    [<JsonProperty("T")>]
    Name : string
    [<JsonProperty("t")>]
    Time : int64
    [<JsonProperty("P")>]
    Ask : double
    [<JsonProperty("p")>]
    Bid : double
}

type HistoricalQuoteResponse = {
    Results : list<HistoricalQuote>
}