using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiCore;
using ElasticseatchApi.Models;
using ElasticseatchApi.Dto.Request;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Cors;

namespace ElasticseatchApi.Controllers
{
    /// <summary>
    /// 查询所有楼盘控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public readonly BuildShopDbContext _dbContext;
     
    
        public ValuesController(BuildShopDbContext dbcontext)
        {
            this._dbContext = dbcontext;            
        }


        [HttpPost("list")]        

        public async Task<PagingResponseMessage<BuildingBaseInfo>> Get([FromBody]PageCondition page)
        {
            var cancellationToken =new  CancellationToken();
           var resposne = new PagingResponseMessage<BuildingBaseInfo> { };
            var all = _dbContext.buildingBaseInfos.AsNoTracking().Where(y => !y.IsDeleted);            
            resposne.TotalCount = await all.CountAsync(cancellationToken);
            var list = await all.Skip(page.PageIndex * page.PageSize).Take(page.PageSize).ToListAsync(cancellationToken);
            resposne.Extension = list;
            resposne.PageIndex = page.PageIndex;
            resposne.PageSize = page.PageSize;
            return resposne;
        }

       
    }
}
