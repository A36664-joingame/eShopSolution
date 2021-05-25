using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Common
{
    public class PagedResult<T> : PageResultBase
    {
        public List<T> Items { set; get; }
    }
}