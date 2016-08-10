using System;
using System.Collections.Generic;
using System.Linq;
using Nest;

namespace Thesaurus
{
    public class Elastic
    {
        private readonly string _index;
        private readonly ElasticClient _client;

        public Elastic(string connectionString, string index)
        {
            _index = index;
            _client = new ElasticClient(new Uri(connectionString));
        }

        // should be paged
        public IEnumerable<string> GetWords()
        {
            var response = _client.Search<WordWithSynonyms>(search => search
                .Query(queryContainer => queryContainer.MatchAll())
                .Index(_index)
                .Aggregations(agg => agg.Terms("words", termsAgg => termsAgg.Field(w => w.Keyword))));

            return response.Aggs.Terms("words").Buckets.Select(b => b.Key);
        }

        public virtual void AddOrUpdate(IList<WordWithSynonyms> documents)
        {
            if (!documents.Any())
            {
                return;
            }

            var response = _client.Bulk(
                bulkDescriptor =>
                    bulkDescriptor.IndexMany(documents, (d, p) => d.Index(_index).Id(p.Word).Document(p)).Refresh()
                );

            if (response.Errors)
            {
                if (response.ServerError != null)
                {
                    throw new Exception(response.ServerError.Error.ToString());
                }

                var err = response.ItemsWithErrors?.ToList();
                if (err?.Count > 0)
                {
                    throw new Exception("Items with errors");
                }
                throw new Exception("Unknown error");
            }
        }

        // should be paged
        public IEnumerable<string> GetSynonyms(string word)
        {
            var response = _client.Search<WordWithSynonyms>(search => search
                .Index(_index)
                .Query(queryContainer => queryContainer
                    .Match(matchQuery => matchQuery
                        .Query(word)
                        .Field(w => w.Word))));

            return response.Documents.First().Sysnonyms;
        }

        public void CreateIndex()
        {
            var response = _client.CreateIndex(_index,
                index => index
                    .Settings(
                        indexSettings => indexSettings
                            .NumberOfShards(5)
                            .NumberOfReplicas(1)
                            .Analysis(analysis => analysis
                                .Tokenizers(tokenizers => tokenizers
                                    .EdgeNGram("edgengram_tokenizer", edgeNGramTokenize => edgeNGramTokenize
                                        .MaxGram(20)
                                        .MinGram(2)
                                        .TokenChars(TokenChar.Letter, TokenChar.Digit, TokenChar.Punctuation,
                                            TokenChar.Whitespace)))
                                .Analyzers(analyzers =>
                                    analyzers.Custom("edgengram_analyzer",
                                        analyzer => analyzer
                                            .Tokenizer("edgengram_tokenizer")
                                            .Filters("lowercase")))))
                    .Mappings(
                        mappings => mappings.Map<WordWithSynonyms>(
                            mapping => mapping
                                .AllField(allField => allField.Enabled(false))
                                .Properties(props => props
                                    .String(property => property
                                        .Name(w => w.Word)
                                        .Analyzer("edgengram_analyzer")
                                        .IndexOptions(IndexOptions.Docs)
                                        .Index(FieldIndexOption.Analyzed)
                                        .Store()
                                    ).String(property => property
                                        .Name(w => w.Keyword)
                                        .NotAnalyzed()
                                        .Store()
                                    ).String(property => property
                                        .Name(w => w.Sysnonyms)
                                        .NotAnalyzed()
                                        .IndexOptions(IndexOptions.Freqs)
                                        .Store()
                                    )))));

            if (!response.Acknowledged)
            {
                throw response.OriginalException;
            }
        }

        public void TearDown()
        {
            var response = _client.DeleteIndex(_index);

            if (!response.Acknowledged)
            {
                throw response.OriginalException;
            }
        }
    }
}