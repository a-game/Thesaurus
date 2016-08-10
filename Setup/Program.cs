
using Thesaurus;

namespace Setup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var thesaurus = new Thesaurus.Thesaurus(new Elastic("http://goelastic:9200", "thesaurus"));
            thesaurus.Init();
            thesaurus.AddSynonyms(new []{"hubba", "bubba"});
            thesaurus.AddSynonyms(new []{"hullo", "hi", "howdy", "how-do-you-do", "hello"});
            thesaurus.AddSynonyms(new [] { "computing machine","computing device","data processor", "electronic computer","information processing system","computer"});
        }
    }
}
