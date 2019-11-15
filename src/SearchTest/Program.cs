using DotNetLive.Search.Engine.Client;
using System;

namespace SearchTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Create();
            Console.ReadKey();

        }

        public static void Create()
        {
            DotNetSearch dotnetsearch = new DotNetSearch() { };
            dotnetsearch.UseIndex("1"); //指定索引名称
            var model = new OrderInfo { Id = Guid.NewGuid().ToString(), AddressId = Guid.NewGuid().ToString(),OrderId = Guid.NewGuid().ToString(),PassengerId  = Guid.NewGuid().ToString(), Price = new Random().Next(100)};
            var index = dotnetsearch.Index(model); //新增
            var query = dotnetsearch.Query<OrderInfo>(model.Id, "1"); //实体得id就是该条数据得id
            dotnetsearch.Update<OrderInfo>(new OrderInfo // 修改
            {
                Id = model.Id,
                AddressId = "修改后AddressId",
                OrderId = "修改后的OrderId",
                PassengerId = "修改后的PassengerId",
                Price = 200
            });

           /// 如何配合数据库进行企业级的项目搭建 ？？？？
           /// 房源项目的搜索》》
           /// 不推荐直接使用es 因为做不到事务处理，还是要配合mysql使用， 在创建es 索引时，新增一个数据id， 在后续的搜索就把该id 拿到mysql中查询即可 ！！！！
           ///注意分词带来的搜索不准确问题，推荐使用ik分词
           ///如何实现搜索后自动补全的效果：： search-as-you-type  (一般是统计用户搜索热词，提前提示出来？)
           ///搜索引擎优化：提高某些关键字的权重搜索....

        }
    }

    public class OrderInfo
    {


        public string Id { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>

        public string OrderId { get; set; }


        public string PassengerId { get; set; }


        public string AddressId { get; set; }

        /// <summary>
        /// 订单价格
        /// </summary>

        public int Price { get; set; }

    }

     
    }