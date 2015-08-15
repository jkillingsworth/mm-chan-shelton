module Program

open System

//-------------------------------------------------------------------------------------------------

let random = Random()

let series = Compute.generateSeries random 1 0.4 |> Seq.take 1000 |> Seq.toList

let imbThresholds = [ 1; 2; 3 ]

let result =
    imbThresholds
    |> List.map (Compute.computeResults random 1000000)
    |> List.zip imbThresholds

Chart.renderSeries @"..\..\..\ExpectedProfit-Series.png" series
Chart.renderResult @"..\..\..\ExpectedProfit-Result.png" result
