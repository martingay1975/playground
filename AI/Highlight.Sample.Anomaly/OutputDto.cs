namespace Highlight.Sample.Anomaly
{
    public class OutputDto
    {
        public void SetPrediction(double[] prediction)
        {
            Alert = prediction[0] == 1;
            Score = prediction[1];
            PValue = prediction[2];
        }

        public int Index { get; set; }
        public DateTime Time { get; set; }
        public float DataIn { get; set; }
        public bool Alert { get; set; }
        public double Score { get; set; }
        public double PValue { get; set; }
        public double SpikeMG { get; set; }
        public double ChangePointMG { get; set; }


        public override string ToString() => $"{Index} {Time} {DataIn} sc={Score} pv={PValue} spike={Alert}";

    }
}
