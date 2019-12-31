using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using System;
using Xunit;
using System.Linq;
using Engine;

namespace NUnitTestHouseSearch
{
    /// <summary>
    /// 测试-ES-的搜索结果
    /// </summary>
    public class PlainElasticSearch
    {
        ElasticSearchHelper helper = new ElasticSearchHelper() { };

        /// <summary>
        /// 
        /// </summary>
        [Fact(DisplayName ="搜索1")]
        public void Test1() {

            var reult = helper.Search<BuildingBaseInfo>("xkj_fy_buildingbaseinfos", "buildingbaseinfos_type","重庆",0,20);


        }
    }
}
