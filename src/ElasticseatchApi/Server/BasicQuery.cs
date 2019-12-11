using ElasticseatchApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticseatchApi.Server
{
    /// <summary>
    /// 基本查询
    /// </summary>
    public class BasicQuery
    {
        private string _index { get; set; }


        /// <summary>
        /// 索引名称
        /// </summary>
        /// <param name="index"></param>
        public BasicQuery(string index)
        {
            _index = index;
        }

        /// <summary>
        /// 查询该索引下面的所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> All<T>() 
        {
            var _url = $"{_index}/_search";
            var _search = HttclientRequest.GetResponse<T>(_url);
            return _search;
        }




    }
}
