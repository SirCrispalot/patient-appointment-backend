using FluentValidation;
using Panda.ClientModel;

namespace Panda.WebApi.Validators
{
    public class MissedAppointmentReportRequestValidator : AbstractValidator<MissedAppointmentReportRequest>
    {
        public MissedAppointmentReportRequestValidator()
        {
            RuleFor(request => request.ReportTo).Must(BeInThePast).WithMessage("Report to date must be in the past");
            RuleFor(request => request.ReportFrom).Must(BeInThePast).WithMessage("Report from date must be in the past");
            RuleFor(request => request.ReportTo).GreaterThan(request => request.ReportFrom);
        }

        private bool BeInThePast(DateTime dateTime)
        {
            return dateTime < DateTime.UtcNow;
        }
    }
}
