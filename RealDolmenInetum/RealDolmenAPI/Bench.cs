namespace RealDolmenAPI
{
    public class Bench
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartBench { get; set; }
        public DateTime EndBench { get; set; }
        public string Occupation { get; set; }
    }
}
