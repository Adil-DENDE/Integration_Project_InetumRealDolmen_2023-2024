using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Models
{
    public class OccupationHistory
    {
        public int Id { get; set; }
        public int Bench_id { get; set; }
        public int Occupation_id { get; set; }
        public DateTime Start_occupationdate { get; set; }
        public DateTime? End_occupationdate { get; set; }
    }
}
