using Highlight.Sample.Anomaly.Spike.DataStructure;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Highlight.Sample.Anomaly.ChangePoint
{
    public class ChangePointBySsa
    {
        private readonly MLContext mlContext;

        public ChangePointBySsa(MLContext mlContext)
        {
            this.mlContext = mlContext;
        }

        public IEnumerable<OutputDto> Detect(IEnumerable<LoadInDto> data)
        {
            var dataSize = data.Count();
            var dataView = mlContext.Data.LoadFromEnumerable(data);
            var outputColumnName = nameof(SpikePredictionPValue.Prediction);
            var inputColumnName = nameof(LoadInDto.In);


            var convertTypePipeline = mlContext.Transforms.Conversion.ConvertType(nameof(LoadInDto.In), outputKind: DataKind.Single);

            var ssaPipline = mlContext.Transforms.DetectChangePointBySsa(
                    outputColumnName, inputColumnName,
                    confidence: 90.0d,
                    changeHistoryLength: 100,
                    trainingWindowSize: 1440,
                    martingale: Microsoft.ML.Transforms.TimeSeries.MartingaleType.Mixture,
                    seasonalityWindowSize: 480);

            var pipeline = convertTypePipeline.Append(ssaPipline);

            var transformedData = pipeline
                .Fit(dataView)
                .Transform(dataView);

            var predictions = mlContext.Data.CreateEnumerable<SpikePredictionPValue>(transformedData, reuseRowObject: false);

            return AnomalyHelper.CreateOutputList(data.ToArray(), predictions);
        }
    }
}