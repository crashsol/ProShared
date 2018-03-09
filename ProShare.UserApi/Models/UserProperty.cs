namespace ProShare.UserApi.Models
{


    /// <summary>
    /// 用户属性扩展
    /// </summary>
    public class UserProperty
    {
        /// <summary>
        /// 关联用户ID
        /// </summary>
        public int AppUserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }
    }
}