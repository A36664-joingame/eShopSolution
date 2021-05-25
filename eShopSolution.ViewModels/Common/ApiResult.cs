using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class ApiResult<T>
    {
        // thông báo thêm cho người dùng khi điền không phù hợp

        public bool IsSuccessed { get; set; }

        public string Message { get; set; }

        public T ResultObj { get; set; }  // nếu trường hợp true thì trả về một Object mà chuyền vào kiểu của nó theo Gennerric
    }
}