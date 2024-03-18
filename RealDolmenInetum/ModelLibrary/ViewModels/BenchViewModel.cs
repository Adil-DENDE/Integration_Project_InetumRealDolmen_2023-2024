using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.ViewModels
{
    public class BenchViewModel
    {
        public int BenchId { get; set; }
        public DateTime StartBench { get; set; }
        public DateTime? EndBench { get; set; }
        public int UserId { get; set; }
        public int? OccupationId { get; set; }
    }
}
