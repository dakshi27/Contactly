using Contactly.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Contactly.Models;
using Contactly.Data;
using Contactly.Services;
using Contactly.Models.Domain;

namespace Contactly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactlyDbContext dbContext;
        private readonly EmailService emailService;

        public ContactsController(ContactlyDbContext dbContext, EmailService emailService)
        {
            this.dbContext = dbContext;
            this.emailService = emailService;
        }
        [HttpGet]
        public IActionResult GetAllContacts()
        {
            var contacts = dbContext.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactRequestDTO request)
        {
            var domainModelContact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Favorite = request.Favorite,
            };

            dbContext.Contacts.Add(domainModelContact);
            dbContext.SaveChanges();

            // Send welcome email
            await emailService.SendWelcomeEmail(domainModelContact.Email, domainModelContact.Name);

            return Ok(domainModelContact);
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteContact(Guid id)
        {
            var contact=dbContext.Contacts.Find(id);

            if(contact is not null)
            {
                dbContext.Contacts.Remove(contact);
                dbContext.SaveChanges();
            }

            return Ok(); 
        }
    }
}
