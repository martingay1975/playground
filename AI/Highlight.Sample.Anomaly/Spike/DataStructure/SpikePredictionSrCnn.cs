using Microsoft.ML.Data;

namespace Highlight.Sample.Anomaly.Spike.DataStructure
{
    public class SpikePredictionSrCnn : ISpikePredictionBase
    {
        //vector to hold anomaly detection results. Including isAnomaly, anomalyScore, magnitude, expectedValue, boundaryUnits, upperBoundary and lowerBoundary.
        [VectorType(7)]
        public double[] Prediction { get; set; }

        public bool Alert => Prediction[0] == 1;

        public double Score => Prediction[1];

        public double PValue => Prediction[2];
    }
}
