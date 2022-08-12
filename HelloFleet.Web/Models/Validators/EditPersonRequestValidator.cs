namespace HelloFleet.Models.Validators;

using FluentValidation;

public class EditPersonRequestValidator : AbstractValidator<EditPersonRequest> {
    public EditPersonRequestValidator()
    {
        RuleFor(m => m.Name).NotEmpty();
    }
}