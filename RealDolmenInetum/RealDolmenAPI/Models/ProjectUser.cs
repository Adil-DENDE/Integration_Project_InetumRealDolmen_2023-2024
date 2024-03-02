namespace RealDolmenAPI.Models
{
    public class ProjectUser

    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDateForUser { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
