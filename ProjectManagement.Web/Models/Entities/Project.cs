using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Web.Models.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<TimeLog> TimeLogs { get; } = [];
    }
}
