using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechChallengeFIAP.Models;
using TechChallengeFIAP.Models;

namespace TechChallengeFIAP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _repository;

        public ContactsController(IContactRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _repository.GetAllContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _repository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _repository.AddContactAsync(contact);
            return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
        {
            if (id != contact.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }
            await _repository.UpdateContactAsync(contact);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _repository.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            await _repository.DeleteContactAsync(id);
            return NoContent();
        }
    }
}
