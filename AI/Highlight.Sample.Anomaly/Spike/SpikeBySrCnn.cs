using Highlight.Sample.Anomaly.Spike.DataStructure;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;

namespace Highlight.Sample.Anomaly.Spike
{
    public class SpikeBySrCnn
    {
        private readonly MLContext mlContext;

        public SpikeBySrCnn(MLContext mlContext)
        {
            this.mlContext = mlContext;
        }

        public IEnumerable<OutputDto> Detect(IEnumerable<LoadInDto> data)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // Get data into the correct format/types - use a transformation.
            var pipeline = mlContext.Transforms.Conversion.ConvertType(nameof(LoadInDto.In), outputKind: DataKind.Double);
            var transformDataView = pipeline.Fit(dataView).Transform(dataView);

            int period = mlContext.AnomalyDetection.DetectSeasonality(transformDataView, nameof(LoadInDto.In));
            Console.WriteLine("Period of the series is: {0}.", period);

            //STEP 3: Setup the parameters
            var options = new SrCnnEntireAnomalyDetectorOptions()
            {
                Threshold = 0.3,
                Sensitivity = 64.0,
                DetectMode = SrCnnDetectMode.AnomalyOnly,
                Period = period,
            };

            //STEP 4: Invoke SrCnn algorithm to detect anomaly on the entire series.
            var outputDataView = mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(transformDataView, nameof(SpikePredictionSrCnn.Prediction), nameof(LoadInDto.In), options);

            //STEP 5: Get the detection results as an IEnumerable
            var predictions = mlContext.Data.CreateEnumerable<SpikePredictionSrCnn>(outputDataView, reuseRowObject: false);

            return AnomalyHelper.CreateOutputList(data.ToArray(), predictions);
        }
    }
}
