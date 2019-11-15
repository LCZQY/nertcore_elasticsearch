using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;

namespace DotNetLive.House.Search.Controllers
{
    public class Article
    {
        public int Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        } = DateTime.Now;

        public decimal Price
        {
            get;
            set;
        }


        public double Number
        {
            get;
            set;
        }

        public decimal? IsNullPrice
        {
            get;
            set;
        }


        public double? IsNullNumber
        {
            get;
            set;
        }
    }

    public class HomeController : Controller
    {

        public readonly BuildShopDbContext _dbContext;
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("buildingbaseinfo");
        public HomeController(BuildShopDbContext dbcontext)
        {
            this._dbContext = dbcontext;                 
        }

      


        /// <summary>
        /// 同步mysql数据到es中
        /// </summary>
        /// <returns></returns>
        public bool InitializationEs()
        {                                           
            //保证该数据中有一个不为空

            //DotNetSearch search = new DotNetSearch().UseIndex("test");
            //var result = search.Index<Article>(new Article
            //{
            //    Id =4,
            //    Author = "zhangsan4",
            //    Content = "this is an article4",
            //    Title = "test article4",
            //    Number = 2200.5,
            //    Price = 252,
            //    IsNullNumber = 1000,
            //    IsNullPrice = 150,                
            //});
         
            //同步数据
            var list = _dbContext.buildingBaseInfos.ToList();
            // list.Where(h=> string.IsNullOrWhiteSpace(h.Id)).ToList().ForEach(x=>x.Id = Guid.NewGuid().ToString());
            return dotnetsearch.Bulk(list as IEnumerable<BuildingBaseInfo>) > 0;   
            
        }




        public IActionResult BuildShopView(List<BuildingBaseInfo> baseInfos)
        {
            var list = _dbContext.buildingBaseInfos.ToList();
            ViewData["data"] = list;
            return View();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(string name)
        {


            var keys = new string[] { "address", "name" };
            int page = 1;
            int size = 20;
            //查询参数构造
            IPageParam pageParams = new PageParamWithSearch
            {
                PageIndex = page,
                PageSize = size,
                KeyWord = name,
                Operator = Nest.Operator.Or, //拼接条件
                SearchKeys = keys,
                Highlight = new HighlightParam
                {
                    Keys = keys,
                    PostTags = "</strong>",
                    PreTags = "<strong  style='color:red;'>",
                    //PrefixOfKey = "h_"//替换字段前缀
                }
            };
            //返回查询结果         
            var select = dotnetsearch.Query<BuildingBaseInfo>(pageParams);
            var  ids = select.List.Select(x=>x.Id).ToList();
            //在mysql 查询出来
            var list = _dbContext.buildingBaseInfos.Where(h => ids.Contains(h.Id)).ToList();
            //var list = _dbContext.buildingBaseInfos.FirstOrDefault();
            //var frist = new List<BuildingBaseInfo> { list };            



            // dotnetsearch.Query<BuildingBaseInfo>();            
            return View(list);

        }

        /// <summary>
        /// 新增房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
       
        /// <summary>
        /// 编辑房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit()
        {
            return View();
        }

        /// <summary>
        /// 删除房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Delete()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
