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
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("buildingbaseinfo");

        [Fact(DisplayName ="������ͨ��ѯ")]
        public void Test1()
        {
            var keys = new string[] { "address","name" };
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
        }
    }
}
