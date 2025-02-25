namespace Highlight.Sample.Anomaly.Spike.DataStructure
{
    public interface ISpikePredictionBase
    {
        double[] Prediction { get; }

        bool Alert { get; }

        double Score { get; }

        double PValue { get; }
    }
}
