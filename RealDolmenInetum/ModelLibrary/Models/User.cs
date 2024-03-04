namespace ModelLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public int Niveau_Id { get; set; }
        public int? Manager_Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? Deleted_planned_date { get; set; }
    }
}
