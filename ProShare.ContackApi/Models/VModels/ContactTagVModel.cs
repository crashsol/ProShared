using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi.Models.VModels
{
    /// <summary>
    /// 给好友打标签
    /// </summary>
    public class ContactTagVModel
    {

        public ContactTagVModel()
        {
            Tags = new List<string>();
        }
        /// <summary>
        /// 好友ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 标签信息
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
