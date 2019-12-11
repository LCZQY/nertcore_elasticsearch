using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetLive.House.Search.Controllers
{ 
    public class HomeController : Controller
    {

        public readonly BuildShopDbContext _dbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("xkj_fy_buildingbaseinfos").SetType("buildingbaseinfos_type");
        
        public HomeController(BuildShopDbContext dbcontext, IHostingEnvironment hostingEnvironment)
        {
            this._dbContext = dbcontext;
            this._hostingEnvironment = hostingEnvironment;
        }




        ///// <summary>
        ///// 同步mysql数据到es中
        ///// </summary>
        ///// <returns></returns>
        //public bool InitializationEs()
        //{
        //    //保证该数据中有一个不为空

        //    //DotNetSearch search = new DotNetSearch().UseIndex("test");
        //    //var result = search.Index<Article>(new Article
        //    //{
        //    //    Id =4,
        //    //    Author = "zhangsan4",
        //    //    Content = "this is an article4",
        //    //    Title = "test article4",
        //    //    Number = 2200.5,
        //    //    Price = 252,
        //    //    IsNullNumber = 1000,
        //    //    IsNullPrice = 150,                
        //    //});

        //    //同步数据
        //    var list = _dbContext.buildingBaseInfos.ToList();
        //    // list.Where(h=> string.IsNullOrWhiteSpace(h.Id)).ToList().ForEach(x=>x.Id = Guid.NewGuid().ToString());
        //    return dotnetsearch.Bulk(list as IEnumerable<BuildingBaseInfo>) > 0;
        //}


        public IActionResult BuildShopView(int pageIdnex = 0, int pageSize = 10)
        {
            var list = _dbContext.buildingBaseInfos.Where(y => !y.IsDeleted).Skip(pageIdnex * pageSize).Take(pageSize).
                OrderByDescending(u => u.CreateTime).ToList();
            list.ForEach(y =>
            {
                if (y.Summary != null && y?.Summary?.Length > 30)
                    y.Summary = y.Summary.Substring(0, 30) + "......";
            });           
            ViewData["data"] = list;
            return View();
        }


        /// <summary>
        /// 新增页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
          
            return View();
        }


        /// <summary>
        /// 新增房源信息
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateUpload(BuildingBaseInfo baseInfo)
        {

 

            string image =await UploadFile(baseInfo.File[0]);
            if (string.IsNullOrWhiteSpace(image))
            {
                return new BadRequestObjectResult("图片上传失败");
            }
            var deatil = new BuildingBaseInfo();
            deatil.Address = baseInfo.Address;
            deatil.Name = baseInfo.Name;
            deatil.MaxPrice = baseInfo.MaxPrice;
            deatil.MinPrice = baseInfo.MinPrice;
            deatil.Summary = baseInfo.Summary;
            deatil.BuiltupArea = baseInfo.BuiltupArea;
            deatil.Icon = image;
            deatil.CreateTime = DateTime.Now;
            deatil.UpdateTime = DateTime.Now;
            deatil.IsDeleted = false;

            _dbContext.Add(deatil);
            if (_dbContext.SaveChanges() > 0)
            {
                return RedirectToAction("BuildShopView");
            }           
            return new BadRequestResult();
        }

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(string Id)
        {
            var deatil = _dbContext.buildingBaseInfos.SingleOrDefault(s => s.Id == Id);
            return View(deatil);
        }

        ///<summary>
        /// 编辑房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult EditUpload(BuildingBaseInfo baseInfo)
        {
            var deatil = _dbContext.buildingBaseInfos.SingleOrDefault(s => s.Id == baseInfo.Id);
            deatil.Address = baseInfo.Address;
            deatil.Name = baseInfo.Name;
            deatil.MaxPrice = baseInfo.MaxPrice;
            deatil.MinPrice = baseInfo.MinPrice;
            deatil.Summary = baseInfo.Summary;
            deatil.BuiltupArea = baseInfo.BuiltupArea;
            deatil.UpdateTime = DateTime.Now;
            _dbContext.Attach(deatil);
            _dbContext.Update(deatil);
            if (_dbContext.SaveChanges() > 0)
            {
                return RedirectToAction("BuildShopView");
            }            
            return new BadRequestResult();
        }

        /// <summary>
        /// 删除房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Delete(string Id)
        {

            var deatil = _dbContext.buildingBaseInfos.SingleOrDefault(s => s.Id == Id);
            deatil.IsDeleted = true;
            deatil.UpdateTime = DateTime.Now;
            _dbContext.Attach(deatil);
            _dbContext.Update(deatil);
            if (_dbContext.SaveChanges() > 0)
            {
                return RedirectToAction("BuildShopView");
            }         
            return new BadRequestResult();
        }

        /// <summary>
        /// 上传附件(单个附件)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> UploadFile(IFormFile MyPhoto)
        {
            try
            {
                if (MyPhoto == null || MyPhoto.Length <= 0)
                {
                    ViewData["MsgBox"] = "请上传图片。";// return View();
                }
                //var file = Request.Form.Files;
                var fileName = MyPhoto.FileName;
                var contentType = MyPhoto.ContentType;
                var len = MyPhoto.Length;
                var fileType = new string[] { "image/jpeg", "image/png" };
                if (!fileType.Any(b => b.Contains(contentType)))
                {
                    ViewData["MsgBox"] = ($"只能上传{string.Join(",", fileType)}格式的图片。"); //return View(); 
                }
                if (len > 1024 * 1024 * 4)
                {
                    ViewData["MsgBox"] = ("上传图片大小只能在4M以下。"); //return View();
                }
                var webroot = _hostingEnvironment.WebRootPath + "\\images\\";
                var path = Path.Combine(webroot, fileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await MyPhoto.CopyToAsync(stream);
                }
                //为了方便展示，我这儿只是返回了文件名称，按道理应该返回该文件所在的路径
                return fileName;
            }
            catch (Exception ex)
            {
                return "";
            }
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
                    PostTags = "</h1>",
                    PreTags = "<h1>",
                 //   PrefixOfKey = "h_"//替换字段前缀
                }
            };

            //返回查询结果         
            var select = dotnetsearch.Query<BuildingBaseInfo>(pageParams);
            var list = select.List.Where(u=> !u.IsDeleted).ToList();
            list.ForEach(y =>
            {
                
                if (y.Summary != null && y?.Summary?.Length > 30)
                    y.Summary = y.Summary.Substring(0, 30) + "......";
            });
            ViewData["data"] = list;
            //var  ids = select.List.Select(x=>x.Id).ToList();
            //在mysql 查询出来
            //var list = _dbContext.buildingBaseInfos.Where(h => ids.Contains(h.Id)).ToList();
            //var list = _dbContext.buildingBaseInfos.FirstOrDefault();
            //var frist = new List<BuildingBaseInfo> { list };            
            // dotnetsearch.Query<BuildingBaseInfo>();            
            return View("BuildShopView");
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
