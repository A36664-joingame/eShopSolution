using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    // ta muốn cho thằng user đăng kí cái gì thì cho vào đây
    public class RegisterRequest
    {
        [Display(Name = "Tên")]
        public string FirstName { get; set; }

        [Display(Name = "Họ Tên")]
        public string LastName { get; set; }

        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Display(Name = "Địa Chỉ Email")]
        public string Email { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tên Tài Khoản")]
        public string UserName { get; set; }

        [Display(Name = "Mật Khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhập Lại Mật Khẩu")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }  // nhập lại password
    }
}