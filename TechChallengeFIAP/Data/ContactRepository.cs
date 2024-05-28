using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechChallengeFIAP.Models;

namespace TechChallengeFIAP.Models
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            var contacts = new List<Contact>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM Contacts", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            contacts.Add(new Contact
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Phone = reader.GetString("Phone"),
                                Email = reader.GetString("Email"),
                                Ddd = reader.GetInt32("Ddd")
                            });
                        }
                    }
                }
            }

            return contacts;
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            Contact contact = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM Contacts WHERE Id = @Id", connection))
                {
                    command.Parameters.Add(new MySqlParameter("@Id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            contact = new Contact
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Phone = reader.GetString("Phone"),
                                Email = reader.GetString("Email"),
                                Ddd = reader.GetInt32("Ddd")
                            };
                        }
                    }
                }
            }

            return contact;
        }

        public async Task AddContactAsync(Contact contact)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("INSERT INTO Contacts (Name, Phone, Email, Ddd) VALUES (@Name, @Phone, @Email, @Ddd)", connection))
                {
                    command.Parameters.Add(new MySqlParameter("@Name", contact.Name));
                    command.Parameters.Add(new MySqlParameter("@Phone", contact.Phone));
                    command.Parameters.Add(new MySqlParameter("@Email", contact.Email));
                    command.Parameters.Add(new MySqlParameter("@Ddd", contact.Ddd));

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("UPDATE Contacts SET Name = @Name, Phone = @Phone, Email = @Email, Ddd = @Ddd WHERE Id = @Id", connection))
                {
                    command.Parameters.Add(new MySqlParameter("@Name", contact.Name));
                    command.Parameters.Add(new MySqlParameter("@Phone", contact.Phone));
                    command.Parameters.Add(new MySqlParameter("@Email", contact.Email));
                    command.Parameters.Add(new MySqlParameter("@Ddd", contact.Ddd));
                    command.Parameters.Add(new MySqlParameter("@Id", contact.Id));

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteContactAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("DELETE FROM Contacts WHERE Id = @Id", connection))
                {
                    command.Parameters.Add(new MySqlParameter("@Id", id));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
