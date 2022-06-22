using Newtonsoft.Json;

namespace Inertia.Storage;

public class BestScoresStorage
{
    private const int MaxEntries = 100;
    private const string FileName = "scores.json";

    private readonly Dictionary<string, int> _scores;

    public BestScoresStorage()
    {
        using var stream = new FileStream(FileName, FileMode.OpenOrCreate);
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        _scores = JsonConvert.DeserializeObject<Dictionary<string, int>>(json) 
                  ?? new Dictionary<string, int>();
    }

    public void Add(string username, int score)
    {
        _scores[username] = score;

        if (_scores.Count > MaxEntries)
        {
            var last = _scores.OrderBy(x => x.Value).First().Key;
            _scores.Remove(last);
        }
        
        var json = JsonConvert.SerializeObject(_scores);

        File.WriteAllText(FileName, json);
    }

    public Dictionary<string, int> GetTopScores(int entriesCount)
    {
        var ordered = _scores.OrderByDescending(x => x.Value).ToList();

        var result = new Dictionary<string, int>();

        var resultLength = Math.Min(entriesCount, ordered.Count);

        for (var i = 0; i < resultLength; i++)
        {
            var (key, value) = ordered[i];

            result[key] = value;
        }

        return result;
    }
}