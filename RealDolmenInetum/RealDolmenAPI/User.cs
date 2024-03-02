namespace RealDolmenAPI
{
    public class User
    {
        public int Id { get; set; }
        public int NiveauId { get; set; }
        public int? ManagerId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime StartDate { get; set; }
    }
}
