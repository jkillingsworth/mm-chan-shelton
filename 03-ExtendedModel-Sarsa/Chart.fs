module Chart

open OxyPlot
open OxyPlot.Axes
open OxyPlot.Series

//-------------------------------------------------------------------------------------------------

let private exportToPng path w h model =

    use writeStream = System.IO.File.OpenWrite(path)
    let pngExporter = OxyPlot.WindowsForms.PngExporter()
    pngExporter.Width <- w
    pngExporter.Height <- h
    pngExporter.Export(model, writeStream)

let private defaultColorsToUseForPlots =

    [| OxyColors.Red
       OxyColors.Blue
       OxyColors.Green
       OxyColors.Orange
       OxyColors.Purple
       OxyColors.Teal |]

let private renderEpisode path data expected configureAxisY =

    let model = PlotModel()

    model.DefaultColors <- defaultColorsToUseForPlots
    model.LegendBackground <- OxyColors.White
    model.LegendBorder <- OxyColors.Gray
    model.LegendBorderThickness <- 1.0
    model.LegendPlacement <- LegendPlacement.Inside
    model.LegendPosition <- LegendPosition.RightTop
    model.PlotMargins <- OxyThickness(nan, nan, 10.0, nan)

    let axis = LinearAxis()
    axis.Title <- "Episode"
    axis.Position <- AxisPosition.Bottom
    axis.Minimum <- 0.0
    axis.Maximum <- (float Compute.episodes)
    axis.MajorStep <- (float Compute.episodes) / 10.0
    axis.MinorStep <- (float Compute.episodes) / 20.0
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    model.Axes.Add(axis)

    let axis = LinearAxis()
    axis.Position <- AxisPosition.Left
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    axis.MinorGridlineColor <- OxyColors.LightGray
    axis.MinorGridlineStyle <- LineStyle.Dot
    model.Axes.Add(axis)
    configureAxisY axis

    let series = LineSeries()
    series.Title <- "Expected"
    series.StrokeThickness <- 1.0
    data
    |> Array.mapi (fun i x -> DataPoint(float i, expected))
    |> Array.iter series.Points.Add
    model.Series.Add(series)

    let series = LineSeries()
    series.Title <- "Actual"
    series.StrokeThickness <- 1.0
    data
    |> Array.mapi (fun i x -> DataPoint(float i, x))
    |> Array.iter series.Points.Add
    model.Series.Add(series)

    model |> exportToPng path 700 400

//-------------------------------------------------------------------------------------------------

let renderPrices path data =

    let model = PlotModel()

    model.DefaultColors <- defaultColorsToUseForPlots
    model.LegendBackground <- OxyColors.White
    model.LegendBorder <- OxyColors.Gray
    model.LegendBorderThickness <- 1.0
    model.LegendPlacement <- LegendPlacement.Inside
    model.LegendPosition <- LegendPosition.RightTop
    model.PlotMargins <- OxyThickness(nan, nan, 10.0, nan)

    let axis = LinearAxis()
    axis.Title <- "Time"
    axis.Position <- AxisPosition.Bottom
    axis.Minimum <- 0.0
    axis.Maximum <- 250.0
    axis.MajorStep <- 50.0
    axis.MinorStep <- 10.0
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    model.Axes.Add(axis)

    let axis = LinearAxis()
    axis.Title <- "Price"
    axis.Position <- AxisPosition.Left
    axis.Minimum <- -15.0
    axis.Maximum <- +15.0
    axis.MajorStep <- 5.0
    axis.MinorStep <- 1.0
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    axis.MinorGridlineColor <- OxyColors.LightGray
    axis.MinorGridlineStyle <- LineStyle.Dot
    axis.AxisTitleDistance <- 20.0
    model.Axes.Add(axis)

    let series = LineSeries()
    series.Title <- "True price"
    series.StrokeThickness <- 1.0
    data
    |> Array.mapi (fun i (p, _, _, _, _, _) -> DataPoint(float i, float p))
    |> Array.iter series.Points.Add
    model.Series.Add(series)

    let series = AreaSeries()
    series.Title <- "MM price"
    series.StrokeThickness <- 1.0
    data
    |> Array.mapi (fun i (_, p, _, _, _, _) -> DataPoint(float i, float p))
    |> Array.iter series.Points.Add
    data
    |> Array.mapi (fun i (_, _, p, _, _, _) -> DataPoint(float i, float p))
    |> Array.iter series.Points2.Add
    model.Series.Add(series)

    model |> exportToPng path 700 400

let renderEndOfEpisodeProfitSum path data expected =

    let data = data |> Array.map float

    let configureAxisY (axis : LinearAxis) =
        axis.Title <- "Profit"
        axis.Minimum <- -2500.0
        axis.Maximum <- +500.0
        axis.MajorStep <- 500.0
        axis.MinorStep <- 100.0
        axis.AxisTitleDistance <- 6.0

    renderEpisode path data expected configureAxisY

let renderEndOfEpisodeInventory path data expected =

    let data = data |> Array.map float

    let configureAxisY (axis : LinearAxis) =
        axis.Title <- "Inventory"
        axis.Minimum <- -150.0
        axis.Maximum <- +150.0
        axis.MajorStep <- 50.0
        axis.MinorStep <- 10.0
        axis.AxisTitleDistance <- 13.0

    renderEpisode path data expected configureAxisY

let renderAveragePriceDeviation path data expected =

    let configureAxisY (axis : LinearAxis) =
        axis.Title <- "Price Deviation"
        axis.Minimum <- 0.0
        axis.Maximum <- 30.0
        axis.MajorStep <- 5.0
        axis.MinorStep <- 1.0
        axis.AxisTitleDistance <- 25.0

    renderEpisode path data expected configureAxisY

let renderAverageEpisodicSpread path data expected =

    let configureAxisY (axis : LinearAxis) =
        axis.Title <- "Spread"
        axis.Minimum <- 1.0
        axis.Maximum <- 4.0
        axis.MajorStep <- 1.0
        axis.MinorStep <- 0.1
        axis.AxisTitleDistance <- 32.0

    renderEpisode path data expected configureAxisY
