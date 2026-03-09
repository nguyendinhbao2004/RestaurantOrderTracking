using Application.Feature.WorkSchedules.Commands.Create;
using Application.Feature.WorkSchedules.Commands.Update;
using Application.Feature.WorkSchedules.Commands.Delete;
using Application.Feature.WorkSchedules.Commands.CheckIn;
using Application.Feature.WorkSchedules.Commands.CheckOut;
using Application.Feature.WorkSchedules.Queries.GetAllWorkSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkSchedules(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllWorkScheduleQueries(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWorkSchedule([FromBody] UpdateWorkScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkSchedule(Guid id)
        {
            var command = new DeleteWorkScheduleCommand(id);
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        [HttpPut("CheckIn/{id}")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            var command = new CheckInWorkScheduleCommand(id);
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        [HttpPut("CheckOut/{id}")]
        public async Task<IActionResult> CheckOut(Guid id)
        {
            var command = new CheckOutWorkScheduleCommand(id);
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }
    }
}
