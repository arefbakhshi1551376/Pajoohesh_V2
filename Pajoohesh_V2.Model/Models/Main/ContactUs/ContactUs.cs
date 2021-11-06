using System;
using System.Collections.Generic;
using System.Text;

namespace Pajoohesh_V2.Model.Models.Main.ContactUs
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderFamily { get; set; }
        public string SenderEmail { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsReplied { get; set; }
        public string ReplySubject { get; set; }
        public string ReplyText { get; set; }
        public User.User Replier { get; set; }
        public DateTime? ReplyDate { get; set; }
    }
}
