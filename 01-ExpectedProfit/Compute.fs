module Compute

open MathNet.Numerics.Distributions

//-------------------------------------------------------------------------------------------------

let alphaP = 0.2

let noiseMin = 0.0
let noiseMax = 2.0
let noiseStep = 0.05

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

let private execute random imbThreshold alphaU (pStar, pMark, inv, imb, profit) =

    let (pMark, imb) =
        match imb with
        | imb when imb >= +imbThreshold -> pMark + 1, 0
        | imb when imb <= -imbThreshold -> pMark - 1, 0
        | imb -> pMark, imb

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

//-------------------------------------------------------------------------------------------------

let generateStates random imbThreshold alphaU =

    let pairResult x = Some (x, x)
    Seq.unfold (execute random imbThreshold alphaU >> pairResult) (0, 0, 0, 0, 0)

let computeResults random iterations imbThreshold =

    let computeExpectedProfit alphaU =
        let (_, _, _, _, profit) = generateStates random imbThreshold alphaU |> Seq.item iterations
        let expectedProfit = float profit / float iterations
        alphaU, expectedProfit

    [ noiseMin .. noiseStep .. noiseMax ] |> List.map computeExpectedProfit
