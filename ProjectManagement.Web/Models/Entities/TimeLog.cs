namespace ProjectManagement.Web.Models.Entities
{
    public class TimeLog
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int ProjectId { get; set; }
        public required DateOnly Date { get; set; }
        public required float Hours { get; set; }
    }
}
