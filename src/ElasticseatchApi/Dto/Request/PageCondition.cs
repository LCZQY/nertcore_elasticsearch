using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticseatchApi.Dto.Request
{


    public class PageCondition
    {

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }

    /// <summary>
    /// 搜索关键字
    /// </summary>
    public class SeachRequest : PageCondition
    {

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }


    }
}
