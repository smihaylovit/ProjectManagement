namespace ProjectManagement.Web.Data.Initialization
{
    public class TimeLogData
    {
        public required int UserId { get; set; }
        public required int ProjectId { get; set; }
        public required DateOnly Date { get; set; }
        public required float Hours { get; set; }
    }
}
