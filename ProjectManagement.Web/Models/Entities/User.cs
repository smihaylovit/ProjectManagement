using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Web.Models.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<TimeLog> TimeLogs { get; } = [];
    }
}
