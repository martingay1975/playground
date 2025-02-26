using CsvHelper.Configuration.Attributes;
using Microsoft.ML.Data;

namespace Highlight.Sample.Anomaly
{
    public class LoadInDto
    {
        [Index(0)]
        [LoadColumn(0)]
        [Format("dd/MM/yyyy HH:mm")]
        [ColumnName(@"Time")]
        public DateTime Time { get; set; }

        [Index(1)]
        [LoadColumn(1)]
        [ColumnName(@"In")]
        public int In { get; set; }

        [Index(2)]
        public string Out { get; set; }

        [Index(3)]
        public string UtilIn { get; set; }

        [Index(4)]
        public string UtilOut { get; set; }

        [Index(5)]
        public string Error { get; set; }

        [Index(6)]
        public string Discards { get; set; }

        [Index(7)]
        public double SpikeMG { get; set; }

        [Index(8)]
        public double ChangePointMG { get; set; }
    }
}
