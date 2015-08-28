module Program

open System

//-------------------------------------------------------------------------------------------------

let random = Random()

let alphaU = 0.25

let expectedProfitSum = -0.361333 * float Compute.timeSteps
let expectedInventory = 0.0
let expectedDeviation = 1.0

let states, output = Compute.executeExperiment random alphaU
let endOfEpisodeProfitSum, endOfEpisodeInventory, averagePriceDeviation = output

Chart.renderPrices @"..\..\..\BasicModel-Prices-025.png" states.[025]
Chart.renderPrices @"..\..\..\BasicModel-Prices-100.png" states.[100]
Chart.renderPrices @"..\..\..\BasicModel-Prices-200.png" states.[200]
Chart.renderPrices @"..\..\..\BasicModel-Prices-500.png" states.[500]

Chart.renderEndOfEpisodeProfitSum @"..\..\..\BasicModel-EndOfEpisodeProfitSum.png" endOfEpisodeProfitSum expectedProfitSum
Chart.renderEndOfEpisodeInventory @"..\..\..\BasicModel-EndOfEpisodeInventory.png" endOfEpisodeInventory expectedInventory
Chart.renderAveragePriceDeviation @"..\..\..\BasicModel-AveragePriceDeviation.png" averagePriceDeviation expectedDeviation
