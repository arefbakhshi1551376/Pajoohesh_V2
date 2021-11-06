using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Utility
{
    public class NotifierOfNewMovies
    {
        private IUserRepository _userRepository;

        public NotifierOfNewMovies(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Inform(string filmName, string filmWatchUrl)
        {
            string subject = "فیلم جدید آپلود شد!";
            string message = $"فیلم جدید با عنوان {filmName} در سایت آپلود شد.\n" +
                             $"با مراجعه به آدرس زیر می توانید این فیلم را مشاهده نمایید:\n" +
                             $"{filmWatchUrl}";

            /*var usersInNewsletter = (await _userRepository.GetAllAsync()).Where(u=>u.IsHeMemberOfTheNewsletter==true).Select(u=>u.Email);

            SendMessages sendMessages = new SendMessages();
            foreach (var email in usersInNewsletter)
            {
                await sendMessages.SendEmailAsync(email, subject, message);
            }*/
        }
    }
}
