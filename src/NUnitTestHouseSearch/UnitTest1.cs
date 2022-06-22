using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using System;
using Xunit;
using System.Linq;
namespace NUnitTestHouseSearch
{
    /// <summary>
    /// ����-ES-���������
    /// </summary>
    public class UnitTest1
    {
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("xkj_fy_buildingbaseinfos").SetType("buildingbaseinfos_type");



        [Fact(DisplayName = "���ڵ���")]
        public void Test2()
        {
            var result = dotnetsearch.AddGreaterThanEqual<BuildingBaseInfo>("builtuparea", "200");
            Assert.True(result.Count()>0,"������ѯ�ɹ�");


        }


        [Fact(DisplayName = "������ͨ��ѯ")]
        public void Test1()
        {
            var keys = new string[] { "address", "name" };
            int page = 1;
            int size = 20;

            //��ѯ��������
            IPageParam pageParams = new PageParamWithSearch
            {
                PageIndex = page,
                PageSize = size,
                KeyWord = "����",
                Operator = Nest.Operator.Or, //ƴ������
                SearchKeys = keys,
                Highlight = new HighlightParam
                {
                    Keys = keys,
                    PostTags = "</strong>",
                    PreTags = "<strong>",
                    PrefixOfKey = "h_"//�滻�ֶ�ǰ׺
                }
            };
            //���ز�ѯ���         
            var select = dotnetsearch.Query<BuildingBaseInfo>(pageParams);
            var list = select.List;
            Assert.True(list.Count() > 0, "������ѯ�ɹ�");
        }



        /// <summary>
        /// ����
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [Fact(DisplayName = "���Զ���������")]
        public void Index()
        {

            CourseEsSearchInput input = new CourseEsSearchInput
            {
                Key = "����",
                PriceStart = 0,
                PriceEnd = 100000
            };
            int pageIndex = 1;
            pageIndex = pageIndex > 0 ? pageIndex : 1;

            //var musts = new List<Func<QueryContainerDescriptor<CourseEsDto>, QueryContainer>>();
            var musts = EsUtil.Must<BuildingBaseInfo>();
            //if (!string.IsNullOrWhiteSpace(input.School))
            //{
            //    //musts.Add(c => c.Term(cc => cc.Field("School").Value(input.School)));
            //    musts.AddMatch("school", input.School);
            //}

            if (!string.IsNullOrWhiteSpace(input.Key))
            {
                //musts.Add(c => c.MultiMatch(cc => cc.Fields(ccc => ccc.Fields(ced => new[] {ced.Title, ced.School})).Query(input.Key)));
                musts.Add(c => c.MultiMatch(cc => cc.Query(input.Key).Fields(new[] { "name", "address" })));
            }

            var must2 = EsUtil.Must<BuildingBaseInfo>();

            if (!string.IsNullOrWhiteSpace(input.Ver1))
            {
                //musts.Add(c => c.Term(cc => cc.Ver1, input.Ver1));
                must2.AddTerm("summary", input.Ver1);
            }

            //if (!string.IsNullOrWhiteSpace(input.Ver2))
            //{
            //    //musts.Add(c => c.Term(cc => cc.Field(ced => ced.Ver2).Value(input.Ver2)));
            //    must2.AddTerm("ver2", input.Ver2);
            //}

            //if (!string.IsNullOrWhiteSpace(input.Ver3))
            //{
            //    //musts.Add(c => c.Term(cc => cc.Field(ced => ced.Ver3).Value(input.Ver2)));
            //    must2.AddTerm("ver3", input.Ver3);

            //}

            if (input.PriceStart.HasValue)
            {
                //musts.Add(c => c.Range(cc => cc.Field(ccc => ccc.Price).GreaterThan((double)input.PriceStart.Value)));
                must2.AddGreaterThan("maxPrice", (double)input.PriceStart.Value);
            }

            if (input.PriceEnd.HasValue)
            {
                //musts.Add(c => c.Range(cc => cc.Field(ccc => ccc.Price).LessThanOrEquals((double)input.PriceEnd.Value)));
                must2.AddLessThanEqual("maxPrice", (double)input.PriceEnd.Value);
            }

            var client = dotnetsearch.Client();
            
            var result = client.Search<BuildingBaseInfo>(sd =>
                sd.Query(qcd => qcd
                        .Bool(cc => cc
                            .Must(musts)
                            .Filter(must2)
                        ))
                        .From(10 * (pageIndex - 1))
                        .Take(10)
                    //.Sort(sdd => sdd.Descending("price"))
                    .Sort(EsUtil.SortDesc<BuildingBaseInfo>(c => c.BuiltupArea))
            );
            var total = result.Total;
            var data = result.Documents;
            Assert.True(data != null, "��ѯEsʧ�ܣ�����");
        }
    }

    public class CourseEsSearchInput
    {
        public int? PriceStart { get; set; }

        public int? PriceEnd { get; set; }

        public string Ver1 { get; set; }

        public string Key { get; set; }

    }
}