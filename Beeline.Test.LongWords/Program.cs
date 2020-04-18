using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Beeline.Test.LongWords
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var path = args[0].Replace("--path=", "");
            Console.WriteLine($"Path set to {path}");

            var isLenSet = int.TryParse(args[1].Replace("--len=", ""), out var minWordLen);

            if (!isLenSet)
            {
                Console.WriteLine("Min word length is empty.");
                return;
            }

            Console.WriteLine($"Min word len set to {minWordLen}");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (o, eventArgs) => cts.Cancel();

            //в задании про thread's Task как раз с их помощью реализованы
            var tasks = Directory.EnumerateFiles(path)
                .Select(file =>
                    Task.Run(async () => await GetFileWordsWithCountsAsync(minWordLen, file), cts.Token))
                .ToArray();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                return;
            }


            var completeDictionary = new Dictionary<string, int>();
            foreach (var task in tasks)
            {
                var data = task.Result;
                foreach (var (word, counter) in data)
                    if (completeDictionary.ContainsKey(word))
                        completeDictionary[word] += counter;
                    else
                        completeDictionary.Add(word, counter);
            }

            var topLongestWords = completeDictionary
                .OrderByDescending(x => x.Value)
                .Take(10)
                .Select(x => x);


            Console.WriteLine("Result is:");
            foreach (var (word, counter) in topLongestWords) Console.WriteLine($"{word} ({counter})");
        }

        private static async Task<IDictionary<string, int>> GetFileWordsWithCountsAsync(int minWordLen, string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);
            var pattern = $@"\b\w{{{minWordLen},}}\b";

            var words = Regex.Matches(content, pattern)
                .Select(m => m.Value)
                .Distinct()
                .ToDictionary(x => x, y => y.Length);

            return words;
        }
    }
}