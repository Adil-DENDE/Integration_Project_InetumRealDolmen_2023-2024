using Microsoft.AspNetCore.Identity;

namespace ModelLibrary.Models
{
    public class User : IdentityUser<int>
    {
        public int? Niveau_Id { get; set; }
        public int? Manager_Id { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? Deleted_planned_date { get; set; }
    }
}
