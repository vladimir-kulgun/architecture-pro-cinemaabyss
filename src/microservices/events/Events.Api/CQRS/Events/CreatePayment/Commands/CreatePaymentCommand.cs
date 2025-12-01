using System;
using MediatR;

namespace Events.Api.CQRS.Events.CreatePayment.Commands
{
    public class CreatePaymentCommand : IRequest<CreateEventResponse>
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string MethodType { get; set; }
    }
}
