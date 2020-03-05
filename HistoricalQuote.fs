namespace Models

open Newtonsoft.Json

type HistoricalQuote = {
    [<JsonProperty("T")>]
    Time : int64
    [<JsonProperty("P")>]
    Ask : double
    [<JsonProperty("p")>]
    Bid : double
}

type HistoricalQuoteResponse = {
    Results : list<HistoricalQuote>
}