using System.Text.Json;
using System.Text.Encodings.Web;

namespace Application
{
    public class JsonDataImporter : BaseFileImporter
    {
        public override BankDataSnapshot Parse(string content)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var snapshot = JsonSerializer.Deserialize<BankDataSnapshot>(content, options);
            if (snapshot == null)
            {
                throw new InvalidOperationException("Не удалось распарсить JSON, так как получен null");
            }
            return snapshot;
        }
    }
}