using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    // giờ tạo thực thế User khi ta kế thừa từ IdentityUser thì nó đã viết sẵn những thứu quan trong và phổ biến của User giờ ta chỉ cần muốn thêm gì vào thì thêm vào class mới này
    public class AppUser:IdentityUser<Guid> // để khóa chính là Guild tức là kiểu duy nhất cho toàn hệ thống
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public List<Cart> Carts { get; set; }
        public List<Order> Orders { get; set; }  // một thăng user có nhiều order

        public List<TranSaction> TranSactions { get; set; } // một thằng user có nhiều giao dịch
    }
}
