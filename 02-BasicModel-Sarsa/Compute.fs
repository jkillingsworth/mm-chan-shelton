module Compute

open MathNet.Numerics.Distributions

//-------------------------------------------------------------------------------------------------

let alphaP = 0.2

let timeSteps = 250
let episodes = 2000

let epsilon = 0.1
let gamma = 0.9
let alpha = 1.0 / float timeSteps

//-------------------------------------------------------------------------------------------------

type private Event =
    | ChangePriceStarPos
    | ChangePriceStarNeg
    | TradeUninformedPos
    | TradeUninformedNeg
    | TradeInformed

let private event random alphaU =

    let lambdaI = 1.0 / ((2.0 * alphaP) + (2.0 * alphaU) + 1.0)
    let lambdaP = alphaP * lambdaI
    let lambdaU = alphaU * lambdaI

    match Sample.continuousUniform 0.0 1.0 random with
    | x when x < (lambdaP * 1.0) -> ChangePriceStarPos
    | x when x < (lambdaP * 2.0) -> ChangePriceStarNeg
    | x when x < (lambdaP * 2.0 + lambdaU * 1.0) -> TradeUninformedPos
    | x when x < (lambdaP * 2.0 + lambdaU * 2.0) -> TradeUninformedNeg
    | x -> TradeInformed

//-------------------------------------------------------------------------------------------------

let private getValue (values : float[,]) state action =

    let (_, _, _, imb, _) = state
    values.[imb + 3, action + 1]

let private setValue (values : float[,]) state action value =

    let (_, _, _, imb, _) = state
    values.[imb + 3, action + 1] <- value

let private getProfit state =
    let (_, _, _, _, profit) = state
    profit

let private computeReward state state' =
    float (getProfit state' - getProfit state)

//-------------------------------------------------------------------------------------------------

let private actions = [| -1; 0; +1 |]

let private selectAction random explore values state =

    let explore = explore && Sample.continuousUniform 0.0 1.0 random < epsilon
    if (explore) then
        let index = Sample.discreteUniform 0 (actions.Length - 1) random
        actions.[index]
    else
        actions
        |> Array.map (fun action -> action, getValue values state action)
        |> Array.maxBy (fun (action, value) -> value)
        |> fst

//-------------------------------------------------------------------------------------------------

let private executeAction random alphaU state action =

    let (pStar, pMark, inv, imb, profit) = state

    let (pMark, imb) =
        match action, imb with
        | +1, _ -> pMark + 1, 0
        | -1, _ -> pMark - 1, 0
        | _, +3 -> pMark + 1, 0
        | _, -3 -> pMark - 1, 0
        | _  -> pMark, imb

    match event random alphaU with
    | ChangePriceStarPos
        -> (pStar + 1, pMark, inv, imb, profit)
    | ChangePriceStarNeg
        -> (pStar - 1, pMark, inv, imb, profit)
    | TradeUninformedPos
        -> (pStar, pMark, inv - 1, imb + 1, profit + (pMark - pStar))
    | TradeUninformedNeg
        -> (pStar, pMark, inv + 1, imb - 1, profit - (pMark - pStar))
    | TradeInformed when (pStar > pMark)
        -> (pStar, pMark, inv - 1, imb + 1, profit + (pMark - pStar))
    | TradeInformed when (pStar < pMark)
        -> (pStar, pMark, inv + 1, imb - 1, profit - (pMark - pStar))
    | TradeInformed
        -> (pStar, pMark, inv, imb, profit)

let private executeOneTimeStep random explore alphaU (values, state, action) =

    let state' = executeAction random alphaU state action
    let action' = selectAction random explore values state'
    let reward = computeReward state state'
    let qNext = getValue values state' action'
    let q = getValue values state action
    let q = q + alpha * (reward + (gamma * qNext) - q)

    setValue values state action q

    (values, state'), (values, state', action')

let private executeOneEpisode random explore alphaU values =

    let state = (0, 0, 0, 0, 0)
    let action = selectAction random explore values state

    let results =
        (values, state, action)
        |> Seq.unfold (executeOneTimeStep random explore alphaU >> Some)
        |> Seq.take timeSteps
        |> Seq.toArray
        |> Array.unzip

    let values = fst results |> Array.last
    let states = snd results

    states, values

let private executeOneSession random alphaU =

    let explore = true
    let values = Array2D.create 7 3 0.0

    values
    |> Seq.unfold (executeOneEpisode random explore alphaU >> Some)
    |> Seq.take episodes

//-------------------------------------------------------------------------------------------------

let private computeEndOfEpisodeProfitSum states =
    let (pStar, pMark, inv, imb, profit) = Array.last states
    profit

let private computeEndOfEpisodeInventory states =
    let (pStar, pMark, inv, imb, profit) = Array.last states
    inv

let private computeAveragePriceDeviation states =
    states
    |> Seq.map (fun (pStar, pMark, _, _, _) -> (pMark - pStar) |> abs |> float)
    |> Seq.average

//-------------------------------------------------------------------------------------------------

let executeExperiment random alphaU =

    let mapping states =
        let endOfEpisodeProfitSum = computeEndOfEpisodeProfitSum states
        let endOfEpisodeInventory = computeEndOfEpisodeInventory states
        let averagePriceDeviation = computeAveragePriceDeviation states
        states, (endOfEpisodeProfitSum, endOfEpisodeInventory, averagePriceDeviation)

    let states, output =
        executeOneSession random alphaU
        |> Seq.map mapping
        |> Seq.toArray
        |> Array.unzip

    states, Array.unzip3 output
