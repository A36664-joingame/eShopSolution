using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    //thêm AddFluentValidation vào startup cảu Api để sử dụng Vlidation tải từ nuget FluentValidation.AspNetCore tahwngf này cài cả ở Api và ViewModel nhé

    //https://docs.fluentvalidation.net/en/latest/aspnet.html
    public class LoginRequestValidator : AbstractValidator<LoginRequest>// ta validate cho LoginRequest
    {
        // gọi là thêm hạn chế cho phương thức Required(cần thiết) nhớ cái này khác với Configuration nhe

        // Validator dùng cho các Request  còn Confguration sử dụng cho table và dữ liệu  trên database

        public LoginRequestValidator()
        {
            // xem trong https://docs.fluentvalidation.net/en/latest/built-in-validators.html
            RuleFor(x => x.UserName).NotEmpty().WithMessage("User Name is Required"); // không được để User Name Rỗng ,notEmpty có cả notNull còn notNull ko có empty
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is Required").MinimumLength(8).WithMessage("Password must be at least eight characters long, have special characters and contain at least one uppercase letter");
        }
    }
}