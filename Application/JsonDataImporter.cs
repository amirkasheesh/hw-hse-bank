using System.Text.Json;
using System.Text.Encodings.Web;

namespace Application
{
    public class JsonDataImporter : IDataImporter
    {
        public BankDataSnapshot Import(string path)
        {
            var json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var snapshot = JsonSerializer.Deserialize<BankDataSnapshot>(json, options);

            if (snapshot == null)
            {
                throw new InvalidOperationException("Не удалось прочитать данные из файла!");
            }

            return snapshot;
        }
    }

}