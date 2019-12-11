using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElasticseatchApi.Models
{
    public class BuildingBaseInfo
    {
    //    / <summary>
    //    / 主键Id
    //    / </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        ///// <summary>
        ///// 录入商铺数量
        ///// </summary>
        //public int? WriteShopsNumber { get; set; }
        ///// <summary>
        ///// 已售商铺数量
        ///// </summary>
        //public int? SoldShopsNumber { get; set; }

        ///// <summary>
        ///// 楼盘类型
        ///// </summary>
        //public string BuildingType { get; set; }

        /// <summary>
        /// 楼盘名称
        /// </summary>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 楼盘城市
        /// </summary>
        [MaxLength(255)]
        public string City { get; set; }
        /// <summary>
        /// 楼盘大区域
        /// </summary>
        [MaxLength(255)]
        public string District { get; set; }
        /// <summary>
        /// 楼盘小区域
        /// </summary>
        [MaxLength(255)]
        public string Area { get; set; }

        /// <summary>
        /// 总户数
        /// </summary>
        ////public int? HouseHolds { get; set; }
        ///// <summary>
        ///// 开盘时间
        ///// </summary>
        //public DateTime? OpenDate { get; set; }
        ///// <summary>
        ///// 交放时间
        ///// </summary>
        //public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// 最低售价
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// 最高售价
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 产权年限
        /// </summary>
        //public int? PropertyTerm { get; set; }
        ///// <summary>
        ///// 土地到期时间
        ///// </summary>
        //public DateTime? LandExpireDate { get; set; }
        ///// <summary>
        ///// 占地面积
        ///// </summary>
        //public double? FloorSurface { get; set; }
        /// <summary>
        /// 建设面积
        /// </summary>
        public double BuiltupArea { get; set; }
        ///// <summary>
        ///// 容积率
        ///// </summary>
        //public double? PlotRatio { get; set; }
        ///// <summary>
        ///// 绿化率
        ///// </summary>
        //public double? GreeningRate { get; set; }
        ///// <summary>
        ///// 地下室车位
        ///// </summary>
        //public int? BasementParkingSpace { get; set; }
        ///// <summary>
        ///// 地面停车位
        ///// </summary>
        //public int? ParkingSpace { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public int? BuildingNum { get; set; }
        ///// <summary>
        ///// 商铺总数
        ///// </summary>
        //public int? Shops { get; set; }
        ///// <summary>
        ///// 可售商铺总数
        ///// </summary>
        //public int? ShopsStock { get; set; }
        ///// <summary>
        ///// 物业公司
        ///// </summary>
        //[MaxLength(255)]
        //public string PMC { get; set; }
        ///// <summary>
        ///// 物管费
        ///// </summary>
        //public decimal? PMF { get; set; }
        ///// <summary>
        ///// 开发商
        ///// </summary>
        //[MaxLength(255)]
        //public string Developer { get; set; }
        //[NotMapped]
        //public string DeveloperName { get; set; }
        /// <summary>
        /// 楼盘地址
        /// </summary>
        [MaxLength(255)]
        public string Address { get; set; }



        ///// <summary>
        ///// 地图类型
        ///// </summary>
        //public string MapType { get; set; }
        ///// <summary>
        ///// 经度
        ///// </summary>
        //public string Longitude { get; set; }

        ///// <summary>
        ///// 纬度
        ///// </summary>
        //public string Latitude { get; set; }

        ///// <summary>
        ///// 腾讯经度
        ///// </summary>
        //public string TXLongitude { get; set; }

        ///// <summary>
        ///// 腾讯纬度
        ///// </summary>
        //public string TXLatitude { get; set; }

        #region v1.5 楼盘层级调整新增字段


        /// <summary>
        /// 楼盘封面
        /// </summary>
        [MaxLength(300)]
        public string Icon { get; set; }

        [NotMapped]
        public List<IFormFile> File { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        //public int? ExamineStatus { get; set; }

  //      /// <summary>
  //      /// 审核保护期
  //      /// </summary>
  //      public DateTime? IsCreateAlterDate { get; set; }

  //      /// <summary>
  //      /// 所属公司
  //      /// </summary>
		//[MaxLength(36)]
  //      public string FilialeId { get; set; }

  //      /// <summary>
  //      /// 审核时间
  //      /// </summary>
  //      public DateTime? ReviewTime { get; set; }

        /// <summary>
        /// 是否更改
        /// </summary>
        //public int? IsUpdate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
		[MaxLength(1000)]
        public string Summary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

    

  

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        #endregion

        //// 1.8 新加的字段
        ///// <summary>
        ///// 楼盘地图上的配套信息 冗余字段,只有查询会使用到|值| 是否有医院这种数据,有值表示有
        ///// </summary>
        //public string Facilities { get; set; }

        ///// <summary>
        ///// 规划户数
        ///// </summary>
        ////public int? HousesNumber { get; set; }
        ///// <summary>
        ///// 住宅物业费
        ///// </summary>
        //public decimal? HousPMF { get; set; }

        ///// <summary>
        ///// 车位分子
        ///// </summary>
        //public decimal? ParkingProportionMolecule { get; set; }
        ///// <summary>
        ///// 车位分母
        ///// </summary>
        //public decimal? ParkingProportionDenominator { get; set; }
    }
}
