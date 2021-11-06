using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.Models.Relationship;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Models
{
    public class Films
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private UserManager<User> _userManager;
        private IUserRepository _userRepository;

        public Films(IUnitOfWork unitOfWork, IWebHostEnvironment env, UserManager<User> userManager,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            this.env = env;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<FilmListVm> FilmListVms(bool isInManagement, int pageNumber)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            var firstFilms = await _unitOfWork.FilmRepository.GetAllAsync(skipNumber, numberOfFilmsInEachPage);
            var lastFilms = firstFilms.RefineFilms();
            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in lastFilms)
            {
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = film.ImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListVm filmListVm = null;

            if (isInManagement)
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms
                };
            }
            else
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList()
                };
            }

            return filmListVm;
        }

        public async Task<FilmTheFirstPageListsVm> FilmTheFirstPageListsVms(bool isInManagement, HttpRequest httpRequest)
        {
            var newestFilms = (await _unitOfWork.FilmRepository.GetAllAsync())
                .OrderBy(f => f.UploadDate)
                .Take(8)
                .RefineFilms();

            List<FilmListBaseVm> filmListBaseVmsForNewestFilms = new List<FilmListBaseVm>();

            foreach (var film in newestFilms)
            {
                var finalImagePathForNewestFilm = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePathForNewestFilm,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl,
                    UploaderId = film.Uploader.Id
                };
                filmListBaseVmsForNewestFilms.Add(currentFilmListBaseVm);
            }

            var sevenDaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

            var mostVisitedOfTheWeekFilms = (await _unitOfWork.FilmRepository.GetAllAsync())
                .Where(f => f.UploadDate >= sevenDaysAgo)
                .OrderBy(f => f.LikeUsers.Count)
                .Take(4)
                .RefineFilms();

            List<FilmListBaseVm> filmListBaseVmsForMostVisitedOfTheWeekFilms = new List<FilmListBaseVm>();

            foreach (var film in mostVisitedOfTheWeekFilms)
            {
                var finalImagePathForMostVisitedOfTheWeekFilm = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePathForMostVisitedOfTheWeekFilm,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl,
                    UploaderId = film.Uploader.Id
                };
                filmListBaseVmsForMostVisitedOfTheWeekFilms.Add(currentFilmListBaseVm);
            }

            FilmListVm finalNewestFilms = null;
            FilmListVm finalMostVisitedOfTheWeek = null;

            if (isInManagement)
            {
                finalNewestFilms = new FilmListVm()
                {
                    NumberOfPages = 1,
                    CurrentPageNumber = 1,
                    FilmListBaseVms = filmListBaseVmsForNewestFilms
                };

                finalMostVisitedOfTheWeek = new FilmListVm()
                {
                    NumberOfPages = 1,
                    CurrentPageNumber = 1,
                    FilmListBaseVms = filmListBaseVmsForMostVisitedOfTheWeekFilms
                };
            }
            else
            {
                finalNewestFilms = new FilmListVm()
                {
                    NumberOfPages = 1,
                    CurrentPageNumber = 1,
                    FilmListBaseVms = filmListBaseVmsForNewestFilms.Where(f => f.State == State.Enable).ToList()
                };

                finalMostVisitedOfTheWeek = new FilmListVm()
                {
                    NumberOfPages = 1,
                    CurrentPageNumber = 11,
                    FilmListBaseVms = filmListBaseVmsForMostVisitedOfTheWeekFilms.Where(f => f.State == State.Enable)
                        .ToList()
                };
            }

            FilmTheFirstPageListsVm filmTheFirstPageListsVm = new FilmTheFirstPageListsVm()
            {
                FilmMostVisitedOfTheWeekListVms = finalMostVisitedOfTheWeek,
                NewestFilms = finalNewestFilms
            };

            return filmTheFirstPageListsVm;
        }

        public async Task<FilmListVm> FilmMostVisitedOfTheWeekListVms(bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            var sevenDaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

            var firstFilms = (await _unitOfWork.FilmRepository.GetAllAsync(skipNumber, numberOfFilmsInEachPage))
                .Where(f => f.UploadDate >= sevenDaysAgo)
                .OrderBy(f => f.UploadDate)
                .Take(20);

            var lastFilms = firstFilms.RefineFilms();
            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in lastFilms)
            {
                var finalImagePathForFilm = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = film.ImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl,
                    UploaderId = film.Uploader.Id
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListVm filmListVm = null;

            if (isInManagement)
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms
                };
            }
            else
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList()
                };
            }

            return filmListVm;
        }

        public async Task<FilmListBySearchStatementVm> FilmSearchVms(bool isInManagement, int pageNumber, string statement, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            List<string> searchWords = statement.Split(" ").ToList();
            var allFilms = (await _unitOfWork.FilmRepository.GetAllAsync()).ToList();
            List<Film> selectedFilms = new List<Film>();
            foreach (var searchWord in searchWords)
            {
                foreach (var film in allFilms)
                {
                    var filmName = film.Name;
                    var filmDescription = film.Description;
                    var filmSubjectTitle = film.Subject.Title;
                    var filmCommentsTitles = film.Comments.Select(c => c.Text);
                    var filmUploaderName = film.Uploader.Name;
                    var filmUploaderFamily = film.Uploader.Family;
                    var filmTagTitles = film.FilmTags.Select(t => t.Tag).Select(t => t.Title);
                    var filmLastModifierName = film.LastModifier?.Name;
                    var filmLastModifierFamily = film.LastModifier?.Family;

                    if (filmName.Contains(searchWord.Trim()))
                    {
                        selectedFilms.Add(film);
                    }
                    else
                    {
                        if (filmDescription.Contains(searchWord.Trim()))
                        {
                            selectedFilms.Add(film);
                        }
                        else
                        {
                            if (filmSubjectTitle.Contains(searchWord.Trim()))
                            {
                                selectedFilms.Add(film);
                            }
                            else
                            {
                                if (filmTagTitles.Any(t => t.Contains(searchWord.Trim())))
                                {
                                    selectedFilms.Add(film);
                                }
                                else
                                {
                                    if (filmCommentsTitles.Any(c => c.Contains(searchWord.Trim())))
                                    {
                                        selectedFilms.Add(film);
                                    }
                                    else
                                    {
                                        if (filmUploaderName.Contains(searchWord.Trim()) ||
                                            filmUploaderFamily.Contains(searchWord.Trim()))
                                        {
                                            selectedFilms.Add(film);
                                        }
                                        else
                                        {
                                            if (filmLastModifierFamily != null &&
                                                filmLastModifierName != null &&
                                                (filmLastModifierName.Contains(searchWord.Trim()) ||
                                                 filmLastModifierFamily.Contains(searchWord.Trim())))
                                            {
                                                selectedFilms.Add(film);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var firstFilms = selectedFilms.Distinct().Skip(skipNumber).Take(numberOfFilmsInEachPage);

            var lastFilms = firstFilms.RefineFilms();
            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in lastFilms)
            {
                var finalImagePath = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePath,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl,
                    UploaderId = film.Uploader.Id
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListBySearchStatementVm filmListBySearchStatementVm = null;

            if (isInManagement)
            {
                filmListBySearchStatementVm = new FilmListBySearchStatementVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms,
                    Statement = statement
                };
            }
            else
            {
                filmListBySearchStatementVm = new FilmListBySearchStatementVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList(),
                    Statement = statement
                };
            }

            return filmListBySearchStatementVm;
        }

        public async Task<FilmListVm> FilmNewestListVms(bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            var firstFilms = (await _unitOfWork.FilmRepository.GetAllAsync(skipNumber, numberOfFilmsInEachPage))
                .OrderBy(f => f.UploadDate)
                .Take(30);

            var lastFilms = firstFilms.RefineFilms();
            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in lastFilms)
            {
                var finalImagePath = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePath,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    UploaderImage = film.Uploader.ImageUrl,
                    UploaderId = film.Uploader.Id
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListVm filmListVm = null;

            if (isInManagement)
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms
                };
            }
            else
            {
                filmListVm = new FilmListVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList()
                };
            }

            return filmListVm;
        }

        public async Task<FilmListApiVm> FilmApiListVms(bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            var firstFilms = await _unitOfWork.FilmRepository.GetAllAsync(skipNumber, numberOfFilmsInEachPage);
            var lastFilms = firstFilms.RefineFilms();
            List<FilmListBaseApiVm> filmListBaseApiVms = new List<FilmListBaseApiVm>();

            foreach (var film in lastFilms)
            {
                //var finalPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", film.ImageUrl);
                var finalImagePathForFilm = film.GetFilmImageUrl(httpRequest);


                List<TagNecessaryData> tagNecessaryDataListForThisFilm = new List<TagNecessaryData>();
                foreach (var filmTag in film.FilmTags)
                {
                    Tag currentTag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == filmTag.TagId);
                    TagNecessaryData currenTagNecessaryData = new TagNecessaryData()
                    {
                        CreateDate = currentTag.CreateDate.ToPersianDate(),
                        Creator = currentTag.Creator.GetName(),
                        Id = currentTag.Id,
                        LastModifier = currentTag.LastModifier?.GetName(),
                        LastModifyDate = currentTag.LastModifyDate?.ToPersianDate(),
                        State = currentTag.State,
                        Title = currentTag.Title
                    };
                    tagNecessaryDataListForThisFilm.Add(currenTagNecessaryData);
                }

                var finalImagePathForUploader = film.Uploader.GetUserImageUrl(httpRequest);

                var currentFilmListBaseApiVm = new FilmListBaseApiVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePathForFilm,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State,
                    TagNecessaryDataList = tagNecessaryDataListForThisFilm,
                    UploaderId = film.Uploader.Id,
                    UploaderImage = finalImagePathForUploader,
                };
                filmListBaseApiVms.Add(currentFilmListBaseApiVm);
            }

            FilmListApiVm filmListApiVm = null;

            if (isInManagement)
            {
                filmListApiVm = new FilmListApiVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseApiVm = filmListBaseApiVms,
                };
            }
            else
            {
                filmListApiVm = new FilmListApiVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseApiVm = filmListBaseApiVms.Where(f => f.State == State.Enable).ToList()
                };
            }

            return filmListApiVm;
        }

        public async Task<FilmWatchVm> FilmWatchVms(int id, bool isManagement, HttpRequest httpRequest, int pageNumber = 1)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);
            List<int> tagIds = new List<int>();
            List<RelatedFilmsVm> relatedFilmsVms = new List<RelatedFilmsVm>();
            var filmTags = currentFilm.FilmTags.Select(f => f.Tag).ToList();
            foreach (var currentFilmTag in filmTags)
            {
                tagIds.Add(currentFilmTag.Id);
            }

            var similarFilms =
                (await _unitOfWork.FilmRepository.FindByTagsAndSubjectAsync(tagIds, currentFilm.Subject.Title, id))
                .RefineFilms();
            foreach (var similarFilm in similarFilms)
            {
                var finalImagePathForSimilarFilm = similarFilm.GetFilmImageUrl(httpRequest);
                relatedFilmsVms.Add(new RelatedFilmsVm()
                {
                    Description = similarFilm.Description,
                    Id = similarFilm.Id,
                    ImageUrl = finalImagePathForSimilarFilm,
                    Name = similarFilm.Name,
                    Uploader = similarFilm.Uploader.GetName(),
                    UploadPastDateTime = similarFilm.UploadDate.CallPastTime(),
                    NumberOfViews = similarFilm.NumberOfViews,
                    UploadDate = similarFilm.UploadDate.ToPersianDate()
                });
            }

            Comments comments = new Comments(_unitOfWork);
            var commentListVm = await comments.CommentListVms(isManagement, pageNumber, id, httpRequest);

            var finalImagePathForCurrentFilm = currentFilm.GetFilmImageUrl(httpRequest);
            var finalImagePathForUploader = currentFilm.Uploader.GetUserImageUrl(httpRequest);
            var finalFilmPathForCurrentFilm = currentFilm.GetFilmFilmUrl(httpRequest);

            FilmWatchVm filmWatchVm = new FilmWatchVm()
            {
                Description = currentFilm.Description,
                Id = currentFilm.Id,
                UploadDate = currentFilm.UploadDate.ToPersianDate(),
                ImageUrl = finalImagePathForCurrentFilm,
                Name = currentFilm.Name,
                NumberOfViews = currentFilm.NumberOfViews,
                OriginalUploadDate = currentFilm.UploadDate,
                UploadPastDateTime = currentFilm.UploadDate.CallPastTime(),
                Uploader = currentFilm.Uploader.GetName(),
                RelatedFilmsVms = relatedFilmsVms,
                FilmUrl = finalFilmPathForCurrentFilm,
                Subject = currentFilm.Subject,
                Tags = filmTags,
                UploaderImage = finalImagePathForUploader,
                NumberOfLikes = currentFilm.LikeUsers.Count,
                CommentListVm = commentListVm
            };
            return filmWatchVm;
        }

        public async Task<FilmWatchApiVm> FilmWatchApiVms(int id, bool isManagement, HttpRequest httpRequest, int pageNumber = 1)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);
            List<int> tagIds = new List<int>();
            List<RelatedFilmsVm> relatedFilmsVms = new List<RelatedFilmsVm>();
            var filmTags = currentFilm.FilmTags.Select(f => f.Tag).ToList();
            foreach (var currentFilmTag in filmTags)
            {
                tagIds.Add(currentFilmTag.Id);
            }

            var similarFilms =
                (await _unitOfWork.FilmRepository.FindByTagsAndSubjectAsync(tagIds, currentFilm.Subject.Title, id))
                .RefineFilms();
            foreach (var similarFilm in similarFilms)
            {
                var finalImagePathForSimilarFilm = similarFilm.GetFilmImageUrl(httpRequest);

                relatedFilmsVms.Add(new RelatedFilmsVm()
                {
                    Description = similarFilm.Description,
                    Id = similarFilm.Id,
                    ImageUrl = finalImagePathForSimilarFilm,
                    Name = similarFilm.Name,
                    Uploader = similarFilm.Uploader.GetName(),
                    UploadPastDateTime = similarFilm.UploadDate.CallPastTime(),
                    NumberOfViews = similarFilm.NumberOfViews,
                    UploadDate = similarFilm.UploadDate.ToPersianDate()
                });
            }

            Comments comments = new Comments(_unitOfWork);
            var commentListVm = await comments.CommentListVms(isManagement, pageNumber, id, httpRequest);

            SubjectNecessaryData subjectNecessaryData = new SubjectNecessaryData()
            {
                Creator = currentFilm.Subject.Creator.GetName(),
                Id = currentFilm.Subject.Id,
                State = currentFilm.Subject.State,
                Title = currentFilm.Subject.Title,
                CreateDate = currentFilm.Subject.CreateDate.ToPersianDate(),
                LastModifier = currentFilm.Subject.LastModifier?.GetName(),
                LastModifyDate = currentFilm.Subject.LastModifyDate?.ToPersianDate()
            };

            List<TagNecessaryData> tagNecessaryDataList = new List<TagNecessaryData>();
            foreach (var filmTag in filmTags)
            {
                TagNecessaryData currentTagNecessaryData = new TagNecessaryData()
                {
                    Creator = filmTag.Creator.GetName(),
                    Id = filmTag.Id,
                    State = filmTag.State,
                    Title = filmTag.Title,
                    CreateDate = filmTag.CreateDate.ToPersianDate(),
                    LastModifier = filmTag.LastModifier?.GetName(),
                    LastModifyDate = filmTag.LastModifyDate?.ToPersianDate(),
                };
                tagNecessaryDataList.Add(currentTagNecessaryData);
            }

            var finalImagePathForCurrentFilm = currentFilm.GetFilmImageUrl(httpRequest);
            var finalImagePathForUploader = currentFilm.Uploader.GetUserImageUrl(httpRequest);
            var finalFilmPathForCurrentFilm = currentFilm.GetFilmFilmUrl(httpRequest);

            FilmWatchApiVm filmWatchApiVm = new FilmWatchApiVm()
            {
                Description = currentFilm.Description,
                Id = currentFilm.Id,
                UploadDate = currentFilm.UploadDate.ToPersianDate(),
                ImageUrl = finalImagePathForCurrentFilm,
                Name = currentFilm.Name,
                NumberOfViews = currentFilm.NumberOfViews,
                OriginalUploadDate = currentFilm.UploadDate,
                UploadPastDateTime = currentFilm.UploadDate.CallPastTime(),
                Uploader = currentFilm.Uploader.GetName(),
                RelatedFilmsVms = relatedFilmsVms,
                FilmUrl = finalFilmPathForCurrentFilm,
                UploaderImage = finalImagePathForUploader,
                NumberOfLikes = currentFilm.LikeUsers.Count,
                CommentListVm = commentListVm,
                SubjectNecessaryData = subjectNecessaryData,
                TagNecessaryDataList = tagNecessaryDataList
            };
            return filmWatchApiVm;
        }

        public async Task<FilmListByTagVm> FilmListByTagVms(int tagId, bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            IEnumerable<Film> firstFilms = null;

            if (isInManagement)
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByTagAsync(tagId));
            }
            else
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByTagAsync(tagId)).RefineFilms();
            }

            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in firstFilms)
            {
                var finalImageUrl = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListByTagVm filmListVm = new FilmListByTagVm()
            {
                NumberOfPages = numberOfPages,
                CurrentPageNumber = pageNumber,
                FilmListBaseVms = filmListBaseVms,
                Tag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == tagId)
            };

            return filmListVm;
        }

        public async Task<FilmListByTagApiVm> FilmListByTagApiVms(int tagId, bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            IEnumerable<Film> firstFilms = null;

            if (isInManagement)
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByTagAsync(tagId));
            }
            else
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByTagAsync(tagId)).RefineFilms();
            }

            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in firstFilms)
            {
                var finalImageUrl = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            var currentTag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == tagId);
            TagNecessaryData tagNecessaryData = new TagNecessaryData()
            {
                Creator = currentTag.Creator.GetName(),
                Id = currentTag.Id,
                State = currentTag.State,
                Title = currentTag.Title,
                CreateDate = currentTag.CreateDate.ToPersianDate(),
                LastModifier = currentTag.LastModifier?.GetName(),
                LastModifyDate = currentTag.LastModifyDate?.ToPersianDate()
            };

            FilmListByTagApiVm filmListVm = new FilmListByTagApiVm()
            {
                NumberOfPages = numberOfPages,
                CurrentPageNumber = pageNumber,
                FilmListBaseVms = filmListBaseVms,
                TagNecessaryData = tagNecessaryData
            };

            return filmListVm;
        }

        public async Task<FilmListByUploaderVm> FilmListByUploaderVms(string uploaderId, bool isInManagement,
            int pageNumber,HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            IEnumerable<Film> firstFilms = null;

            if (isInManagement)
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByUploaderId(uploaderId));
            }
            else
            {
                firstFilms = (await _unitOfWork.FilmRepository.FindByUploaderId(uploaderId)).RefineFilms();
            }

            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in firstFilms)
            {
                var finalImageUrl = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListByUploaderVm filmListVm = new FilmListByUploaderVm()
            {
                NumberOfPages = numberOfPages,
                CurrentPageNumber = pageNumber,
                FilmListBaseVms = filmListBaseVms,
                User = await _userRepository.FindAsync(uploaderId)
            };

            return filmListVm;
        }

        public async Task<FilmListBySubjectVm> FilmListBySubjectVms(int subjectId, bool isInManagement, int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            IEnumerable<Film> films = null;

            if (isInManagement)
            {
                films = (await _unitOfWork.FilmRepository.FindBySubjectAsync(subjectId, null));
            }
            else
            {
                films = (await _unitOfWork.FilmRepository.FindBySubjectAsync(subjectId, null)).RefineFilms();
            }

            var currentSubject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == subjectId);
            //var subjectImageUrl = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", currentSubject.ImageUrl);

            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in films)
            {
                var finalImageUrl = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = film.ImageUrl,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListBySubjectVm filmListVm = null;

            if (isInManagement == true)
            {
                filmListVm = new FilmListBySubjectVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms,
                    SubjectId = currentSubject.Id,
                    ImageUrl = currentSubject.ImageUrl,
                    SubjectTitle = currentSubject.Title
                };
            }
            else
            {
                filmListVm = new FilmListBySubjectVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList(),
                    SubjectId = currentSubject.Id,
                    ImageUrl = currentSubject.ImageUrl,
                    SubjectTitle = currentSubject.Title
                };
            }

            return filmListVm;
        }

        public async Task<FilmListBySubjectVm> FilmListBySubjectApiVms(int subjectId, bool isInManagement,
            int pageNumber, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }

            IEnumerable<Film> films = null;

            if (isInManagement)
            {
                films = (await _unitOfWork.FilmRepository.FindBySubjectAsync(subjectId, null));
            }
            else
            {
                films = (await _unitOfWork.FilmRepository.FindBySubjectAsync(subjectId, null)).RefineFilms();
            }

            var currentSubject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == subjectId);
            var finalImagePathForSubject = currentSubject.GetSubjectImageUrl(httpRequest);

            List<FilmListBaseVm> filmListBaseVms = new List<FilmListBaseVm>();

            foreach (var film in films)
            {
                var finalImagePathForFilm = film.GetFilmImageUrl(httpRequest);
                var currentFilmListBaseVm = new FilmListBaseVm()
                {
                    Id = film.Id,
                    ImageUrl = finalImagePathForFilm,
                    Description = film.Description,
                    NumberOfViews = film.NumberOfViews,
                    Name = film.Name,
                    UploadDate = film.UploadDate.ToPersianDate(),
                    Uploader = film.Uploader.GetName(),
                    OriginalUploadDate = film.UploadDate,
                    UploadPastDateTime = film.UploadDate.CallPastTime(),
                    State = film.State
                };
                filmListBaseVms.Add(currentFilmListBaseVm);
            }

            FilmListBySubjectVm filmListVm = null;

            if (isInManagement == true)
            {
                filmListVm = new FilmListBySubjectVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms,
                    SubjectId = currentSubject.Id,
                    ImageUrl = finalImagePathForSubject,
                    SubjectTitle = currentSubject.Title
                };
            }
            else
            {
                filmListVm = new FilmListBySubjectVm()
                {
                    NumberOfPages = numberOfPages,
                    CurrentPageNumber = pageNumber,
                    FilmListBaseVms = filmListBaseVms.Where(f => f.State == State.Enable).ToList(),
                    SubjectId = currentSubject.Id,
                    ImageUrl = currentSubject.ImageUrl,
                    SubjectTitle = currentSubject.Title
                };
            }

            return filmListVm;
        }

        public async Task<FilmDeleteVm> FilmDeleteVms(int id)
        {
            var film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);
            var filmTags = film.FilmTags.Select(f => f.Tag).ToList();
            FilmDeleteVm filmDeleteVm = new FilmDeleteVm()
            {
                FilmUrl = film.FilmUrl,
                Description = film.Description,
                Id = film.Id,
                UploadDate = film.UploadDate.ToPersianDate(),
                Tags = filmTags,
                Name = film.Name,
                Subject = film.Subject,
                UploadPastDate = film.UploadDate.CallPastTime()
            };
            return filmDeleteVm;
        }

        public async Task<FilmDeleteApiVm> FilmDeleteApiVms(int id)
        {
            var film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);
            var filmTags = film.FilmTags.Select(f => f.Tag).ToList();
            List<TagNecessaryData> tagNecessaryDataList = new List<TagNecessaryData>();
            foreach (var filmTag in filmTags)
            {
                TagNecessaryData currentTagNecessaryData = new TagNecessaryData()
                {
                    Creator = filmTag.Creator.GetName(),
                    Id = filmTag.Id,
                    State = filmTag.State,
                    Title = filmTag.Title,
                    CreateDate = filmTag.CreateDate.ToPersianDate(),
                    LastModifier = filmTag.LastModifier?.GetName(),
                    LastModifyDate = filmTag.LastModifyDate?.ToPersianDate(),
                };
                tagNecessaryDataList.Add(currentTagNecessaryData);
            }

            SubjectNecessaryData subjectNecessaryData = new SubjectNecessaryData()
            {
                Creator = film.Subject.Creator.GetName(),
                Id = film.Subject.Id,
                State = film.Subject.State,
                Title = film.Subject.Title,
                CreateDate = film.Subject.CreateDate.ToPersianDate(),
                LastModifier = film.Subject.LastModifier?.GetName(),
                LastModifyDate = film.Subject.LastModifyDate?.ToPersianDate()
            };

            FilmDeleteApiVm filmDeleteApiVm = new FilmDeleteApiVm()
            {
                FilmUrl = film.FilmUrl,
                Description = film.Description,
                Id = film.Id,
                UploadDate = film.UploadDate.ToPersianDate(),
                TagNecessaryDataList = tagNecessaryDataList,
                Name = film.Name,
                SubjectNecessaryData = subjectNecessaryData,
                UploadPastDate = film.UploadDate.CallPastTime()
            };
            return filmDeleteApiVm;
        }

        public async Task<FilmEditVm> FilmEditVms(int id, HttpRequest httpRequest)
        {
            var film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);
            Tags tags = new Tags(_unitOfWork);
            var filmTags = film.FilmTags.Select(f => f.Tag).ToList();
            var stringTags = await tags.GetTagsAsStringAsync(filmTags);
            var subjecs = (await _unitOfWork.SubjectRepository.GetAllAsync()).Where(s => s.Title != "عمومی");

            List<User> filmLikeUsers = new List<User>();

            foreach (var item in film.LikeUsers)
            {
                var currentUser = await _userManager.FindByIdAsync(item.UserId);
                filmLikeUsers.Add(currentUser);
            }

            var finalImageUrl = film.GetFilmImageUrl(httpRequest);
            var finalFilmUrl = film.GetFilmFilmUrl(httpRequest);

            FilmEditVm filmEditVm = new FilmEditVm()
            {
                State = film.State,
                Tags = stringTags,
                Description = film.Description,
                Id = film.Id,
                Name = film.Name,
                Subjects = subjecs.ToList(),
                UploadDate = film.UploadDate.ToPersianDate(),
                Comments = film.Comments,
                UploaderId = film.Uploader.Id,
                FilmUrl = finalFilmUrl,
                ImageUrl = finalImageUrl,
                LastModifyDate = film.LaseModifyDate?.ToPersianDate(),
                LastModifierId = film.LastModifier?.Id,
                OriginalLastModifyDate = film.LaseModifyDate,
                OriginalUploadDate = film.UploadDate,
                SubjectId = film.Subject.Id,
                LikeUsers = filmLikeUsers,
                NumberOfViews = film.NumberOfViews
            };

            return filmEditVm;
        }

        public async Task<FilmAddVm> FilmAddVms()
        {
            var subjecs = (await _unitOfWork.SubjectRepository.GetAllAsync()).Where(s => s.Title != "عمومی");
            FilmAddVm filmAddVm = new FilmAddVm()
            {
                SubjectSelectList = new SelectList(subjecs, "Id", "Title")
            };

            return filmAddVm;
        }
    }
}