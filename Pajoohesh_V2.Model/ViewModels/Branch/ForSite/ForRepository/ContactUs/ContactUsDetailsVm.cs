namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs
{
    public class ContactUsDetailsVm
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string SenderName { get; set; }
        public string SenderFamily { get; set; }
        public string SenderEmail { get; set; }
        public string SendDate { get; set; }
        public bool IsReplied { get; set; }
        public string ReplySubject { get; set; }
        public string ReplyText { get; set; }
        public string Replier { get; set; }
        public string ReplyDate { get; set; }
    }
}
