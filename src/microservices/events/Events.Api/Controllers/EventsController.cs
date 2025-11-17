using System.Threading.Tasks;
using AutoMapper;
using Events.Api.CQRS.Events.CreateMovie.Commands;
using Events.Api.CQRS.Events.CreatePayment.Commands;
using Events.Api.CQRS.Events.CreateUser.Commands;
using Events.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Events.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EventsController(IMediator mediator, IMapper mapper, ILogger<EventsController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("movie")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieEvent body)
        {
            var command = _mapper.Map<CreateMovieCommand>(body);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] UserEvent body)
        {
            var command = _mapper.Map<CreateUserCommand>(body);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentEvent body)
        {
            var command = _mapper.Map<CreatePaymentCommand>(body);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }
    }
}
