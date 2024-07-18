using Microsoft.AspNetCore.Mvc;
using TestGymBot.API.Request;
using TestGymBot.API.Response;
using TestGymBot.Domain;
using TestGymBot.Domain.Abstractions.Services;

namespace TestGymBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController : ControllerBase
    {

        private readonly IRecordsService _recordService;

        public RecordController(IRecordsService recordService)
        {
            _recordService = recordService;
        }
        [HttpPost("Create/")]
        public async Task<ActionResult<Guid>> CreateRecord([FromBody] RecordRequest request)
        {
            var record = Record.Create(Guid.NewGuid(),
                request.DayName,
                request.Time);

            var recordId = await _recordService.CreateRecord(record);

            return Ok(recordId);
        }
        [HttpGet("Get/{id:guid}")]
        public async Task<ActionResult<RecordResponse>> GetPerson(Guid id)
        {
            var record = await _recordService.GetRecord(id);
            return new RecordResponse(record.Id, record.DayName, record.Time);
        }

        [HttpGet("GetAll/")]
        public async Task<ActionResult<List<RecordResponse>>> GetAllRecords()
        {
            var records = await _recordService.GetAllRecords();

            var response = records.Select(r => new RecordResponse(r.Id, r.DayName, r.Time)).ToList();
            return response;
        }

        [HttpDelete("Delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeletePerson(Guid id)
        {
            return await _recordService.DeleteRecord(id);
        }

    }
}

