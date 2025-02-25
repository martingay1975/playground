using CsvHelper;
using CsvHelper.Configuration;
using Highlight.Sample.Anomaly.ChangePoint;
using Highlight.Sample.Anomaly.Spike;
using Microsoft.ML;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Highlight.Sample.Anomaly.App
{
    internal class Program
    {
        private const string dataPath = @"C:\git\playground\AI\Highlight.Sample.Anomaly.App\Data\";
        private const string modelPath = @"C:\git\playground\AI\Highlight.Sample.Anomaly.App\Models\";

        // this is a subset of data where I have gone through the data and put in where I think it should detect spikes. I have given a confidence rating between 0 and 1. If I entered '1' then it should deffo detect it as a spike.
        // If I entered '0.6' for example, I think it should detect as a spike but maybe can be sympathetic that it has not.
        private const string janDataFile = "samples-dedx-jan2024.csv";

        // full set of 
        private const string allDataFile = "samples-lh.csv";
        //private const string dataFile = janDataFile;
        private static Stream GetDataFileStream(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(name => name.EndsWith(filename, StringComparison.OrdinalIgnoreCase));
            return assembly.GetManifestResourceStream(resourceName);
        }

        static void Main(string[] args)
        {
            //TransformCsv();

            // Spike
            RunBySpikeIId();
            //RunBySpikeSsa();
            //RunBySpikeSrCnn();

            //Change Point
            //RunByChangePointIId();
            //RunByChangePointSsa();
        }

        private static MLContext mlContext = new MLContext(seed: 0);

        private static void RunByChangePointSsa()
        {
            var changepointBySsa = new ChangePointBySsa(mlContext);
            var loadInData = LoadData(allDataFile);
            var stopWatch = new Stopwatch();

            stopWatch.Restart();
            var outputDto = changepointBySsa.Detect(loadInData);
            stopWatch.Stop();
            Console.WriteLine($"Detection took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            Print(outputDto, GetSpikesByAlert(outputDto));
            OutputToFile(outputDto, "changepoint-ssa");
        }

        private static void RunByChangePointIId()
        {

            var changepointByIid = new ChangePointByIId(mlContext);

            var loadInData = LoadData(allDataFile);
            var stopWatch = new Stopwatch();

            stopWatch.Restart();
            var outputDto = changepointByIid.Detect(loadInData);
            stopWatch.Stop();
            Console.WriteLine($"Detection took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            Print(outputDto, GetSpikesByAlert(outputDto));
            OutputToFile(outputDto, "changepoint-iid");
        }

        //private static void TransformCsv()
        //{
        //    var dataFilePath = Path.Combine(dataPath, dataFile);
        //    var loadInData = LoadData(dataFilePath);

        //    foreach (var data in loadInData)
        //    {
        //        data.Time += ":00";
        //    }

        //    using var streamOutput = new StreamWriter(Path.Combine(dataPath, $"loadin.csv"));
        //    using var outputCsv = new CsvWriter(streamOutput, CultureInfo.InvariantCulture);
        //    outputCsv.WriteRecords(loadInData);
        //}

        private static IEnumerable<OutputDto> GetSpikesByAlert(IEnumerable<OutputDto> outputList) => outputList.Where(outputData => outputData.Alert && outputData.DataIn > 0);
        private static IEnumerable<OutputDto> GetSpikesByBelow0_02(IEnumerable<OutputDto> outputList) => outputList.Where(outputData => outputData.PValue < 0.01);

        private static void RunBySpikeIId()
        {
            var spikeIid = new SpikeByIId(mlContext);

            var loadInData = LoadData(janDataFile);
            var stopWatch = new Stopwatch();

            stopWatch.Restart();
            var outputDto = spikeIid.Detect(loadInData);
            stopWatch.Stop();
            Console.WriteLine($"Detection took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            Print(outputDto, GetSpikesByAlert(outputDto));
            Print(outputDto, GetSpikesByBelow0_02(outputDto));
            OutputToFile(outputDto, "spike-iid");
        }

        private static void RunBySpikeSsa()
        {
            var modelFilePath = Path.Combine(modelPath, "ssa-spike.zip");
            var spikeBySsa = new SpikeBySsa(mlContext, modelFilePath);

            var loadInData = LoadData(allDataFile);
            var stopWatch = new Stopwatch();

            stopWatch.Restart();
            spikeBySsa.SpikesTrain(loadInData, 180);
            stopWatch.Stop();
            Console.WriteLine($"Training data took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            stopWatch.Restart();
            var outputDto = spikeBySsa.SpikeDetection(loadInData);
            stopWatch.Stop();
            Console.WriteLine($"Spike detection took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            Print(outputDto, GetSpikesByAlert(outputDto));
            OutputToFile(outputDto, "spike-ssa");
        }

        private static void RunBySpikeSrCnn()
        {
            var spikeBySrCnn = new SpikeBySrCnn(mlContext);

            var loadInData = LoadData(allDataFile);
            var stopWatch = new Stopwatch();

            stopWatch.Restart();
            var outputDto = spikeBySrCnn.Detect(loadInData);
            stopWatch.Stop();
            Console.WriteLine($"Detection took {stopWatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"");

            Print(outputDto, GetSpikesByAlert(outputDto));
            Print(outputDto, GetSpikesByBelow0_02(outputDto));
            OutputToFile(outputDto, "spike-srcnn");
        }

        private static LoadInDto[] LoadData(string filePath)
        {
            Console.WriteLine($"Loading data: {filePath}");
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, MissingFieldFound = null };
            using var stream = GetDataFileStream(filePath);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<LoadInDto>();
            return records.Where(rec => rec.In > 0).ToArray();
        }

        public static void OutputToFile(IEnumerable<OutputDto> outputList, string postfixFilename)
        {
            var path = Path.Combine(dataPath, $"output-{postfixFilename}.csv");
            using var streamOutput = new StreamWriter(path);
            using var outputCsv = new CsvWriter(streamOutput, CultureInfo.InvariantCulture);
            outputCsv.WriteRecords(outputList);
            Console.WriteLine($"Written to {path}");
        }

        private static void Print(IEnumerable<OutputDto> outputList, IEnumerable<OutputDto> spikes)
        {
            Console.WriteLine("");
            Console.WriteLine("SPIKE DETECTION - results");
            var listCount = outputList.Count();
            if (!spikes.Any())
            {
                Console.WriteLine($"There are no anomalies");
                return;
            }

            var spikeCount = spikes.Count();
            var spikesOrderedByDataIn = spikes.OrderBy(sp => sp.DataIn);
            var spikeHighest = spikesOrderedByDataIn.First();
            var spikeLowest = spikesOrderedByDataIn.Last();
            var notSpikesButHigherThanLowestSpike = outputList.Where(outputData => !outputData.Alert && outputData.DataIn >= spikeLowest.DataIn);

            Console.WriteLine($"Processed {listCount} samples with {spikeCount} spikes. {((double)spikeCount / (double)listCount) * 100d:0.00}% of data points were spikes");
            Console.WriteLine($"Highest value of spike was: {spikeHighest}");
            Console.WriteLine($"Lowest value of spike was: {spikeLowest}");
            Console.WriteLine($"Records higher than lowest spike and not a spike: {notSpikesButHigherThanLowestSpike.Count()}");
        }
    }
}
