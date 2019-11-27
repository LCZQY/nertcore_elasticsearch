using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetLive.House.Search.Models;
using DotNetLive.Search.Engine.Client;
using DotNetLive.Search.Engine.Model;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

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
        private readonly IHostingEnvironment _hostingEnvironment;
        DotNetSearch dotnetsearch = new DotNetSearch().UseIndex("buildingbaseinfo");
        public HomeController(BuildShopDbContext dbcontext, IHostingEnvironment hostingEnvironment)
        {
            this._dbContext = dbcontext;
            this._hostingEnvironment = hostingEnvironment;
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
            list.ForEach(y =>
            {
                if (y.Summary != null && y?.Summary?.Length > 30)
                    y.Summary = y.Summary.Substring(0, 30) + "......";
            });
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
            var list = select.List.ToList();
            //var  ids = select.List.Select(x=>x.Id).ToList();
            //在mysql 查询出来
            //var list = _dbContext.buildingBaseInfos.Where(h => ids.Contains(h.Id)).ToList();
            //var list = _dbContext.buildingBaseInfos.FirstOrDefault();
            //var frist = new List<BuildingBaseInfo> { list };            
            // dotnetsearch.Query<BuildingBaseInfo>();            
            return View(list);

        }







        /// <summary>
        /// 新增页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            //>> 如何做到与 mysql 的数据同步?
            //https://github.com/jprante/elasticsearch-jdbc
            return View();
        }


        /// <summary>
        /// 新增房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateUpload(BuildingBaseInfo baseInfo)
        {
            string image = UploadFile();
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

            _dbContext.Add(deatil);
            if (_dbContext.SaveChanges() > 0)
            {
                return View("BuildShopView");
            }
            // 如何结合ES ?

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
                return View("BuildShopView");
            }
            // 如何结合ES ?

            return new BadRequestResult();
        }

        /// <summary>
        /// 删除房源信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Delete(string Id)
        {


            var deatil = _dbContext.buildingBaseInfos.SingleOrDefault(s => s.Id == Id);
            _dbContext.Attach(deatil);
            _dbContext.Remove(deatil);
            if (_dbContext.SaveChanges() > 0)
            {
                return View("BuildShopView");
            }
            // 如何结合ES ?
            return new BadRequestResult();
        }

        /// <summary>
        /// 上传附件(单个附件)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadFile()
         {
            try
            {
                string Id = "";//记录返回的附件Id
                string filePath = "";//记录文件路径
                IFormFileCollection formFiles = Request.Form.Files;//获取上传的文件
                if (formFiles == null || formFiles.Count == 0)
                {
                    return "";// Json(new { status = -1, message = "没有上传文件", filepath = "" });
                }
                IFormFile file = formFiles[0];
                string fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);//获取文件名称后缀 
                //保存文件
                var stream = file.OpenReadStream();
                // 把 Stream 转换成 byte[] 
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);

                var path = _hostingEnvironment.WebRootPath + "images\\" + file.FileName;
                // 把 byte[] 写入文件 
                FileStream fs = new FileStream(path, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
                return path; //Json(new { success = true, status = 0, message = "上传成功", filepath = "D:\\" + file.FileName, code = Id });
            }

            catch (Exception ex)
            {
                return "";//Json(new { success = false, status = -3, message = "上传失败", data = ex.Message, code = "" });
            }
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
