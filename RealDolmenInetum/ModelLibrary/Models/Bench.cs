namespace RealDolmenAPI.Models
{
    public class Bench
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartBench { get; set; }
        public DateTime EndBench { get; set; }

         // NOT SURE
        public int Occupation { get; set; }
    }
}
