using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MadelineMerced
{
    public static class Program
    {

        /// <summary>
        ///  public static void Main: Ringba Test Answers
        ///  Returns formatted data for each test request: 
        ///     1. Downloads file from AWS
        ///     2. How many of each letter are in the file
        ///     3. How many letters are capitalized in the file
        ///     4. The most common word and the number of times it has been seen.
        ///     5. The most common 2 character prefix and the number of occurrences in the text file.
        /// </summary>  
        public static void Main(string[] args)
        {
            //Downloads file from AWS to local file
            var webClient = new System.Net.WebClient();
            string filePath = @"c:\text\myfile.txt";
            webClient.DownloadFile("https://ringba-test-html.s3-us-west-1.amazonaws.com/TestQuestions/output.txt", filePath);

            //Counts letter frequency
            string withSpace = InsertSpaceString(File.ReadAllText(filePath));
            string lower = withSpace.ToLower();

            //Stores file data into a list for capitalized letter counting
            List<char> lines = File.ReadAllText(filePath).ToList();
            int caps = lines.Count(char.IsUpper);

            //Rewrites file with space between words
            File.WriteAllText(filePath, withSpace);

            //Reads new file contents into a text variable
            var text = File.ReadAllText(filePath);

            //Uses Regex to match and cound word frequency
            var matchWord = Regex.Match(text, "\\w+");
            Dictionary<string, int> frequency = new Dictionary<string, int>();
            while (matchWord.Success)
            {
                string word = matchWord.Value;
                if (frequency.ContainsKey(word))
                {
                    frequency[word]++;
                }
                else
                {
                    frequency.Add(word, 1);
                }

                matchWord = matchWord.NextMatch();
            }

            //Displays word with highest frequency 
            foreach (var wholeWord in frequency.OrderByDescending(a => a.Value).Take(1))
            {
                Console.WriteLine("The most common word is " + wholeWord.Key + " and it is seen " + wholeWord.Value + " times in the file.\n");
            }

            //Uses Regex to match and count prefix frequency
            var matchPrefix = Regex.Match(text, @"[A-Z]\w");
            Dictionary<string, int> prefixFreq = new Dictionary<string, int>();
            while (matchPrefix.Success)
            {
                string word = matchPrefix.Value;
                if (prefixFreq.ContainsKey(word))
                {
                    prefixFreq[word]++;
                }
                else
                {
                    prefixFreq.Add(word, 1);
                }

                matchPrefix = matchPrefix.NextMatch();
            }

            //Displays prefix with highest frequency
            foreach (var prefix in prefixFreq.OrderByDescending(a => a.Value).Take(1))
            {
                Console.WriteLine("The most common prefix is " + prefix.Key + " and it is seen " + prefix.Value + " times in the file.\n");
            }


            //Displays capitalized letter total to the console. 
            Console.WriteLine("There are " + caps + " capitalized letters in the file.\n");

            //Finds and displays frequency of each letter in the file
            int[] c = new int[(int)char.MaxValue];
            foreach (char t in lower)
            {
                // Increments table
                c[(int)t]++;
            }

            for (int i = 0; i < (int)char.MaxValue; i++)
            {
                if (c[i] > 0 &&
                    char.IsLetterOrDigit((char)i))
                {
                    Console.WriteLine("The letter {0} is in the file {1} times.",
                        (char)i,
                        c[i]);
                }
            }


        }

        /// <summary>
        ///  public static string InsertSpaceString: Returns string with spaces between words for processing 
        /// </summary>     
        public static string InsertSpaceString(this string original)
        {
            var updatedString = new StringBuilder();

            char preCharacter = char.MinValue;

            foreach (char character in original)
            {
                if (char.IsUpper(character))
                {
                    if (updatedString.Length != 0 && preCharacter != ' ')
                    {
                        updatedString.Append(' ');
                    }
                }

                updatedString.Append(character);

                preCharacter = character;
            }

            return updatedString.ToString();
        }
    }
}