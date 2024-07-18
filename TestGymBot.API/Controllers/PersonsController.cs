using Microsoft.AspNetCore.Mvc;
using TestGymBot.API.Request;
using TestGymBot.API.Response;
using TestGymBot.Domain;
using TestGymBot.Domain.Abstractions.Services;

namespace TestGymBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonsService _personsService;

        public PersonsController(IPersonsService personsService)
        {
            _personsService = personsService;
        }
        [HttpPost("Create/")]
        public async Task<ActionResult<Guid>> CreatePerson([FromBody]PersonRequest request)
        {
            var person = Person.Create(
                Guid.NewGuid(),
                request.UserId,
                request.ChatId,
                request.UserName,
                request.FirstName,
                request.LastName);

            var personId = await _personsService.CreatePerson(person);

            return Ok(personId);
        }
        [HttpGet("Get/{id:guid}")]
        public async Task<ActionResult<PersonResponse>> GetPerson(Guid id)
        {
            var person = await _personsService.GetPerson(id);

            return new PersonResponse(person.Id, person.UserId, person.ChatId, person.UserName, person.FirstName, person.LastName);
        }

        [HttpGet("GetAll/")]
        public async Task<ActionResult<List<PersonResponse>>> GetAllPerson()
        {
            var persons = await _personsService.GetAllPerson();

            var response = persons.Select(p => new PersonResponse(p.Id, p.UserId, p.ChatId, p.UserName, p.FirstName, p.LastName)).ToList();
            return response;
        }

        [HttpDelete("Delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeletePerson(Guid id)
        {
            return await _personsService.DeletePerson(id);
        }

    }
}
