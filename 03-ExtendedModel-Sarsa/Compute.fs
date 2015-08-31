module Compute

open MathNet.Numerics.Distributions

//-------------------------------------------------------------------------------------------------

let alphaP = 0.2

let timeSteps = 250
let episodes = 1000

let epsilon = 0.1
let gamma = 0.9
let alpha = 1.0 / float timeSteps

let wPro = 0.8
let wInv = 0.1
let wQlt = 0.1

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

let private getValue (values : float[,,,]) state action =

    let (_, pBid, pAsk, _, imb, _) = state
    let spread = pAsk - pBid
    let actionBid = fst action
    let actionAsk = snd action
    values.[imb + 1, spread - 1, actionBid + 1, actionAsk + 1]

let private setValue (values : float[,,,]) state action value =

    let (_, pBid, pAsk, _, imb, _) = state
    let spread = pAsk - pBid
    let actionBid = fst action
    let actionAsk = snd action
    values.[imb + 1, spread - 1, actionBid + 1, actionAsk + 1] <- value

let private getProfit state =
    let (_, _, _, _, _, profit) = state
    profit

let private computeReward state state' =
    let profit = float (getProfit state' - getProfit state)
    let (_, pBid, pAsk, inv, _, _) = state'
    (wPro * profit) - (wInv * float (abs inv)) - (wQlt * float (pAsk - pBid))

//-------------------------------------------------------------------------------------------------

let private actions =
    [| (-1, -1); (-1,  0); (-1, +1)
       ( 0, -1); ( 0,  0); ( 0, +1)
       (+1, -1); (+1,  0); (+1, +1) |]

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

let executeAction random alphaU state action =

    let (pStar, pBid, pAsk, inv, imb, profit) = state

    let (pBid, pAsk, imb) =
        let actionBid, actionAsk = action
        let pBid = pBid + actionBid
        let pAsk = pAsk + actionAsk
        let pBid = if pBid > (pAsk - 1) then (pAsk - 1) else pBid
        let pAsk = if pAsk > (pBid + 4) then (pBid + 4) else pAsk
        (pBid, pAsk, 0)

    match event random alphaU with
    | ChangePriceStarPos
        -> (pStar + 1, pBid, pAsk, inv, imb, profit)
    | ChangePriceStarNeg
        -> (pStar - 1, pBid, pAsk, inv, imb, profit)
    | TradeUninformedPos
        -> (pStar, pBid, pAsk, inv - 1, imb + 1, profit + (pAsk - pStar))
    | TradeUninformedNeg
        -> (pStar, pBid, pAsk, inv + 1, imb - 1, profit - (pBid - pStar))
    | TradeInformed when (pStar > pAsk)
        -> (pStar, pBid, pAsk, inv - 1, imb + 1, profit + (pAsk - pStar))
    | TradeInformed when (pStar < pBid)
        -> (pStar, pBid, pAsk, inv + 1, imb - 1, profit - (pBid - pStar))
    | TradeInformed
        -> (pStar, pBid, pAsk, inv, imb, profit)

let executeOneTimeStep random explore alphaU (values, state, action) =

    let state' = executeAction random alphaU state action
    let action' = selectAction random explore values state'
    let reward = computeReward state state'
    let qNext = getValue values state' action'
    let q = getValue values state action
    let q = q + alpha * (reward + (gamma * qNext) - q)

    setValue values state action q

    (values, state'), (values, state', action')

let private executeOneEpisode random explore alphaU values =

    let state = (0, -1, +1, 0, 0, 0)
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
    let values = Array4D.create 3 4 3 3 0.0

    values
    |> Seq.unfold (executeOneEpisode random explore alphaU >> Some)
    |> Seq.take episodes

//-------------------------------------------------------------------------------------------------

let private computeEndOfEpisodeProfitSum states =
    let (pStar, pBid, pAsk, inv, imb, profit) = Array.last states
    profit

let private computeEndOfEpisodeInventory states =
    let (pStar, pBid, pAsk, inv, imb, profit) = Array.last states
    inv

let private computeAveragePriceDeviation states =
    states
    |> Seq.map (fun (pStar, pBid, pAsk, _, _, _) -> (abs (pBid - pStar)) + (abs (pAsk - pStar)) |> float)
    |> Seq.average

let private computeAverageEpisodicSpread states =
    states
    |> Seq.map (fun (pStar, pBid, pAsk, _, _, _) -> (pAsk - pBid) |> float)
    |> Seq.average

//-------------------------------------------------------------------------------------------------

let executeExperiment random alphaU =

    let mapping states =
        let endOfEpisodeProfitSum = computeEndOfEpisodeProfitSum states
        let endOfEpisodeInventory = computeEndOfEpisodeInventory states
        let averagePriceDeviation = computeAveragePriceDeviation states
        let averageEpisodicSpread = computeAverageEpisodicSpread states
        states, (endOfEpisodeProfitSum, endOfEpisodeInventory, averagePriceDeviation, averageEpisodicSpread)

    let unzip4 array =
        let array1 = array |> Array.map (fun (x, _, _, _) -> x)
        let array2 = array |> Array.map (fun (_, x, _, _) -> x)
        let array3 = array |> Array.map (fun (_, _, x, _) -> x)
        let array4 = array |> Array.map (fun (_, _, _, x) -> x)
        (array1, array2, array3, array4)

    let states, output =
        executeOneSession random alphaU
        |> Seq.map mapping
        |> Seq.toArray
        |> Array.unzip

    states, unzip4 output
