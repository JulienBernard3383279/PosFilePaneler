// See https://aka.ms/new-console-template for more information

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections.Immutable;
using System.Globalization;

Console.WriteLine("Hello, World!");


string inputPath = "C:\\Users\\julbe\\OneDrive\\KicadProjects\\PicoAdapterPanel2023-07-08\\outputs-20230708\\PicoAdapter-bottom-pos.csv";
string outputPath = "C:\\Users\\julbe\\OneDrive\\KicadProjects\\PicoAdapterPanel2023-07-08\\outputs-20230708\\PicoAdapter-bottom-pos-processed.csv";
var records = new List<Pos>();

using (var reader = new StreamReader(inputPath))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Read();
    csv.ReadHeader();
    while (csv.Read())
    {
        var record = new Pos
        {
            Designator = csv.GetField("Ref")!,
            Val = csv.GetField("Val")!,
            Package = csv.GetField("Package")!,
            Mid_X = csv.GetField<double>("PosX"),
            Mid_Y = csv.GetField<double>("PosY"),
            Rotation = csv.GetField("Rot")!,
            Layer = csv.GetField("Side")!
        };

        records.Add(record);
    }
}

var recordsGroup = records.GroupBy(x => x.Designator)
    .Select(x => x.OrderBy(y => y.Mid_Y).ToList())
    .Select(x => assignHundreds(x))
    .ToImmutableList();

int a = 0;

using (var writer = new StreamWriter(outputPath))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(recordsGroup.SelectMany(x => x));
}

List<Pos> assignHundreds(List<Pos> records)
{
    List<Pos> result = new();
    int i = 0;
    foreach (Pos pos in records)
    {
        Pos newPos = pos;
        int indexNumber = newPos.Designator.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        newPos.Designator = newPos.Designator[..indexNumber] + (double.Parse(newPos.Designator[indexNumber..])+i*100).ToString();
        result.Add(newPos);
        i++;
    }
    return result;
}

class Pos
{
    [Name("Designator")]
    public string Designator { get; set; }
    [Name("Val")]
    public string Val { get; set; }
    [Name("Package")]
    public string Package { get; set; }
    [Name("Mid X")]
    public double Mid_X { get; set; }
    [Name("Mid Y")]
    public double Mid_Y { get; set; }
    [Name("Rotation")]
    public string Rotation { get; set; }
    [Name("Layer")]
    public string Layer { get; set; }
}