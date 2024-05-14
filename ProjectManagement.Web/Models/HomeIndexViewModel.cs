namespace ProjectManagement.Web.Models
{
    public class HomeIndexViewModel
    {
        public required string MinDate { get; set; }
        public required string MaxDate { get; set; }
        public required List<ProjectViewModel> Projects { get; set; }
    }
}
