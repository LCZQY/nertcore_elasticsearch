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
    /// ����-ES-���������
    /// </summary>
    public class PlainElasticSearch
    {
        ElasticSearchHelper helper = new ElasticSearchHelper() { };

        /// <summary>
        /// 
        /// </summary>
        [Fact(DisplayName ="����1")]
        public void Test1() {

            var reult = helper.Search<BuildingBaseInfo>("xkj_fy_buildingbaseinfos", "buildingbaseinfos_type","����",0,20);


        }
    }
}
