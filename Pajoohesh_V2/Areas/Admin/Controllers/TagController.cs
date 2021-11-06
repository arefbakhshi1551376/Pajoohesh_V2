using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags;

namespace Pajoohesh_V2.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("Admin")]
    public class TagController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Tags modelList;

        public TagController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment env, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> List(int pageNumber = 1)
        {
            modelList = new Tags(_unitOfWork);
            var tagListVms = await modelList.TagListVms(pageNumber);
            return View(tagListVms);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TagAddVm tagAddVm)
        {
            TagActions tagActions =
                new TagActions(_unitOfWork, _userManager);
            var name = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            await tagActions.TagAddAction(tagAddVm, user);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Edit(int id)
        {
            modelList = new Tags(_unitOfWork);
            var tagEditVm = await modelList.TagEditVms(id);
            return View(tagEditVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TagEditVm tagEditVm)
        {
            TagActions tagActions =
                new TagActions(_unitOfWork, _userManager);
            var name = User.Identity.Name;
            var lastModifier = await _userManager.FindByNameAsync(name);
            await tagActions.TagEditAction(tagEditVm, lastModifier);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int id)
        {
            modelList = new Tags(_unitOfWork);
            var tagDeleteVm = await modelList.TagDeleteVms(id);
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");
            if (tagDeleteVm.Id == currentTag.Id)
            {
                tagDeleteVm.Message = "این تگ قابل حذف نیست!";
            }
            return View(tagDeleteVm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TagDeleteVm tagDeleteVm)
        {
            TagActions tagActions =
                new TagActions(_unitOfWork, _userManager);
            await tagActions.TagDeleteAction(tagDeleteVm);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Details(int id, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListByTagVm = await films.FilmListByTagVms(id, true, pageNumber,Request);
            return View(filmListByTagVm);
        }
    }
}