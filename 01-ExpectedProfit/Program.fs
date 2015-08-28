module Program

open System

//-------------------------------------------------------------------------------------------------

let random = Random()

let states = Compute.generateStates random 1 0.4 |> Seq.take 1000 |> Seq.toArray

let imbThresholds = [ 1; 2; 3 ]

let result =
    imbThresholds
    |> List.map (Compute.computeResults random 1000000)
    |> List.zip imbThresholds

Chart.renderPrices @"..\..\..\ExpectedProfit-Prices.png" states
Chart.renderResult @"..\..\..\ExpectedProfit-Result.png" result
