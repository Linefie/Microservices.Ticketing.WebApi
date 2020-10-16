using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace Ticketing.Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        //linje 17-21 Here we use the Bus variable we configured earlier in the Startup file. 
        //Finally we will inject the IBus object to the constructor of the Ticket Controller.
        private readonly IBus _bus;
        public TicketController(IBus bus)
        {
            _bus = bus;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            if (ticket != null)
            {
                //We add the current datetime to the received ticket object.
                ticket.BookedOn = DateTime.Now;
                // We will name our Queue as ticketQueue.
                //Now, let’s create a new URL ‘rabbitmq://localhost/ticketQueue’. If ticketQueue does not exist, RabbitMQ creates one for us.
                Uri uri = new Uri("rabbitmq://localhost/ticketQueue");
                //Gets an endpoint to which we can send the shared model object.
                var endPoint = await _bus.GetSendEndpoint(uri);
                //Push the message to the queue.
                await endPoint.Send(ticket);
                return Ok();
            }
            return BadRequest();
        }
    }
}
