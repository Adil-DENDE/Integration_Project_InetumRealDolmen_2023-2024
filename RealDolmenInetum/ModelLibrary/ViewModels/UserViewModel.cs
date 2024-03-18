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
        public int? NiveauId { get; set; }// naam
        //public int? Manager_Id { get; set; }// naam
        public string Mail { get; set; }
        public string Username { get; set; }
        //public string Last_Name { get; set; }
        //public DateTime Birthdate { get; set; }
        public DateTime StartBench { get; set; }// nadenken
        public DateTime? EndBench { get; set; }
    }
}
