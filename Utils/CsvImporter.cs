using System.Text;
namespace Avaratra.BackOffice.Utils{

    public class CsvImporter<T>
    {
        private readonly Func<string[], int, (T? entity, string? error)> _mapLine;

        public CsvImporter(Func<string[], int, (T? entity, string? error)> mapLine)
        {
            _mapLine = mapLine;
        }

        public async Task<(List<T> entities, List<string> errors)> ImportAsync(Stream fileStream)
        {
            var entities = new List<T>();
            var errors = new List<string>();

            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            var header = await reader.ReadLineAsync(); // ignore l’en-tête
            int lineNumber = 1;

            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                var (entity, error) = _mapLine(values, lineNumber);

                if (error != null)
                    errors.Add(error);
                else if (entity != null)
                    entities.Add(entity);
            }

            return (entities, errors);
        }
    }
}
