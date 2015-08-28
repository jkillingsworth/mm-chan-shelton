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
    axis.Maximum <- 1000.0
    axis.MajorStep <- 100.0
    axis.MinorStep <- 20.0
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
    |> Array.mapi (fun i (p, _, _, _, _) -> DataPoint(float i, float p))
    |> Array.iter series.Points.Add
    model.Series.Add(series)

    let series = LineSeries()
    series.Title <- "MM price"
    series.StrokeThickness <- 1.0
    data
    |> Array.mapi (fun i (_, p, _, _, _) -> DataPoint(float i, float p))
    |> Array.iter series.Points.Add
    model.Series.Add(series)

    model |> exportToPng path 700 400

let renderResult path data =

    let model = PlotModel()

    model.DefaultColors <- defaultColorsToUseForPlots
    model.LegendBackground <- OxyColors.White
    model.LegendBorder <- OxyColors.Gray
    model.LegendBorderThickness <- 1.0
    model.LegendPlacement <- LegendPlacement.Inside
    model.LegendPosition <- LegendPosition.RightBottom
    model.PlotMargins <- OxyThickness(nan, nan, 10.0, nan)

    let axis = LinearAxis()
    axis.Title <- "Noise"
    axis.Position <- AxisPosition.Bottom
    axis.Minimum <- 0.0
    axis.Maximum <- 2.0
    axis.MajorStep <- 0.2
    axis.MinorStep <- 0.05
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    model.Axes.Add(axis)

    let axis = LinearAxis()
    axis.Title <- "Expected Profit"
    axis.Position <- AxisPosition.Left
    axis.Minimum <- -0.65
    axis.Maximum <- -0.20
    axis.MajorStep <- 0.05
    axis.MinorStep <- 0.05
    axis.MajorGridlineColor <- OxyColors.LightGray
    axis.MajorGridlineStyle <- LineStyle.Dot
    axis.MinorGridlineColor <- OxyColors.LightGray
    axis.MinorGridlineStyle <- LineStyle.Dot
    axis.AxisTitleDistance <- 10.0
    axis.StringFormat <- "F2"
    model.Axes.Add(axis)

    for (imbThreshold, points) in data do
        let series = LineSeries()
        series.Title <- sprintf "Imbalance = %i" imbThreshold
        series.StrokeThickness <- 1.0
        points
        |> List.mapi (fun i (x, y) -> DataPoint(x, y))
        |> List.iter series.Points.Add
        model.Series.Add(series)

    model |> exportToPng path 700 400
