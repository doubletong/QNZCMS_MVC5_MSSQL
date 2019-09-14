using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TZGCMS.Model.Search
{
    public static class LuceneHelper
    {
        public static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "LuceneIndex");
        public static LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null)
                {
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                }

                if (IndexWriter.IsLocked(_directoryTemp))
                {
                    IndexWriter.Unlock(_directoryTemp);
                }

                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }

                return _directoryTemp;
            }
        }

       
        //public static IEnumerable<SearchData> Search(int pageIndex, int pageSize, out int totalCount, string input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        totalCount = 0;
        //        return new List<SearchData>();
        //    }
        //    int total = 0;
        //    var terms = input.Trim().Replace("-", " ").Split(' ')
        //        .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
        //    input = string.Join(" ", terms);

        //    var list = _search(pageIndex, pageSize, out total, input);
        //    totalCount = total;

        //    return list;
        //}
        public static IEnumerable<SearchData> SearchDefault(int pageIndex, int pageSize, out int totalCount, string input)
        {
            int total = 0;
            var list = string.IsNullOrEmpty(input) ? new List<SearchData>() : _search(pageIndex, pageSize, out total, input);
            totalCount = total;
            return list;
        }

        private static IEnumerable<SearchData> _search(int pageIndex, int pageSize, out int totalCount, string keyword)
        {
            var hits_limit = 1000;
            //create an analyzer to process the text
            var analyzer = new SmartChineseAnalyzer(AppLuceneVersion);

            //create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            var writer = new IndexWriter(_directory, indexConfig);

           

            var searcher = new IndexSearcher(writer.GetReader(applyAllDeletes: true));
            var parser = new MultiFieldQueryParser
                (AppLuceneVersion, new[] { "Id", "Name", "Description" }, analyzer);
            var query = parseQuery(keyword, parser);
            var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            var results = _mapLuceneToDataList(pageIndex, pageSize, hits, searcher);
            totalCount = hits.Length;
            analyzer.Dispose();
            return results;


        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
       
        // map Lucene search index to data
        private static IEnumerable<SearchData> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<SearchData> _mapLuceneToDataList(int pageIndex, int pageSize, IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            // v 2.9.4: use 'hit.doc'
            // v 3.0.3: use 'hit.Doc'
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).Skip(pageSize * pageIndex).Take(pageSize).ToList();
        }
        private static SearchData _mapLuceneDocumentToData(Document doc)
        {
            return new SearchData
            {
                Id = doc.Get("Id"),
                Name = doc.Get("Name"),
                Url = doc.Get("Url"),
                ImageUrl = doc.Get("ImageUrl"),
                Description = doc.Get("Description"),
                CreatedDate = string.IsNullOrEmpty(doc.Get("CreatedDate")) ? DateTime.MinValue : DateTime.Parse(doc.Get("CreatedDate"))
            };
        }

        // add/update/clear search index data 
        public static void AddUpdateLuceneIndex(SearchData sampleData)
        {
            AddUpdateLuceneIndex(new List<SearchData> { sampleData });
        }
        public static void AddUpdateLuceneIndex(IEnumerable<SearchData> sampleDatas)
        {
            // init lucene
            var analyzer = new SmartChineseAnalyzer(AppLuceneVersion); //new StandardAnalyzer(Version.LUCENE_30);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using (var writer = new IndexWriter(_directory, indexConfig))
            {
                // add data to lucene search index (replaces older entries if any)
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                // close handles
                analyzer.Dispose();
                writer.Dispose();
            }
        }
        public static void ClearLuceneIndexRecord(string record_id)
        {
            // init lucene
            var analyzer = new SmartChineseAnalyzer(AppLuceneVersion); //new StandardAnalyzer(Version.LUCENE_30);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using (var writer = new IndexWriter(_directory, indexConfig))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Dispose();
                writer.Dispose();
            }
        }
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new SmartChineseAnalyzer(AppLuceneVersion); //new StandardAnalyzer(Version.LUCENE_30);
                var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
                using (var writer = new IndexWriter(_directory, indexConfig))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Dispose();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        //public static void Optimize()
        //{
        //    var analyzer = new SmartChineseAnalyzer(AppLuceneVersion); //new StandardAnalyzer(Version.LUCENE_30);
        //    var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
        //    using (var writer = new IndexWriter(_directory, indexConfig))
        //    {
        //        analyzer.Close();
        //        writer.Optimize();
        //        writer.Dispose();
        //    }
        //}
        private static void _addToLuceneIndex(SearchData sampleData, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", sampleData.Id));
            writer.DeleteDocuments(searchQuery);

            var doc = new Document
            {
                // StringField indexes but doesn't tokenise
                new StringField("Id", sampleData.Id, Field.Store.YES),
                new TextField("Name", sampleData.Name, Field.Store.YES),
                new TextField("ImageUrl", string.IsNullOrEmpty(sampleData.ImageUrl) ? "/content/img/search_thumb.jpg" : sampleData.ImageUrl, Field.Store.YES),
                new TextField("Url", sampleData.Url, Field.Store.YES),
                new TextField("Description", sampleData.Description, Field.Store.YES),
                new TextField("CreatedDate", sampleData.CreatedDate.ToString(), Field.Store.YES)
            };

            writer.AddDocument(doc);
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

    }

}
