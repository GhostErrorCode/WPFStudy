using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Models
{
    // 保存当前登录的用户信息
    public class UserInfo
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
