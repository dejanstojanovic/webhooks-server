using AutoMapper;
using Core.Domain.Events;
using Webhooks.Application.Models;
using Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Webhooks.Common.Helpers;

namespace Webhooks.Application.Mappings
{
    public class SubscriptionMapping : Profile
    {
        public SubscriptionMapping()
        {
            CreateMap<Subscription, SubscriptionModel>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => DomainEventsHelper.GetDomainEventTypes().SingleOrDefault(t => t.FullName == src.Event)))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event))
                .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers.ToDictionary(h => h.Key, h => h.Value)));

            CreateMap<SubscriptionAddModel, Subscription>()
                .ForMember(dest => dest.Endpoint, opt => opt.MapFrom(src => src.Endpoint))
                .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers.Select(h => new SubscriptionHeader()
                {
                    SubscriptionId = src.Id,
                    Key = h.Key,
                    Value = h.Value
                })));
        }
    }
}
