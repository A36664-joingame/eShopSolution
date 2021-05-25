using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required")
                .MaximumLength(200).WithMessage("First Name can not over 200 charartor");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("last Name is required")
                .MaximumLength(200).WithMessage("Last Name can not over 200 charracters");

            RuleFor(x => x.DOB).GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("Birthday cannot greatr than 100"); // không được trên 100 tuổi

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")//regex cho Email pahir đúng dạng này mới được
                .WithMessage("Email format no match");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("phone number is required");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("User Name is Required"); // không được để User Name Rỗng ,notEmpty có cả notNull còn notNull ko có empty

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is Required").MinimumLength(8).WithMessage("Password must be at least eight characters long, have special characters and contain at least one uppercase letter");

            // confirm pasword
            RuleFor(x => x).Custom((request, context) => // nó như kiểu sự kiện ý một hàm nặc danh với hai tham số
            {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure("Confirm password is not match");
                }
            });
        }
    }
}