using System;
using System.Collections.Generic;

namespace Thesaurus.Tests
{
    public class ElasticForTests : Elastic
    {
        public ElasticForTests(int version = 1) : base("http://goelastic:9200", $"thesaurus_test_{version}")
        {
        }

        public Action<IList<WordWithSynonyms>> OnAddOrUpdate { get; set; }
        public override void AddOrUpdate(IList<WordWithSynonyms> documents)
        {
            if (OnAddOrUpdate != null)
            {
                OnAddOrUpdate.Invoke(documents);
                return;
            }

            base.AddOrUpdate(documents);
        }
    }
}