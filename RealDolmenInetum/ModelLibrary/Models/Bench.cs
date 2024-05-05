using ModelLibrary.Dto;
using System.Numerics;

namespace ModelLibrary.Models
{
    public class Bench
    {
        
        public Bench()
        {
            
        }

        public Bench(int userId, DateTime startBench)
        {
            User_id = userId;
            Start_bench = startBench;
        }
        public int Id { get; set; }
        public int User_id { get; set; }
        public DateTime Start_bench { get; set; }
        public DateTime? End_bench { get; set; }
        public int? Occupation_id { get; set; }
        public bool? IsCurrentBenchManager { get; set; }

    }
}
