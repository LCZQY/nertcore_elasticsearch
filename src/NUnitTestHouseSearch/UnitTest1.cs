using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using System;
using Xunit;
using System.Linq;
namespace NUnitTestHouseSearch
{
    /// <summary>
    /// 测试-ES-的搜索结果
    /// </summary>
    public class UnitTest1
    {
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("buildingbaseinfo");

        [Fact(DisplayName ="测试普通查询")]
        public void Test1()
        {
            var keys = new string[] { "address","name" };
            int page = 1;
            int size = 20;

            //查询参数构造
            IPageParam pageParams = new PageParamWithSearch
            {
                PageIndex = page,
                PageSize = size,
                KeyWord = "重庆",
                Operator = Nest.Operator.Or, //拼接条件
                SearchKeys = keys,
                Highlight = new HighlightParam
                {
                    Keys = keys,
                    PostTags = "</strong>",
                    PreTags = "<strong>",
                    PrefixOfKey = "h_"//替换字段前缀
                }
            };
            //返回查询结果         
            var select = dotnetsearch.Query<BuildingBaseInfo>(pageParams);
            var list = select.List;      
        }
    }
}
