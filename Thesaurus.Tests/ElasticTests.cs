using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Thesaurus.Tests
{
    [TestFixture]
    public class ElasticTests
    {
        private ElasticForTests _elastic;

        [SetUp]
        public void OnceBeforeEachTest()
        {
            _elastic = new ElasticForTests();
            _elastic.CreateIndex();
        }

        [TearDown]
        public void OnceAfterEachTest()
        {
            _elastic.TearDown();
        }

        [Test]
        public void can_init_and_teardown()
        {
            // given
            var elastic = new ElasticForTests(5);

            // when, then
            Assert.DoesNotThrow(elastic.CreateIndex);
            Assert.DoesNotThrow(elastic.TearDown);
        }

        [Test]
        public void can_add()
        {
            // given
            var words = new List<WordWithSynonyms>
            {
                new WordWithSynonyms {Word = "hubba", Sysnonyms = new[] {"bubba"}},
            };

            // when
            _elastic.AddOrUpdate(words);

            var synonyms = _elastic.GetSynonyms("hubba");

            // then
            Assert.That(synonyms.First(), Is.EqualTo("bubba"));
        }

        [Test]
        public void Should_match_on_partial_word()
        {
            // given
            var words = new List<WordWithSynonyms>
            {
                new WordWithSynonyms {Word = "hubba", Sysnonyms = new[] {"bubba"}},
            };

            // when
            _elastic.AddOrUpdate(words);

            var synonyms = _elastic.GetSynonyms("hub");

            // then
            Assert.That(synonyms.First(), Is.EqualTo("bubba"));
        }

        [Test]
        public void should_sort_by_score()
        {
            // given
            var words = new List<WordWithSynonyms>
            {
                new WordWithSynonyms {Word = "hub", Sysnonyms = new []{"pivot"}},
                new WordWithSynonyms {Word = "hubba", Sysnonyms = new[] {"bubba"}},
            };

            // when
            _elastic.AddOrUpdate(words);

            var synonyms = _elastic.GetSynonyms("hub");

            // then
            Assert.That(synonyms.First(), Is.EqualTo("pivot"));
        }

        [Test]
        public void Should_be_able_to_get_all_words()
        {
            // given
            var toIndex = new List<WordWithSynonyms>
            {
                new WordWithSynonyms {Word = "hub", Sysnonyms = new []{"pivot"}},
                new WordWithSynonyms {Word = "hubba", Sysnonyms = new[] {"bubba"}},
            };

            // when
            _elastic.AddOrUpdate(toIndex);

            var words = _elastic.GetWords().ToList();

            // then
            Assert.That(words.Count, Is.EqualTo(2));
            Assert.That(words.Contains("hub"));
            Assert.That(words.Contains("hubba"));
        }
    }
}