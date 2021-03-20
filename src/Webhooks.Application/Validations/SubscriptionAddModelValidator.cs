using FluentValidation;
using Webhooks.Application.Models;
using Webhooks.Application.Services;
using System;
using System.Linq;

namespace Webhooks.Application.Validations
{
    public class SubscriptionAddModelValidator : AbstractValidator<SubscriptionAddModel>
    {
        public SubscriptionAddModelValidator(IEventsService eventService)
        {
            RuleFor(m => m.Id).Must(value =>
            {
                return value != Guid.Empty;
            }).WithMessage($"Invalid subscription id");

            RuleFor(m => m.Event).MustAsync(async (value, cancellation) =>
            {
                var eventNames = (await eventService.GetEvents()).Select(t=>t.FullName);
                return eventNames.Contains(value);
            }).WithMessage($"Invalid event name");

            //RuleFor(m => m.Endpoint).Must(value =>
            //{
            //    try
            //    {
            //        new Uri(value);
            //    }
            //    catch
            //    {
            //        return false;
            //    }

            //    return true;
            //}).WithMessage($"Invalid endpoint Url");

        }
    }
}
