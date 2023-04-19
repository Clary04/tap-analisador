using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

class Program
{
    static string FilterWord(string word)
    {
        word = word.ToLower();
        if (!Regex.IsMatch(word, "^[a-z']+$"))
        {
            word = "";
        }
        return word;
    }

    static Dictionary<string, int> CountWords(string[] words, Dictionary<string, int> wordCounts)
    {
        return words.Aggregate(wordCounts, (result, word) =>
        {
            word = FilterWord(word);
            if (!string.IsNullOrEmpty(word) && !int.TryParse(word, out _))
            {
                result[word] = result.GetValueOrDefault(word) + 1;
            }
            return result;
        });
    }

    static Dictionary<string, int> CreateFile(string filename, Dictionary<string, int> wordCounts)
    {
        string contents = File.ReadAllText(filename);
        string[] words = Regex.Split(contents, @"\s+");
        return CountWords(words, wordCounts);
    }

    static Dictionary<string, int> GetWordCounts(string directory)
    {
        string[] files = Directory.GetFiles(directory, "*.srt");
        return files.Aggregate(new Dictionary<string, int>(), (result, filename) =>
        {
            return CreateFile(filename, result);
        });
    }

    static List<object> FormatWordCounts(Dictionary<string, int> wordCounts)
    {
        return wordCounts.Select(pair => new
        {
            word = pair.Key,
            frequency = pair.Value
        }).Cast<object>().ToList();
    }

    static void WriteWordCountsToFile(List<object> wordCounts, string filename)
    {
        string json = JsonSerializer.Serialize(wordCounts, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        File.WriteAllText(filename, json);
    }

    static void CountWordsInDirectory(string directory)
    {
        Dictionary<string, int> wordCounts = GetWordCounts(directory);
        wordCounts = wordCounts.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        List<object> formattedWordCounts = FormatWordCounts(wordCounts);
        WriteWordCountsToFile(formattedWordCounts, "resultado/resultado.json");
        Console.WriteLine("Arquivo gerado com sucesso!");
    }

    static void Main()
    {
        //using (var dialog = new FolderBrowserDialog())
        //{
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        string selectedPath = dialog.SelectedPath;
        //        Console.WriteLine($"O diretório selecionado foi: {selectedPath}");
        //    }
        //}
        string directory = "";
        CountWordsInDirectory(directory);
    }
}
