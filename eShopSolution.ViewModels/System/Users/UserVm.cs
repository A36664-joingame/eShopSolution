using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserVm
    {
        // muốn show ra gì thì ch nó vào viewModel  điều đó là hiển nhiên product chúng ta cũng làm như thees
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Tên")]
        public string FirstName { get; set; }

        [Display(Name = "Họ Tên")]
        public string LastName { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tên Tài Khoản")]
        public string UserName { get; set; }

        [Display(Name = "Địa Chỉ Email")]
        public string Email { get; set; }

        [Display(Name = "Ngày Sinh")]
        public DateTime Dob { get; set; }

        public IList<string> Roles { get; set; }
    }
}