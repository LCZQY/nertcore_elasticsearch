using ElasticseatchApi.Models;
using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace ElasticseatchApi.Common
{
    public class ElasticSearchHelper
    {
        private ElasticConnection _client { get; }

        /// <summary>
        /// 索引
        /// </summary>
        private string _index { get; }
        /// <summary>
        /// 类型
        /// </summary>
        private string _type { get; set; }

        public ElasticSearchHelper(string index, string type)
        {
            _client = new ElasticConnection("localhost", 9200);
            _index = index;
            _type = type;
        }


        //public QueryContainer BuildQueryContainer(SearchCondition condition)
        //{
        //    var queryCombin = new List<Func<QueryContainerDescriptor<BuildingBaseInfo>, QueryContainer>>();
        //    if (!string.IsNullOrEmpty(condition.Name))
        //        queryCombin.Add(mt => mt.Match(m => m.Field(t => t.Name).Query(condition.Name))); //字符串匹配

        //    if (condition.Age.HasValue)
        //        queryCombin.Add(mt => mt.Range(m => m.Field(t => t.Address).GreaterThanOrEquals(condition.Age))); //数值区间匹配

        //    if (!string.IsNullOrEmpty(condition.Address))
        //        queryCombin.Add(mt => mt.MatchPhrase(m => m.Field(t => t.Address).Query(condition.Address))); //短语匹配

        //    if (!condition.Gender.HasValue)
        //        queryCombin.Add(mt => mt.Term(m => m.Field(t => t.Gender).Value(condition.Gender)));//精确匹配

        //    return PlainElastic.Net.Queries.Query<BuildingBaseInfo>.Bool(b => b
        //        .Must(queryCombin)
        //        .Filter(f => f
        //            .DateRange(dr => dr.Field(t => t.CreateTime) //时间范围匹配
        //                .GreaterThanOrEquals(DateMath.Anchored(condition.BeginCreateTime.ToString("yyyy-MM-ddTHH:mm:ss")))
        //                .LessThanOrEquals(DateMath.Anchored(condition.EndCreateTime.ToString("yyyy-MM-ddTHH:mm:ss"))))));
        //}



        /// <summary>
        ///  匹配多个字段，且的关系
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public personList OrSearch(string key, int from, int size)
        {
            var mustNameQueryKeys = new MustQuery<BuildingBaseInfo>();
            var mustIntroQueryKeys = new MustQuery<BuildingBaseInfo>();
            var arrKeys = GetIKTokenFromStr(key);
            foreach (var item in arrKeys)
            {
                mustNameQueryKeys = mustNameQueryKeys.Term(t3 => t3.Field("name").Value(item)) as MustQuery<BuildingBaseInfo>;
                mustIntroQueryKeys = mustIntroQueryKeys.Term(t3 => t3.Field("address").Value(item)) as MustQuery<BuildingBaseInfo>;
            }

            string cmd = new SearchCommand(_index, _type);
            string query = new QueryBuilder<BuildingBaseInfo>()
                  .Query(b => b.Bool(m =>
                             //或者
                             m.Should(t =>
                                        t.Bool(m1 =>
                                                    m1.Must(
                                                           t2 =>
                                                                //t2.Term(t3=>t3.Field("name").Value("研究"))
                                                                //   .Term(t3=>t3.Field("name").Value("方鸿渐"))  
                                                                mustNameQueryKeys
                                                            )
                                               )
                                      )
                              .Should(t =>
                                        t.Bool(m1 =>
                                                    m1.Must(t2 =>
                                                                    //t2.Term(t3 => t3.Field("intro").Value("研究"))
                                                                    //.Term(t3 => t3.Field("intro").Value("方鸿渐"))  
                                                                    mustIntroQueryKeys
                                                  )
                                               )
                                     )
                                  )
                        )
                 //分页
                 .From(from)
                 .Size(size)
                  //排序
                  .Sort(c => c.Field("updatetime", SortDirection.desc))
                  //添加高亮
                  .Highlight(h => h
                      .PreTags("<i>")
                      .PostTags("</i>")
                      .Fields(
                             f => f.FieldName("name").Order(HighlightOrder.score),
                             f => f.FieldName("address").Order(HighlightOrder.score),
                             f => f.FieldName("_all")
                             )
                     )
                    .Build();

            string result = _client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<BuildingBaseInfo>(result);
            personList datalist = new personList();
            datalist.hits = list.hits.total;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c => new BuildingBaseInfo
            {
                Id = c._source.Id,
                Address = string.Join("", c.highlight["address"]),
                BuiltupArea = c._source.BuiltupArea,
                MaxPrice = c._source.MaxPrice,
                Area = c._source.Area,
                CreateTime = c._source.CreateTime,
                City = c._source.City,
                District = c._source.District,
                Icon = c._source.Icon,
                IsDeleted = c._source.IsDeleted,
                MinPrice = c._source.MinPrice,
                Name = string.Join("", c.highlight["name"]),//高亮显示的内容，一条记录中出现了几次
                Summary = c._source.Summary,
                UpdateTime = c._source.UpdateTime
            }).ToList();
            datalist.list.AddRange(personList);
            return datalist;
        }


        /// <summary>
        ///  匹配一个字段，并的关系
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public personList AndSearch(string key, int from, int size)
        {
            string cmd = new SearchCommand(_index, _type);
            string query = new QueryBuilder<BuildingBaseInfo>()
                .Query(b =>
                            b.Bool(m =>
                                //并且关系
                                m.Must(t =>
                                   //分词的最小单位或关系查询
                                   t.QueryString(t1 => t1.DefaultField("name").Query(key))
                                     //.QueryString(t1=>t1.DefaultField("isDeleted").Query("0"))
                                     //.QueryString(t1 => t1.DefaultField("name").Query(key))
                                     // t .Terms(t2=>t2.Field("intro").Values("研究","方鸿渐"))
                                     //范围查询
                                     // .Range(r =>  r.Field("age").From("100").To("200") )  
                                     )
                                  )
                                )
                 //分页
                 .From(from)
                 .Size(size)
                  //排序
                  .Sort(c => c.Field("updatetime", SortDirection.desc))
                  //添加高亮
                  .Highlight(h => h
                      .PreTags("<i>")
                      .PostTags("</i>")
                      .Fields(
                             f => f.FieldName("name").Order(HighlightOrder.score),
                             f => f.FieldName("_all")
                             )
                     )
                    .Build();

            string result = _client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<BuildingBaseInfo>(result);
            personList datalist = new personList();
            datalist.hits = list.hits.total;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c => new BuildingBaseInfo
            {
                Id = c._source.Id,
                Address = c._source.Address,
                BuiltupArea = c._source.BuiltupArea,
                MaxPrice = c._source.MaxPrice,
                Area = c._source.Area,
                CreateTime = c._source.CreateTime,
                City = c._source.City,
                District = c._source.District,
                Icon = c._source.Icon,
                IsDeleted = c._source.IsDeleted,
                MinPrice = c._source.MinPrice,
                Name = string.Join("", c.highlight["name"]),//高亮显示的内容，一条记录中出现了几次
                Summary = c._source.Summary,
                UpdateTime = c._source.UpdateTime
            }).ToList();
            datalist.list.AddRange(personList);
            return datalist;
        }


        /// <summary>
        /// 将搜索词语用ik分词，返回分词结果的集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<string> GetIKTokenFromStr(string key)
        {
            string s = $"_analyze?analyzer=ik_max_word&pretty=true&text={key}";
            var result = _client.Get(s);
            var serializer = new JsonNetSerializer();
            var list = serializer.Deserialize(result, typeof(ik)) as ik;
            return list.tokens.Select(c => c.token).ToList();
        }



    }
}
