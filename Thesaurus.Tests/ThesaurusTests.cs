using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Thesaurus.Tests
{
    [TestFixture]
    public class ThesaurusTests
    {
        private Thesaurus _thesaurus;
        private ElasticForTests _elasticForTests;

        [SetUp]
        public void OnceBeforeEachTest()
        {
            _elasticForTests = new ElasticForTests();
            _thesaurus = new Thesaurus(_elasticForTests);
            _thesaurus.Init();
        }

        [TearDown]
        public void OnceAfterEachTest()
        {
            _thesaurus.TearDown();
        }

        [Test]
        public void Should_add_synonyms()
        {
            // given
            var synonyms = new[] {"hubba", "bubba"};
            var added = false;
            _elasticForTests.OnAddOrUpdate = _ => added = true;

            // when
            _thesaurus.AddSynonyms(synonyms);

            // then
            Assert.That(added);
        }

        [Test]
        public void Add_Should_convert_to_list_of_WordWithSynonym()
        {
            // given
            var synonyms = new[] {"hubba", "bubba"};
            var isWordWithSysnonym = false;

            _elasticForTests.OnAddOrUpdate = list =>  isWordWithSysnonym = list.First() != null;

            // when
            _thesaurus.AddSynonyms(synonyms);

            // then
            Assert.That(isWordWithSysnonym);
        }

        [Test]
        public void Word_Should_have_sysnonyms()
        {
            // when
            var synonyms = new[] { "hubba", "bubba" };
            var wordsWithsynonyms = new List<WordWithSynonyms>();

            _elasticForTests.OnAddOrUpdate = list => wordsWithsynonyms = list.ToList();

            // when
            _thesaurus.AddSynonyms(synonyms);

            // then
            Assert.That(wordsWithsynonyms.First(w => w.Word == "hubba").Sysnonyms, Does.Not.Contain("hubba"));
        }

        [Test]
        public void Word_Should_not_be_synonym_to_itself()
        {
            // when
            var synonyms = new[] {"hubba", "bubba"};
            var wordsWithsynonyms = new List<WordWithSynonyms>();

            _elasticForTests.OnAddOrUpdate = list => wordsWithsynonyms = list.ToList();

            // when
            _thesaurus.AddSynonyms(synonyms);

            // then
            Assert.That(wordsWithsynonyms.First(w => w.Word == "hubba").Sysnonyms, Does.Contain("bubba"));
        }
    }
}
