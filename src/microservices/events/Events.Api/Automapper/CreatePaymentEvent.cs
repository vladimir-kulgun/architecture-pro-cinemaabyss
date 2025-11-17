using AutoMapper;
using Events.Api.CQRS.Events.CreatePayment.Commands;
using Events.Api.Models;

namespace Events.Api.Automapper
{
    public class CreatePaymentEvent : Profile
    {
        public CreatePaymentEvent()
        {
            CreateMap<PaymentEvent, CreatePaymentCommand>()
                .ReverseMap();
        }
    }
}
