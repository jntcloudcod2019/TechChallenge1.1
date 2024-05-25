using System.Collections.Generic;
using System.Threading.Tasks;
using TechChallengeFIAP.Models;

namespace TechChallengeFIAP.Models
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<Contact> GetContactByIdAsync(int id);
        Task AddContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task DeleteContactAsync(int id);
    }
}
