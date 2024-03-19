namespace ModelLibrary.Models
{
    public class Project_User

    {
        public int User_Id { get; set; }
        public int Project_Id { get; set; }
        public DateTime Start_date_for_user { get; set; }
        public DateTime? End_date { get; set; }

    }
}
