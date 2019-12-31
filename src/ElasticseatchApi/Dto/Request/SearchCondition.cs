using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticseatchApi.Dto.Request
{
    public class SearchCondition
    {

        /// <summary>
        /// 楼盘名称或者区域
        /// </summary>
        public string keyword { get; set; }

        /// <summary>
        /// 最大价格
        /// </summary>
        public int? maxPrice { get; set; }

        /// <summary>
        /// 最小价格
        /// </summary>
        public int? minPrice { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public int? maxArea { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public int? minArea { get; set; }

        /// <summary>
        /// 倒排序条件
        /// </summary>
        public SearchDesc? searchDesc { get; set; }
    }

    public enum SearchDesc
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        CreateTime = 1,
        /// <summary>
        /// 价格
        /// </summary>
        Price = 2,
        /// <summary>
        /// 面积
        /// </summary>
        Area = 3,

        /// <summary>
        /// 总价
        /// </summary>
        TotalPrice
    }
}
