using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Utility
{
    public static class GetMediaPath
    {
        public static string GetFilmImageUrl(this Film film,HttpRequest httpRequest)
        {
            if (film != null)
            {
                string finalImagePath = $"{httpRequest.Scheme}://{httpRequest.Host.Value}/Media/Images/Film/{film.ImageUrl}";
                return finalImagePath;
            }
            else
            {
                return "";
            }
        }

        public static string GetFilmFilmUrl(this Film film, HttpRequest httpRequest)
        {
            if (film != null)
            {
                string finalFilmPath = $"{httpRequest.Scheme}://{httpRequest.Host.Value}/Media/Videos/{film.FilmUrl}";
                return finalFilmPath;
            }
            else
            {
                return "";
            }
        }

        public static string GetUserImageUrl(this User user, HttpRequest httpRequest)
        {
            if (user != null)
            {
                string finalImagePath = $"{httpRequest.Scheme}://{httpRequest.Host.Value}/Media/Images/User/{user.ImageUrl}";
                return finalImagePath;
            }
            else
            {
                return "";
            }
        }

        public static string GetSubjectImageUrl(this Subject subject, HttpRequest httpRequest)
        {
            if (subject != null)
            {
                string finalImagePath = $"{httpRequest.Scheme}://{httpRequest.Host.Value}/Media/Images/Subject/{subject.ImageUrl}";
                return finalImagePath;
            }
            else
            {
                return "";
            }
        }
    }
}
