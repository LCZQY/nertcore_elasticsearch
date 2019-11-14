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
