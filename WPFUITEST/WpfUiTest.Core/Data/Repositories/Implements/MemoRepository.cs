using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.DbContexts;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Services.Interfaces;

namespace WpfUiTest.Core.Data.Repositories.Implements
{
    public class MemoRepository : Repository<Memo>, IMemoRepository
    {
        // 构造函数
        // 调用基类构造函数：base(context)将context传递给基类
        public MemoRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            // 可以在这里初始化产品仓储特有的依赖项
            // 例如：缓存服务、日志服务等
        }
    }

}
