using System.Collections.Generic;

namespace Thesaurus
{
    public class WordWithSynonyms
    {
        /// <summary>
        /// This is a special non analyzed field
        /// It shouldn't be needed really,
        /// insead you should do a multiField mapping
        /// but they've changed the way you do that...
        /// and I don't have time to lookup how.
        /// </summary>
        public string Keyword => Word;

        public string Word { get; set; }

        public IEnumerable<string> Sysnonyms { get; set; }
    }
}