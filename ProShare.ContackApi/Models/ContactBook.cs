using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProShare.ContactApi.Models
{
    /// <summary>
    /// 通讯录
    /// </summary>
    public class ContactBook
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        public List<Contact> Contacts { get; set; }
    }
}
