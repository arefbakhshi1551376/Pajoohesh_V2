$(document).ready(function () {
    function addFilmView(filmId) {
        $.ajax({
            type: "POST",
            url: "/User/Home/AddNumberOfView",
            data: {
                "filmId": filmId
            },
            success: function (response) {
                $("#watch-number-of-views").html(response);
            }
        });
    }
});

function Like(filmId, userId) {
    $.ajax({
        type: "POST",
        url: "/User/Home/LikeDisLikeFilm",
        data: {
            "filmId": filmId,
            "userId": userId
        },
        success: function (response) {
            var filmId = response.FilmId;
            var userId = response.UserId;
            var isLikedByThisUser = response.IsLikedByThisUser;
            var numberOfLikes = response.NumberOfLikes;

            $("#watch-number-of-likes").html(numberOfLikes);

            if (isLikedByThisUser === true) {
                $("#watch-number-of-likes").css("background-color", "#673AB7");
            } else {
                $("#watch-number-of-likes").css("background-color", "#ffffff");
            }
        }
    });
}