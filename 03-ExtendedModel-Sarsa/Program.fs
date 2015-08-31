module Program

open System

//-------------------------------------------------------------------------------------------------

let random = Random()

let alphaU = 0.25

let states, output = Compute.executeExperiment random alphaU
let endOfEpisodeProfitSum, endOfEpisodeInventory, averagePriceDeviation, averageEpisodicSpread = output

Chart.renderPrices @"..\..\..\ExtendedModel-Prices-025.png" states.[025]
Chart.renderPrices @"..\..\..\ExtendedModel-Prices-100.png" states.[100]
Chart.renderPrices @"..\..\..\ExtendedModel-Prices-200.png" states.[200]
Chart.renderPrices @"..\..\..\ExtendedModel-Prices-500.png" states.[500]

Chart.renderEndOfEpisodeProfitSum @"..\..\..\ExtendedModel-EndOfEpisodeProfitSum.png" endOfEpisodeProfitSum 0.0
Chart.renderEndOfEpisodeInventory @"..\..\..\ExtendedModel-EndOfEpisodeInventory.png" endOfEpisodeInventory 0.0
Chart.renderAveragePriceDeviation @"..\..\..\ExtendedModel-AveragePriceDeviation.png" averagePriceDeviation 0.0
Chart.renderAverageEpisodicSpread @"..\..\..\ExtendedModel-AverageEpisodicSpread.png" averageEpisodicSpread 0.0
