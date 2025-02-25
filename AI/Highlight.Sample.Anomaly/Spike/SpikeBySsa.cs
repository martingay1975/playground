using Highlight.Sample.Anomaly.Spike.DataStructure;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Highlight.Sample.Anomaly.Spike
{
    public class SpikeBySsa
    {
        private string modelFilePath;
        private readonly MLContext mlContext;

        public SpikeBySsa(MLContext mlContext, string modelFilePath)
        {
            this.mlContext = mlContext;
            this.modelFilePath = modelFilePath;
        }

        public void SpikesTrain(IEnumerable<LoadInDto> trainingData, int watchIntervalinSecs)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(trainingData);
            var oneDay = TimeSpan.FromDays(1);
            var samplesInADay = (int)oneDay.TotalSeconds / watchIntervalinSecs;

            // Configure the Estimator
            var pValueSize = samplesInADay * 7; // look back 1 week
            var seasonalitySize = samplesInADay;    // be sympathetic to spikes at special times of the day
            var trainingSize = trainingData.Count();    // how much of the data should we use whilst training?
            const double ConfidenceInterval = 99;   // what's the threshold in confidence for it to say we are sure it is a spike?

            string outputColumnName = nameof(SpikePredictionPValue.Prediction);
            string inputColumnName = nameof(LoadInDto.In);

            var convertTypePipeline = mlContext.Transforms.Conversion.ConvertType(nameof(LoadInDto.In), outputKind: DataKind.Single);
            var trainigPipeLine = convertTypePipeline.Append(mlContext.Transforms.DetectSpikeBySsa(
                outputColumnName,
                inputColumnName,
                confidence: ConfidenceInterval,
                pvalueHistoryLength: pValueSize,
                trainingWindowSize: trainingSize,
                seasonalityWindowSize: seasonalitySize));

            ITransformer trainedModel = trainigPipeLine.Fit(dataView);

            // STEP 6: Save/persist the trained model to a .ZIP file
            mlContext.Model.Save(trainedModel, dataView.Schema, modelFilePath);
            Console.WriteLine("The model is saved to {0}", modelFilePath);
        }

        public IEnumerable<OutputDto> SpikeDetection(LoadInDto[] realData)
        {
            ITransformer trainedModel = mlContext.Model.Load(modelFilePath, out var modelInputSchema);
            var dataView = mlContext.Data.LoadFromEnumerable(realData);
            var transformedData = trainedModel.Transform(dataView);

            // Getting the data of the newly created column as an IEnumerable
            IEnumerable<SpikePredictionPValue> predictions =
                mlContext.Data.CreateEnumerable<SpikePredictionPValue>(transformedData, false);

            return AnomalyHelper.CreateOutputList(realData, predictions);
        }
    }
}
