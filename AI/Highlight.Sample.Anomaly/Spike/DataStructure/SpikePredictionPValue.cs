using Microsoft.ML.Data;

namespace Highlight.Sample.Anomaly.Spike.DataStructure
{
    public class SpikePredictionPValue : ISpikePredictionBase
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }

        public bool Alert => Prediction[0] == 1;
        public double Score => Prediction[1];
        public double PValue => Prediction[2];
    }
}
