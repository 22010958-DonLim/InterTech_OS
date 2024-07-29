

$.ajaxSetup({ cache: false });
StrawberryGen = function () {
    function addlike(articleId, userId) {
        // Prepare the data to be sent in the request
        console.log('addlike function called with articleId:', articleId, 'and userId:', userId); 
        var data = {
            ArticleId: articleId,
            UserId: userId,
            Likes: 1, // Assuming 1 like per click
            LikeDateTime: new Date().toISOString()
        };

        // Make an AJAX call to the server
        $.ajax({
            url: '~/StrawberryLikes/Create',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        })
            .done(function (result) {
                // Assuming result is the updated like count
                var likeCountElement = document.getElementById('like-count');
                likeCountElement.textContent = result;
                alert('Like added successfully!');
            })
            .fail(function (xhr) {
                alert(xhr.status + " " + xhr.responseText);
            });
    }

    return {
        addlike: addlike
    }
}();