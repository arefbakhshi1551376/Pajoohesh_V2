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
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects;

namespace Pajoohesh_V2.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Subjects modelList;

        public SubjectController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment env, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> List(int pageNumber = 1)
        {
            modelList = new Subjects(_unitOfWork, env);
            var hostUrl = Request.Host.Value.ToString();
            var currentSubjectsList = await modelList.SubjectListVms(pageNumber, hostUrl);
            return View(currentSubjectsList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(SubjectAddVm subjectAddVm)
        {
            SubjectActions subjectActions =
                new SubjectActions(_unitOfWork, _userManager, env);
            var name = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            await subjectActions.SubjectAddAction(subjectAddVm, user);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Edit(int id)
        {
            modelList = new Subjects(_unitOfWork, env);
            var hostUrl = Request.Host.Value.ToString();
            var subjectEditVm = await modelList.SubjectEditVms(id, hostUrl);
            return View(subjectEditVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SubjectEditVm subjectEditVm)
        {
            SubjectActions subjectActions =
                new SubjectActions(_unitOfWork, _userManager, env);
            var name = User.Identity.Name;
            var lastModifier = await _userManager.FindByNameAsync(name);
            await subjectActions.SubjectEditAction(subjectEditVm, lastModifier);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int id)
        {
            modelList = new Subjects(_unitOfWork, env);
            var subjectDeleteVm = await modelList.SubjectDeleteVms(id);
            var currentSubject = await _unitOfWork.SubjectRepository.FindByTitleAsync("عمومی");
            if (subjectDeleteVm.Id == currentSubject.Id)
            {
                subjectDeleteVm.Message = "این موضوع قابل حذف نیست!";
            }
            return View(subjectDeleteVm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SubjectDeleteVm subjectDeleteVm)
        {
            SubjectActions subjectActions =
                new SubjectActions(_unitOfWork, _userManager, env);
            await subjectActions.SubjectDeleteAction(subjectDeleteVm);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Details(int id, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListBySubjectVm = await films.FilmListBySubjectVms(id, true, pageNumber,Request);
            return View(filmListBySubjectVm);
        }
    }
}