using System.Collections.Generic;
using System.Linq;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Utility
{
    public static class Refiner
    {
        public static IEnumerable<Film> RefineFilms(this IEnumerable<Film> films)
        {
            List<Film> finalFilms = new List<Film>();

            foreach (var film in films)
            {
                List<Tag> tagList = new List<Tag>();
                if (film.Subject.State == State.Disable)
                {
                    continue;
                }
                else
                {
                    var filmTags = film.FilmTags.Select(f => f.Tag);
                    if (filmTags.All(t => t.State != State.Disable))
                    {
                        finalFilms.Add(film);
                    }
                }
            }

            return finalFilms;
        }

        public static IEnumerable<Subject> RefineSubjects(this IEnumerable<Subject> subjects)
        {
            List<Subject> finalSubjects = new List<Subject>();

            foreach (var subject in subjects)
            {
                if (subject.Title==Constants.DefaultTagAndSubjectName||subject.State==State.Disable)
                {
                    continue;
                }
                finalSubjects.Add(subject);
            }

            return finalSubjects;
        }
    }
}
