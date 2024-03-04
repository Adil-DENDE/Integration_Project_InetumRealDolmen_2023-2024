namespace ModelLibrary.Models
{
    public class Bench
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public DateTime Start_bench { get; set; }
        public DateTime? End_bench { get; set; }
        public int? Occupation_id { get; set; }
    }
}
