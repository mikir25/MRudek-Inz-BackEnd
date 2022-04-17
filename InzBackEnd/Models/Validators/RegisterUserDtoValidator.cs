using FluentValidation;
using InzBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(PortalDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty();

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email.Equals(value));
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });

            RuleFor(x => x.Name)
                .Custom((value, context) =>
                {
                    var nameInUse = dbContext.Users.Any(u => u.Name.Equals(value));
                    if (nameInUse)
                    {
                        context.AddFailure("Nick", "That nick is taken");
                    }
                });
        }
    }

    public class EditPasswordValidator : AbstractValidator<EditPassword>
    {
        public EditPasswordValidator(PortalDbContext dbContext)
        {
            RuleFor(x => x.PasswordLast)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty();

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
        }
    }

    public class EditUserValidator : AbstractValidator<EditUser>
    {
        public EditUserValidator(PortalDbContext dbContext)
        {

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email.Equals(value) );
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });

            RuleFor(x => x.Name)
                .Custom((value, context) =>
                {
                    var nameInUse = dbContext.Users.Any(u => u.Name.Equals(value));
                    if (nameInUse)
                    {
                        context.AddFailure("Nick", "That nick is taken");
                    }
                });
        }
    }
}
