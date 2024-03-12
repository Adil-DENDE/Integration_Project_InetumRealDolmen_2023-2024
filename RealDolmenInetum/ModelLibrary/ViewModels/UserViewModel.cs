using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.ViewModels
{
    public class UserViewModel
    {
        public int? Niveau_Id { get; set; }// naam
        public int? Manager_Id { get; set; }// naam
        public string Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime Start_Date { get; set; }// nadenken
    }
}
