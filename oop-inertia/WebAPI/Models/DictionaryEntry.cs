namespace WebAPI.Models;

public class DictionaryEntry
{
    public string Key { get; }   
    public int Value { get; }

    public DictionaryEntry(string key, int value)
    {
        Key = key;
        Value = value;
    }
}