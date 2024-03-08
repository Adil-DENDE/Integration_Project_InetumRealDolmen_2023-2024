using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Models
{
    public class UserBench
    {
        public int UserId { get; set; }
        public int BenchId { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public int NiveauId { get; set; }
        public DateTime? EndBench { get; set; }
        public DateTime? StartBench { get; set;}
    }
}
