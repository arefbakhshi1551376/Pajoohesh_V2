using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Film;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Subject;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.Subject;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Area("WebApi")]
    [Authorize]
    public class SubjectApiController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Subjects modelList;

        public SubjectApiController(UserManager<Model.Models.Main.User.User> userManager,
            SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment env,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List(SendSubjectList sendSubjectList)
        {
            modelList = new Subjects(_unitOfWork, env);
            var hostUrl = Request.Host.Value.ToString();
            var currentSubjectsList = await modelList.SubjectListVms(sendSubjectList.PageNumber, hostUrl);
            ReceiveSubjectList receiveSubjectList = new ReceiveSubjectList()
            {
                CurrentPageNumber = currentSubjectsList.CurrentPageNumber,
                NumberOfPages = currentSubjectsList.NumberOfPages,
                SubjectListBaseVms = currentSubjectsList.SubjectListBaseVms
            };
            return Json(receiveSubjectList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SendSubjectAdd sendSubjectAdd)
        {
            ReceiveSubjectAdd receiveSubjectAdd = new ReceiveSubjectAdd();
            try
            {
                SubjectActions subjectActions =
                    new SubjectActions(_unitOfWork, _userManager, env);
                var name = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(name);
                await subjectActions.SubjectAddAction(sendSubjectAdd, user);
                receiveSubjectAdd.IsAdded = true;
            }
            catch (Exception e)
            {
                receiveSubjectAdd.IsAdded = false;
            }

            return Json(receiveSubjectAdd);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(SendSubjectEditGet sendSubjectEditGet)
        {
            modelList = new Subjects(_unitOfWork, env);
            var hostUrl = Request.Host.Value.ToString();
            var subjectEditVm = await modelList.SubjectEditVms(sendSubjectEditGet.Id, hostUrl);
            ReceiveSubjectEditGet receiveSubjectEditGet = new ReceiveSubjectEditGet()
            {
                Id = subjectEditVm.Id,
                State = subjectEditVm.State,
                Title = subjectEditVm.Title,
                CreateDate = subjectEditVm.CreateDate,
                CreatorId = subjectEditVm.CreatorId,
                ImageUrl = subjectEditVm.ImageUrl,
                LastModifierId = subjectEditVm.LastModifierId,
                LastModifyDate = subjectEditVm.LastModifyDate,
                NewImageUrl = subjectEditVm.NewImageUrl,
                OriginalCreateDate = subjectEditVm.OriginalCreateDate,
                OriginalLastModifyDate = subjectEditVm.OriginalLastModifyDate,
            };
            return Json(receiveSubjectEditGet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SendSubjectEditPost sendSubjectEditPost)
        {
            ReceiveSubjectEditPost receiveSubjectEditPost = new ReceiveSubjectEditPost();
            try
            {
                SubjectActions subjectActions =
                    new SubjectActions(_unitOfWork, _userManager, env);
                var name = User.Identity.Name;
                var lastModifier = await _userManager.FindByNameAsync(name);
                await subjectActions.SubjectEditAction(sendSubjectEditPost, lastModifier);
                receiveSubjectEditPost.IsEdited = true;
            }
            catch (Exception e)
            {
                receiveSubjectEditPost.IsEdited = false;
            }

            return Json(receiveSubjectEditPost);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(SendSubjectDeleteGet sendSubjectDeleteGet)
        {
            modelList = new Subjects(_unitOfWork, env);
            ReceiveSubjectDeleteGet receiveSubjectDeleteGet = new ReceiveSubjectDeleteGet();
            var hostUrl = Request.Host.Value.ToString();
            var subjectDeleteVm = await modelList.SubjectDeleteApiVms(sendSubjectDeleteGet.Id, hostUrl);
            var currentSubject = await _unitOfWork.SubjectRepository.FindByTitleAsync("عمومی");
            if (subjectDeleteVm.Id == currentSubject.Id)
            {
                receiveSubjectDeleteGet.Message = "این موضوع قابل حذف نیست!";
            }

            receiveSubjectDeleteGet.Id = subjectDeleteVm.Id;
            receiveSubjectDeleteGet.Title = subjectDeleteVm.Title;
            receiveSubjectDeleteGet.ImageUrl = subjectDeleteVm.ImageUrl;
            receiveSubjectDeleteGet.NumberOfFilms = subjectDeleteVm.NumberOfFilms;

            return Json(receiveSubjectDeleteGet);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SendSubjectDeletePost sendSubjectDeletePost)
        {
            ReceiveSubjectDeletePost receiveSubjectDeletePost = new ReceiveFilmDeletePost();
            try
            {
                SubjectActions subjectActions =
                    new SubjectActions(_unitOfWork, _userManager, env);
                await subjectActions.SubjectDeleteAction(sendSubjectDeletePost);
                receiveSubjectDeletePost.IsDeleted = true;
            }
            catch (Exception e)
            {
                receiveSubjectDeletePost.IsDeleted = false;
            }
            return Json(receiveSubjectDeletePost);
        }

        [HttpGet]
        public async Task<IActionResult> Details(SendSubjectDetails sendSubjectDetails)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var finalFilms = await films.FilmListBySubjectApiVms(sendSubjectDetails.Id, true, sendSubjectDetails.PageNumber, Request);
            ReceiveSubjectDetails receiveSubjectDetails = new ReceiveSubjectDetails()
            {
                ImageUrl = finalFilms.ImageUrl,
                SubjectId = finalFilms.SubjectId,
                SubjectTitle = finalFilms.SubjectTitle,
                CurrentPageNumber = finalFilms.CurrentPageNumber,
                NumberOfPages = finalFilms.NumberOfPages,
                FilmListBaseVms = finalFilms.FilmListBaseVms,
            };
            return Json(receiveSubjectDetails);
        }
    }
}