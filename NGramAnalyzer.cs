using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NgramUI
{
    public class NGramAnalyzer
    {
        public string AnalyzeFolder(string folderPath)
        {
            StringBuilder output = new StringBuilder();
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            FileInfo[] files = folder.GetFiles("*.txt");

            if (files == null || files.Length == 0)
            {
                return "⚠️ No .txt files found in the selected folder.";
            }

            output.AppendLine("═══════════════════════════════════════════════════════════════════════");
            output.AppendLine($"📁 Folder: {folder.FullName}");
            output.AppendLine($"📅 Analysis started: {DateTime.Now}");
            output.AppendLine("═══════════════════════════════════════════════════════════════════════\n");

            foreach (FileInfo file in files)
            {
                output.AppendLine($"📘 FILE: {file.Name}");
                output.AppendLine("───────────────────────────────────────────────────────────────────────");
                output.AppendLine(ProcessFile(file));
                output.AppendLine();
            }

            output.AppendLine("═══════════════════════════════════════════════════════════════════════");
            output.AppendLine("✅ Analysis complete.");
            output.AppendLine($"🕒 Finished at: {DateTime.Now}");
            output.AppendLine("═══════════════════════════════════════════════════════════════════════");

            return output.ToString();
        }

        private string ProcessFile(FileInfo file)
        {
            StringBuilder result = new StringBuilder();
            string text = ReadFile(file.FullName);

            // Normalize text for Turkish and English
            text = Regex.Replace(text, "[^a-zA-ZğüşöçıİĞÜŞÖÇ\\s]", "")
                        .ToLower(new CultureInfo("tr-TR"));

            string[] words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            result.AppendLine(GenerateNGrams(words, 1));
            result.AppendLine(GenerateNGrams(words, 2));
            result.AppendLine(GenerateNGrams(words, 3));

            long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            result.AppendLine($"⏱️ Total running time: {endTime - startTime} ms");
            result.AppendLine("───────────────────────────────────────────────────────────────────────");

            return result.ToString();
        }

        private string ReadFile(string filename)
        {
            try
            {
                return File.ReadAllText(filename, Encoding.GetEncoding("windows-1254"));
            }
            catch (IOException e)
            {
                return $"❌ Error reading file: {e.Message}";
            }
        }

        private string GenerateNGrams(string[] words, int n)
        {
            if (words.Length < n)
                return $"Not enough words for {n}-gram analysis.\n";

            Dictionary<string, int> ngrams = new Dictionary<string, int>(words.Length);

            for (int i = 0; i <= words.Length - n; i++)
            {
                string gram = string.Join(" ", words.Skip(i).Take(n));
                if (ngrams.ContainsKey(gram))
                    ngrams[gram]++;
                else
                    ngrams[gram] = 1;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"📊 Top 10 {n}-grams:");

            var topN = ngrams
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToList();

            if (topN.Count == 0)
            {
                sb.AppendLine("   (No n-grams found)");
                return sb.ToString();
            }

            sb.AppendLine($"{"Rank",-6}{"N-Gram",-40}{"Count",6}");
            sb.AppendLine(new string('-', 60));

            int rank = 1;
            foreach (var entry in topN)
            {
                sb.AppendLine($"{rank,-6}{entry.Key,-40}{entry.Value,6}");
                rank++;
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
