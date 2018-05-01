using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proshare.RecommendApi.Models.Dtos
{
    public enum EnumRecommendType:int 
    {
        /// <summary>
        /// 平台推荐
        /// </summary>
        PlatForm=1,

        /// <summary>
        /// 一度好友
        /// </summary>
        Friend=2,


        /// <summary>
        /// 二度好友
        /// </summary>
        Foaf=3
    }
}
