using System;
using System.Collections.Generic;
using System.Linq;

namespace Thesaurus
{
    public class Thesaurus : IThesaurus
    {
        private readonly Elastic _elastic;

        public Thesaurus(Elastic elastic)
        {
            _elastic = elastic;
        }

        public void AddSynonyms(IEnumerable<string> synonyms)
        {
            // ReSharper disable PossibleMultipleEnumeration
            var wordWithSynonymses = synonyms.Select(s =>
            {
                var syns = new List<string>(synonyms); // copy
                syns.Remove(s); // we son't want to add the word itself

                return new WordWithSynonyms {Word = s, Sysnonyms = syns};
            }).ToList();

            _elastic.AddOrUpdate(wordWithSynonymses);
            // ReSharper restore PossibleMultipleEnumeration
        }

        public IEnumerable<string> GetSynonyms(string word)
        {
            return _elastic.GetSynonyms(word);
        }

        public IEnumerable<string> GetWords()
        {
            return _elastic.GetWords();
        }

        public void Init()
        {
            _elastic.CreateIndex();
        }

        public void TearDown()
        {
            _elastic.TearDown();
        }
    }
}