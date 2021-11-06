using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.CData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Comment;
using Pajoohesh_V2.Model.Models.Relationship;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films;

namespace Pajoohesh_V2.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private UserManager<Model.Models.Main.User.User> _userManager;
        private IUserRepository _userRepository;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment env, UserManager<Model.Models.Main.User.User> userManager, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            this.env = env;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        // GET
        public async Task<IActionResult> List(int pageNumber = 1)
        {
            Films modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmTheFirstPageListsVms(false, Request);

            return View(forListFilm);
        }

        public async Task<IActionResult> Search(string statement, int pageNumber = 1)
        {
            if (statement == null || statement.Trim() == "")
            {
                return RedirectToAction("List");
            }

            Films modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            FilmListBySearchStatementVm filmListBySearchStatementVm = new FilmListBySearchStatementVm();

            if (User.Identity.IsAuthenticated)
            {
                string currentUserName = User.Identity.Name;
                var currentUser = await _userManager.FindByNameAsync(currentUserName);
                if (await _userManager.IsInRoleAsync(currentUser, Constants.AdminRole))
                {
                    filmListBySearchStatementVm = await modelListFilms.FilmSearchVms(false, pageNumber, statement, Request);
                }
            }
            filmListBySearchStatementVm = await modelListFilms.FilmSearchVms(false, pageNumber, statement, Request);
            return View(filmListBySearchStatementVm);
        }

        // GET
        public async Task<IActionResult> Newest(int pageNumber = 1)
        {
            Films modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmNewestListVms(false, pageNumber, Request);

            return View(forListFilm);
        }

        // GET
        public async Task<IActionResult> MostVisitedOfTheWeek(int pageNumber = 1)
        {
            Films modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmMostVisitedOfTheWeekListVms(false, pageNumber, Request);

            return View(forListFilm);
        }

        public async Task<IActionResult> Watch(int id, bool isManagement = false, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var httpRequest = Request;
            //var hostUrl = Request.Host.Value.ToString();
            var filmWatchVm = await films.FilmWatchVms(id, isManagement, httpRequest, pageNumber);

            return View(filmWatchVm);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int filmId, string userId, string commentText)
        {
            Comment comment = new Comment()
            {
                CreateDate = DateTime.UtcNow,
                Creator = await _userManager.FindByIdAsync(userId),
                Film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmId),
                State = State.Enable,
                Text = commentText
            };
            await _unitOfWork.CommentRepository.AddAsync(comment);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Watch", new { id = filmId });
        }

        [HttpPost]
        public async Task<int> AddNumberOfView(int filmId)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmId);
            try
            {
                currentFilm.NumberOfViews++;
                await _unitOfWork.FilmRepository.Update(currentFilm);
                await _unitOfWork.SaveAsync();
                return currentFilm.NumberOfViews;
            }
            catch (Exception e)
            {
                return currentFilm.NumberOfViews;
            }
        }

        [HttpPost]
        public async Task<int> LikeDisLikeFilm(int filmId, string userId)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmId);
            var currentUser = await _userManager.FindByIdAsync(userId);

            FilmLikeUser filmLikeUser = new FilmLikeUser()
            {
                User = currentUser,
                UserId = currentUser.Id,
                Film = currentFilm,
                FilmId = currentFilm.Id
            };

            if (currentFilm.LikeUsers.Contains(filmLikeUser))
            {
                currentFilm.LikeUsers.Remove(filmLikeUser);
            }
            else
            {
                currentFilm.LikeUsers.Add(filmLikeUser);
            }

            await _unitOfWork.FilmRepository.Update(currentFilm);
            await _unitOfWork.SaveAsync();

            return currentFilm.LikeUsers.Count;
        }

        public async Task<IActionResult> ListByTag(int tagId, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListVms = await films.FilmListByTagVms(tagId, false, pageNumber, Request);

            return View(filmListVms);
        }

        public async Task<IActionResult> ListByUploader(string uploaderId, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListVms = await films.FilmListByUploaderVms(uploaderId, false, pageNumber, Request);

            return View(filmListVms);
        }

        public async Task<IActionResult> ListBySubject(int subjectId, int pageNumber = 1)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListVms = await films.FilmListBySubjectVms(subjectId, false, pageNumber, Request);

            return View(filmListVms);
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}