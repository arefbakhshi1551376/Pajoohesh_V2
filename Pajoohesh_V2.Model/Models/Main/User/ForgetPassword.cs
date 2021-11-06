using System;
using System.Collections.Generic;
using System.Text;

namespace Pajoohesh_V2.Model.Models.Main.User
{
    public class ForgetPassword
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public User User { get; set; }
        public bool IsChangePasswordFinished { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
