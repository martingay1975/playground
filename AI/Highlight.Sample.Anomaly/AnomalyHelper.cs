using Highlight.Sample.Anomaly.Spike.DataStructure;

namespace Highlight.Sample.Anomaly
{
    public static class AnomalyHelper
    {
        public static int SamplesPerDay => 480; // 24 * 20 = 20 samples an hour (3 minutes) * 24 hours.

        public static IEnumerable<OutputDto> CreateOutputList(LoadInDto[] realData, IEnumerable<ISpikePredictionBase> predictions)
        {
            int i = 0;
            var outputList = new List<OutputDto>();

            foreach (var p in predictions)
            {
                var spikeSample = new OutputDto() { Time = realData[i].Time, DataIn = realData[i].In, Index = i, ChangePointMG = realData[i].ChangePointMG, SpikeMG = realData[i].SpikeMG };
                spikeSample.SetPrediction(p.Prediction);
                outputList.Add(spikeSample);
                i++;
            }

            return outputList;
        }
    }
}
