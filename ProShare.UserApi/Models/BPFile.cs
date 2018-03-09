using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.UserApi.Models
{
    public class BPFile
    {
        /// <summary>
        /// BP Id       
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int AppUserId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string  FileName { get; set; }

        /// <summary>
        /// 上传的源文件地址
        /// </summary>
        public string OriginFilePash { get; set; }

        /// <summary>
        /// 格式转换后的文件地址
        /// </summary>
        public string FromatFilePash { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }



    }
}
