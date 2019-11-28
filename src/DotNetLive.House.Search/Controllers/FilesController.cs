using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetLive.House.Search.Controllers
{
    public class MoUser
    {
        public int UserId { get; set; } = 1;
        public string UserName { get; set; } = "神牛步行3";
        public List<IFormFile> MyPhoto { get; set; }
    }
    public class MoData
    {

        public string Msg { get; set; }
        public int Status { get; set; }
    }

    public class MoBar
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 0: 失败，1：上传中，2：上传成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 当前上传大小
        /// </summary>
        public long CurrBar { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public long TotalBar { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public string PercentBar
        {
            get
            {
                return $"{(this.CurrBar * 100 / this.TotalBar)}%";
            }
        }

    }



    public class FilesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly IMemoryCache _cache;
        public FilesController(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {

            this._hostingEnvironment = hostingEnvironment;
            this._cache = cache;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// form提交上传
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FileUp(MoUser user)
        {
            if (user.MyPhoto == null || user.MyPhoto.Count <= 0)
            {
                ViewData["MsgBox"] = "请上传图片。";// return View();
            }

            //var file = Request.Form.Files;
            foreach (var file in user.MyPhoto)
            {
                var fileName = file.FileName;
                var contentType = file.ContentType;
                var len = file.Length;
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
                    await file.CopyToAsync(stream);
                }
            }
            ViewData["MsgBox"] = "上传成功";

            return View("index");
        }

        /// <summary>
        /// ajax无上传进度效果上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AjaxFileUp()
        {
            var data = new MoData { Msg = "上传失败" };
            try
            {
                var files = Request.Form.Files.Where(b => b.Name == "MyPhoto01");
                //非空限制
                if (files == null || files.Count() <= 0) { data.Msg = "请选择上传的文件。"; return Json(data); }
                //格式限制
                var allowType = new string[] { "image/jpeg", "image/png" };
                if (files.Any(b => !allowType.Contains(b.ContentType)))
                {
                    data.Msg = $"只能上传{string.Join(",", allowType)}格式的文件。";
                    return Json(data);
                }
                //大小限制
                if (files.Sum(b => b.Length) >= 1024 * 1024 * 4)
                {
                    data.Msg = "上传文件的总大小只能在4M以下。"; return Json(data);
                }
                //写入服务器磁盘
                foreach (var file in files)
                {
                    var fileName = file.FileName;
                    var webroot = _hostingEnvironment.WebRootPath + "\\images\\";
                    var path = Path.Combine(webroot, fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                data.Msg = "上传成功";
                data.Status = 2;
            }
            catch (Exception ex)
            {
                data.Msg = ex.Message;
            }
            return Json(data);
        }

        #region 单个上传图片显示进度


        private string cacheKey = "UserId_UpFile";  
        /// <summary>
         /// ajax上传进度效果上传
         /// </summary>
         /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AjaxFileUp02()
        {
            var data = new MoData { Msg = "上传失败" };
            try
            {
                var files = Request.Form.Files.Where(b => b.Name == "MyPhoto02");
                //非空限制
                if (files == null || files.Count() <= 0) { data.Msg = "请选择上传的文件。"; return Json(data); }
                //格式限制
                var allowType = new string[] { "image/jpeg", "image/png" };
                if (files.Any(b => !allowType.Contains(b.ContentType)))
                {
                    data.Msg = $"只能上传{string.Join(",", allowType)}格式的文件。";
                    return Json(data);
                }
                //大小限制
                if (files.Sum(b => b.Length) >= 1024 * 1024 * 4)
                {
                    data.Msg = "上传文件的总大小只能在4M以下。"; return Json(data);
                }
                //初始化上传多个文件的Bar，存储到缓存中，方便获取上传进度
                var listBar = new List<MoBar>();
                files.ToList().ForEach(b =>
                {
                    listBar.Add(new MoBar
                    {
                        FileName = b.FileName,
                        Status = 1,
                        CurrBar = 0,
                        TotalBar = b.Length
                    });
                });
                _cache.Set<List<MoBar>>(cacheKey, listBar);
                //写入服务器磁盘
                foreach (var file in files)
                {
                    //总大小
                    var totalSize = file.Length;
                    //初始化每次读取大小
                    var readSize = 1024L;
                    var bt = new byte[totalSize > readSize ? readSize : totalSize];
                    //当前已经读取的大小
                    var currentSize = 0L;
                    var fileName = file.FileName;
                    var webroot = _hostingEnvironment.WebRootPath + "\\images\\";
                    var path = Path.Combine(webroot, fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        //await file.CopyToAsync(stream);
                        //进度条处理流程
                        using (var inputStream = file.OpenReadStream())
                        {
                            //读取上传文件流
                            while (await inputStream.ReadAsync(bt, 0, bt.Length) > 0)
                            {
                                //当前读取的长度
                                currentSize += bt.Length;
                                //写入上传流到服务器文件中
                                await stream.WriteAsync(bt, 0, bt.Length);
                                //获取每次读取的大小
                                readSize = currentSize + readSize <= totalSize ? readSize :
                            totalSize - currentSize;
                                //重新设置
                                bt = new byte[readSize];
                                //设置当前上传的文件进度，并重新缓存到进度缓存中
                                var bars = _cache.Get<List<MoBar>>(cacheKey);
                                var currBar = bars.Where(b => b.FileName == fileName).SingleOrDefault();
                                currBar.CurrBar = currentSize;
                                currBar.Status = currentSize >= totalSize ? 2 : 1;
                                _cache.Set<List<MoBar>>(cacheKey, bars);
                                System.Threading.Thread.Sleep(100 * 1);
                            }
                        }
                    }
                }
                data.Msg = "上传完成";
                data.Status = 2;
            }
            catch (Exception ex)
            {
                data.Msg = ex.Message;
            }
            return Json(data);
        }

        /// <summary>
        /// 每200毫秒读取本缓存的进度
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProgresBar02()
        {
            var bars = new List<MoBar>();
            try
            {
                bars = _cache.Get<List<MoBar>>(cacheKey);
            }
            catch (Exception ex)
            {
            }
            return Json(bars);
        }
        #endregion

        private string cacheKey03 = "UserId_UpFile03";

        /// <summary>
        /// ajax上传进度效果上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxFileUp03()
        {
            var data = new MoData { Msg = "上传失败" };
            try
            {
                var files = Request.Form.Files.Where(b => b.Name == "MyPhoto03");
                //非空限制
                if (files == null || files.Count() <= 0) { data.Msg = "请选择上传的文件。"; return Json(data); }
                //格式限制
                var allowType = new string[] { "image/jpeg", "image/png" };
                if (files.Any(b => !allowType.Contains(b.ContentType)))
                {
                    data.Msg = $"只能上传{string.Join(",", allowType)}格式的文件。";
                    return Json(data);
                }
                //大小限制
                if (files.Sum(b => b.Length) >= 1024 * 1024 * 4)
                {
                    data.Msg = "上传文件的总大小只能在4M以下。"; return Json(data);
                }
                //初始化上传多个文件的Bar，存储到缓存中，方便获取上传进度
                var listBar = new List<MoBar>();
                files.ToList().ForEach(b =>
                {
                    listBar.Add(new MoBar
                    {
                        FileName = b.FileName,
                        Status = 1,
                        CurrBar = 0,
                        TotalBar = b.Length
                    });
                });
                _cache.Set<List<MoBar>>(cacheKey03, listBar);
                var len = files.Count();
                Task[] tasks = new Task[len];
                //写入服务器磁盘
                for (int i = 0; i < len; i++)
                {
                    var file = files.Skip(i).Take(1).SingleOrDefault();
                    tasks[i] = Task.Factory.StartNew((p) =>
                    {
                        var item = p as IFormFile;
                        //总大小
                        var totalSize = item.Length;
                        //初始化每次读取大小
                        var readSize = 1024L;
                        var bt = new byte[totalSize > readSize ? readSize : totalSize];
                        //当前已经读取的大小
                        var currentSize = 0L;
                        var fileName = item.FileName;
                        var webroot = _hostingEnvironment.WebRootPath + "\\images\\";
                        var path = Path.Combine(webroot, fileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            //进度条处理流程
                            using (var inputStream = item.OpenReadStream())
                            {
                                //读取上传文件流
                                while (inputStream.Read(bt, 0, bt.Length) > 0)
                                {
                                    //当前读取的长度
                                    currentSize += bt.Length;
                                    //写入上传流到服务器文件中
                                    stream.Write(bt, 0, bt.Length);
                                    //获取每次读取的大小
                                    readSize = currentSize + readSize <= totalSize ?
                                      readSize :
                                      totalSize - currentSize;
                                    //重新设置
                                    bt = new byte[readSize];
                                    //设置当前上传的文件进度，并重新缓存到进度缓存中
                                    var bars = _cache.Get<List<MoBar>>(cacheKey03);
                                    var currBar = bars.Where(b => b.FileName == fileName).SingleOrDefault();
                                    currBar.CurrBar = currentSize;
                                    currBar.Status = currentSize >= totalSize ? 2 : 1;
                                    _cache.Set<List<MoBar>>(cacheKey03, bars);
                                    System.Threading.Thread.Sleep(200 * 1);
                                }
                            }
                        }
                    }, file);
                }
                //任务等待 ，这里必须等待，不然会丢失上传文件流
                Task.WaitAll(tasks);
                data.Msg = "上传完成";
                data.Status = 2;
            }
            catch (Exception ex)
            {
                data.Msg = ex.Message;
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult ProgresBar03()
        {
            var bars = new List<MoBar>();
            try
            {
                bars = _cache.Get<List<MoBar>>(cacheKey03);
            }
            catch (Exception ex)
            {
            }
            return Json(bars);
        }



    }
}