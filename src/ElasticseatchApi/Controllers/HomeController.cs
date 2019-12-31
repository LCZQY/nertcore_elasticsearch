using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCore;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using ElasticseatchApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PlainElastic.Net;
using PlainElastic.Net.Queries;
using ElasticseatchApi.Common;
using PlainElastic.Net.Serialization;
using ElasticseatchApi.Dto.Request;

namespace ElasticseatchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        public readonly BuildShopDbContext _dbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("xkj_fy_buildingbaseinfos").SetType("buildingbaseinfos_type");
        ElasticSearchHelper searchHelper = new ElasticSearchHelper("xkj_fy_buildingbaseinfos", "buildingbaseinfos_type");
         private readonly ElasticConnection Client = new ElasticConnection("localhost", 9200);
        public HomeController(BuildShopDbContext dbcontext, IHostingEnvironment hostingEnvironment)
        {
            this._dbContext = dbcontext;
            this._hostingEnvironment = hostingEnvironment;
        }

    
        /// <summary>
        /// 按照楼盘名称和区域名称找房
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<PagingResponseMessage<BuildingBaseInfo>> Search([FromBody]SeachRequest  request)
        {
            var response = new PagingResponseMessage<BuildingBaseInfo>() { };
            try
            {
                #region 查询方式二
                var building = searchHelper.OrSearch(request.Keyword, 0, 20);
                response.TotalCount = building.hits;
                response.Extension = building.list;
                response.PageIndex = request.PageIndex;
                request.PageSize = request.PageSize;
                #endregion


                #region 查询方式一
                //var keys = new string[] { "address", "name" };
                //int page = 1;
                //int size = 20;
                ////查询参数构造
                //IPageParam pageParams = new PageParamWithSearch
                //{
                //    PageIndex = page,
                //    PageSize = size,
                //    KeyWord = keyword,
                //    Operator = Nest.Operator.Or, //拼接条件
                //    SearchKeys = keys,
                //    Highlight = new HighlightParam
                //    {
                //        Keys = keys,
                //        PostTags = "<h1>",
                //        PreTags = "</h1>",
                //        //   PrefixOfKey = "h_"//替换字段前缀
                //    }
                //};
                ////返回查询结果         
                //var select = dotnetsearch.Query<BuildingBaseInfo>(pageParams);
                //var list = select.List.Where(u => !u.IsDeleted).ToList();
                //list.ForEach(y =>
                //{
                //    if (y.Summary != null && y?.Summary?.Length > 30)
                //        y.Summary = y.Summary.Substring(0, 30) + "......";
                //});
                //response.Extension = list;
                #endregion


            }
            catch (Exception es)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "搜索失败.";
            }
            return response;
        }


    }
}