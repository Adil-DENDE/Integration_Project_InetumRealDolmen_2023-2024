namespace ModelLibrary.Models
{
    public class Bench
    {
        public int? Id { get; set; }
        public int user_id { get; set; }
        public DateTime Start_bench { get; set; }
        public DateTime? End_bench { get; set; }
        public int? Occupation_id { get; set; }

        static public Bench UserbenchToBench(UserBench userBench)
        {
            return new Bench {
                Id = null,
                user_id = userBench.UserId,
                
                Start_bench = userBench.StartBench.HasValue ? userBench.StartBench.Value : DateTime.Now, 
                End_bench = userBench.EndBench,
                Occupation_id = null // Aangezien de gebruiker net op de bank wordt geplaatst
            };
        }
    }
}
