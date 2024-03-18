using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int? niveauId { get; set; }// naam
        //public int? Manager_Id { get; set; }// naam
        public string mail { get; set; }
        public string username { get; set; }
        //public string Last_Name { get; set; }
        //public DateTime Birthdate { get; set; }
        public DateTime startBench { get; set; }// nadenken
        public DateTime? endBench { get; set; }
    }
}
