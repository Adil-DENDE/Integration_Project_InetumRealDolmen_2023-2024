using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.ViewModels
{

public class UserDetailsViewModel
    {
    public int UserId { get; set; }
    public int Manager_Id { get; set; }
    public int Niveau_id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public int BenchId { get; set; }
    //public DateTime? EndBench { get; set; }
    public DateTime StartBench { get; set; }
    public int? OccupationId { get; set; }
    }
}
