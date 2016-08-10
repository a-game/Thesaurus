using System;
using System.Linq;
using Thesaurus;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var start = true;
            Console.WriteLine("Hey! Here is what you can do:");

            while (start || AnythingElse())
            {
                start = false;
                PresentOptions();
                var key = Console.ReadLine();
                var option = ValidateOption(key);
                ExecuteOption(option);
            }
        }

        private static bool AnythingElse()
        {
            Console.WriteLine("Can I help you with anything else? (y/n)");
            var key = Console.ReadLine();
            while (key == null || key.Length > 1 || !new[] {"y", "n"}.Contains(key))
            {
                Console.WriteLine("try again.");
                Console.WriteLine("Can I help you with anything else? (y/n)");
                key = Console.ReadLine();
            }

            if (key == "y")
            {
                Console.WriteLine("Cool!");
                return true;
            }
            Console.WriteLine("Ok! Goodbye.");
            Console.WriteLine("Press any key to quit");
            Console.Read();
            return false;
        }

        private static int ValidateOption(string key)
        {
            int option;
            while (!int.TryParse(key, out option) || option < 1 || option > 3)
            {
                Console.WriteLine("Invalid option, enter one of the following");
                PresentOptions();
                key = Console.ReadLine();
            }
            return option;
        }

        private static void PresentOptions()
        {
            Console.WriteLine("Add synonyms - 1");
            Console.WriteLine("Get synonym - 2");
            Console.WriteLine("Get all words - 3");
            Console.WriteLine("So what's it donna be?");
        }

        private static void ExecuteOption(int option)
        {
            var thesaurus = new Thesaurus.Thesaurus(new Elastic("http://goelastic:9200", "thesaurus"));

            switch (option)
            {
                case 1:
                {
                    Console.WriteLine("Enter a list of ';' separated synonyms");
                    var line = Console.ReadLine();
                    while (line == null || !line.Contains(';'))
                    {
                        Console.WriteLine("Try again");
                        Console.WriteLine("Enter a list of ';' separated synonyms");
                        line = Console.ReadLine();
                    }

                    var synonyms = line.Split(';');
                    thesaurus.AddSynonyms(synonyms);
                    break;
                }
                case 2:
                {
                    Console.WriteLine("Enter a word you want to know the synonyms for...");
                    var word = Console.ReadLine();
                    while (word == null)
                    {
                        Console.WriteLine("Try again");
                        Console.WriteLine("Enter a word you want to know the synonyms for...");
                        word = Console.ReadLine();
                    }

                    thesaurus.GetSynonyms(word).ToList().ForEach(Console.WriteLine);
                    break;
                }
                case 3:
                {
                    Console.WriteLine("Ok, This should really be paged.");
                    thesaurus.GetWords().ToList().ForEach(Console.WriteLine);
                    break;
                }
            }
        }
    }
}