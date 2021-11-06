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
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Area("WebApi")]
    [Authorize]
    public class TagApiController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Tags modelList;

        public TagApiController(UserManager<Model.Models.Main.User.User> userManager,
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
        public async Task<IActionResult> List(SendTagList sendTagList)
        {
            modelList = new Tags(_unitOfWork);
            var tagListVm = await modelList.TagListVms(sendTagList.PageNumber);
            ReceiveTagList receiveTagList = new ReceiveTagList()
            {
                CurrentPageNumber = tagListVm.CurrentPageNumber,
                NumberOfPages = tagListVm.NumberOfPages,
                TagListBaseVms = tagListVm.TagListBaseVms
            };
            return Json(receiveTagList);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SendTagAdd sendTagAdd)
        {
            ReceiveTagAdd receiveTagAdd = new ReceiveTagAdd();
            try
            {
                TagActions tagActions =
                    new TagActions(_unitOfWork, _userManager);
                var name = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(name);
                await tagActions.TagAddAction(sendTagAdd, user);
                receiveTagAdd.IsAdded = true;
            }
            catch (Exception e)
            {
                receiveTagAdd.IsAdded = false;
            }

            return Json(receiveTagAdd);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(SendTagEditGet sendTagEditGet)
        {
            modelList = new Tags(_unitOfWork);
            var tagEditVm = await modelList.TagEditVms(sendTagEditGet.Id);
            ReceiveTagEditGet receiveTagEditGet = new ReceiveTagEditGet()
            {
                Id = tagEditVm.Id,
                State = tagEditVm.State,
                Title = tagEditVm.Title,
                CreateDate = tagEditVm.CreateDate,
                CreatorId = tagEditVm.CreatorId,
                LastModifierId = tagEditVm.LastModifierId,
                LastModifyDate = tagEditVm.LastModifyDate,
                OriginalCreateDate = tagEditVm.OriginalCreateDate,
                OriginalLastModifyDate = tagEditVm.OriginalLastModifyDate
            };
            return Json(receiveTagEditGet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] SendTagEditPost sendTagEditPost)
        {
            ReceiveTagEditPost receiveTagEditPost = new ReceiveTagEditPost();
            try
            {
                TagActions tagActions =
                    new TagActions(_unitOfWork, _userManager);
                var name = User.Identity.Name;
                var lastModifier = await _userManager.FindByNameAsync(name);
                await tagActions.TagEditAction(sendTagEditPost, lastModifier);
                receiveTagEditPost.IsEdited = true;
            }
            catch (Exception e)
            {
                receiveTagEditPost.IsEdited = false;
            }

            return Json(receiveTagEditPost);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(SendTagDeleteGetGet sendTagDeleteGetGet)
        {
            ReceiveTagDeleteGet receiveTagDeleteGet = new ReceiveTagDeleteGet();
            modelList = new Tags(_unitOfWork);
            var tagDeleteVm = await modelList.TagDeleteVms(sendTagDeleteGetGet.Id);
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");
            if (tagDeleteVm.Id == currentTag.Id)
            {
                receiveTagDeleteGet.Message = "این تگ قابل حذف نیست!";
            }

            receiveTagDeleteGet.Id = tagDeleteVm.Id;
            receiveTagDeleteGet.Title = tagDeleteVm.Title;
            receiveTagDeleteGet.NumberOfFilms = tagDeleteVm.NumberOfFilms;

            return Json(receiveTagDeleteGet);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SendTagDeletePost sendTagDeletePost)
        {
            ReceiveTagDeletePost receiveTagDeletePost = new ReceiveTagDeletePost();
            try
            {
                TagActions tagActions =
                    new TagActions(_unitOfWork, _userManager);
                await tagActions.TagDeleteAction(sendTagDeletePost);
                receiveTagDeletePost.IsDeleted = true;
            }
            catch (Exception e)
            {
                receiveTagDeletePost.IsDeleted = false;
            }

            return Json(receiveTagDeletePost);
        }

        [HttpGet]
        public async Task<IActionResult> Details(SendTagDetails sendTagDetails)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListByTagApiVm =
                await films.FilmListByTagApiVms(sendTagDetails.Id, true, sendTagDetails.PageNumber,Request);

            ReceiveTagDetails receiveTagDetails = new ReceiveTagDetails()
            {
                CurrentPageNumber = filmListByTagApiVm.CurrentPageNumber,
                NumberOfPages = filmListByTagApiVm.NumberOfPages,
                TagNecessaryData = filmListByTagApiVm.TagNecessaryData,
                FilmListBaseVms = filmListByTagApiVm.FilmListBaseVms
            };

            return Json(receiveTagDetails);
        }
    }
}