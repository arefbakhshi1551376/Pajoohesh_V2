using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Utility
{
    public class DefaultBuilder:Controller
    {
        private IUnitOfWork _unitOfWork;

        public DefaultBuilder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Tag> BuildDefaultTag(User user)
        {
            Tag finalTag = null;
            var defaultTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");
            if (defaultTag == null)
            {
                Tag tag = new Tag()
                {
                    CreateDate = DateTime.UtcNow,
                    Creator = user,
                    LastModifier = null,
                    LastModifyDate = null,
                    State = State.Enable,
                    Title = "عمومی"
                };
                await _unitOfWork.TagRepository.AddAsync(tag);
                await _unitOfWork.SaveAsync();
                finalTag = tag;
            }
            else
            {
                finalTag = defaultTag;
            }

            return finalTag;
        }

        public async Task<Subject> BuildDefaultSubject(User user)
        {
            Subject finalSubject = null;
            var defaultSubject = await _unitOfWork.SubjectRepository.FindByTitleAsync("عمومی");
            if (defaultSubject == null)
            {
                Subject subject = new Subject()
                {
                    CreateDate = DateTime.UtcNow,
                    Creator = user,
                    LastModifier = null,
                    LastModifyDate = null,
                    State = State.Enable,
                    Title = "عمومی",
                    ImageUrl = "eeeeeeeeeeeeeeee.jpg",
                };
                await _unitOfWork.SubjectRepository.AddAsync(subject);
                await _unitOfWork.SaveAsync();
                finalSubject = subject;
            }
            else
            {
                finalSubject = defaultSubject;
            }

            return finalSubject;
        }
    }
}
