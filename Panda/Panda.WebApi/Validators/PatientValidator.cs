using FluentValidation;
using Panda.ClientModel;

namespace Panda.WebApi.Validators
{
    public class PatientValidator : AbstractValidator<Patient>
    {
        private NhsNumberValidator _nhsNumberValidator;

        public PatientValidator(NhsNumberValidator nhsNumberValidator)
        {
            _nhsNumberValidator = nhsNumberValidator;

            RuleFor(patient => patient.NhsNumber).NotEmpty();
            RuleFor(patient => patient.NhsNumber).Must(BeAValidNhsNumber).WithMessage("NHS number is not valid");
            
            //TODO: This is just an example of validation rules.  More clearly need to be implemented.
        }

        private bool BeAValidNhsNumber(string nhsNumber)
        {
            return _nhsNumberValidator.IsValid(nhsNumber);
        }
    }
}
