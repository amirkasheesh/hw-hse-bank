using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application
{
    public class JsonDataExporter : IDataExporter
    {
        public void Export(BankDataSnapshot snapshot, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(snapshot, options);
            File.WriteAllText(filePath, json);
        }
    }
}