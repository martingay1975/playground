using Highlight.Sample.Anomaly.Spike.DataStructure;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace Highlight.Sample.Anomaly.ChangePoint
{
    public class ChangePointByIId
    {
        private readonly MLContext mlContext;

        public ChangePointByIId(MLContext mlContext)
        {
            this.mlContext = mlContext;
        }

        public IEnumerable<OutputDto> Detect(IEnumerable<LoadInDto> data)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // STEP 1: Create Estimator.
            var estimator = mlContext.Transforms.DetectIidChangePoint(
                outputColumnName: nameof(SpikePredictionPValue.Prediction),
                inputColumnName: nameof(LoadInDto.In),
                confidence: 90d,
                changeHistoryLength: 100,
                martingale: MartingaleType.Mixture,
                eps: 0.2);

            // STEP 2:The Transformed Model.
            // In IID Spike detection, we don't need to do training, we just need to do transformation. 
            // As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            // So create empty data view and pass to Fit() method. 
            var convertTypePipeline = mlContext.Transforms.Conversion.ConvertType(nameof(LoadInDto.In), outputKind: DataKind.Single);
            var pipeline = convertTypePipeline.Append(estimator);
            ITransformer tansformedModel = pipeline.Fit(CreateEmptyDataView());

            // STEP 3: Use/test model.
            // Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<SpikePredictionPValue>(transformedData, reuseRowObject: false);

            return AnomalyHelper.CreateOutputList(data.ToArray(), predictions);
        }

        private IDataView CreateEmptyDataView()
        {
            //Create empty DataView. We just need the schema to call fit()
            IEnumerable<LoadInDto> enumerableData = new List<LoadInDto>();
            var dv = mlContext.Data.LoadFromEnumerable(enumerableData);
            return dv;
        }
    }
}
