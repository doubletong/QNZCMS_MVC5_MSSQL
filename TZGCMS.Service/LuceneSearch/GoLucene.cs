using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;

namespace TZGCMS.Service.LuceneSearch
{
    public static class GoLucene
    {
        // properties
        public static string _luceneDir =
            Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        // search methods
        public static IEnumerable<SearchData> GetAllIndexRecords()
        {
           
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<SearchData>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);          
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            
            // v 2.9.4: use 'term.Doc()'
            // v 3.0.3: use 'term.Doc'
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }
        public static IEnumerable<SearchData> Search(int pageIndex, int pageSize, out int totalCount, string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input))
            {
                totalCount = 0;
                return new List<SearchData>();
            }
            int total = 0;
            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            var list = _search(pageIndex, pageSize, out total, input, fieldName);
            totalCount = total;

            return list;
        }
        public static IEnumerable<SearchData> SearchDefault(int pageIndex,int pageSize, out int totalCount,string input, string fieldName = "")
        {
            int total = 0;
            var list = string.IsNullOrEmpty(input) ? new List<SearchData>() : _search(pageIndex, pageSize,out total,input, fieldName);
            totalCount = total;
            return list;
        }

        // main search method
        private static IEnumerable<SearchData> _search(int pageIndex, int pageSize, out int totalCount,string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
            {
                totalCount = 0;
                return new List<SearchData>();
            }              
           
            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new PanGuAnalyzer();  //new StandardAnalyzer(Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    totalCount = hits.Length;

                    var results = _mapLuceneToDataList(pageIndex, pageSize, hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "Id", "Name", "Url","ImageUrl","Description", "CreatedDate" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, null, hits_limit, Sort.INDEXORDER).ScoreDocs;

                    totalCount = hits.Length;

                    var results = _mapLuceneToDataList(pageIndex, pageSize, hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
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
            var analyzer =  new PanGuAnalyzer(); //new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entries if any)
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static void ClearLuceneIndexRecord(string record_id)
        {
            // init lucene
            var analyzer =  new PanGuAnalyzer(); //new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer =  new PanGuAnalyzer(); //new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static void Optimize()
        {
            var analyzer =  new PanGuAnalyzer(); //new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }
        private static void _addToLuceneIndex(SearchData sampleData, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", sampleData.Id));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

          //  add lucene fields mapped to db fields
          //  doc.Add(new Field("Id", IdentityGenerator.SequentialGuid() sampleData.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Id", sampleData.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", sampleData.Name, Field.Store.YES, Field.Index.ANALYZED));
            if(!string.IsNullOrEmpty(sampleData.ImageUrl))
                doc.Add(new Field("ImageUrl", sampleData.ImageUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Url", sampleData.Url, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", sampleData.Description, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CreatedDate", DateTime.Now.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            // add entry to index
            writer.AddDocument(doc);
        }

    }
}
