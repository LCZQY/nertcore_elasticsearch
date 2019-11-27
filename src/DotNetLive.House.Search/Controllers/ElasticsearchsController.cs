using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetLive.House.Search.Controllers
{

    /// <summary>
    /// 自己封装的ES增删改查
    /// </summary>
    public class ElasticsearchsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}