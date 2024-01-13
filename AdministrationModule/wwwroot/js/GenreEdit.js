$(document).ready(function () {
    document.getElementById('ajax-save').addEventListener("click", async (evt) => {

        var genreIdField = $('#genreId');
        var genreNameField = $('#genreName');
        var genreDescriptionField = $('#genreDescription');

        if (!genreNameField.val() || !genreDescriptionField.val()) {
            alert("Both genre name and description are required!");
            evt.preventDefault();
            return false;
        }
        $.ajax({
            type: "POST",
            url: "/Genre/Edit/" + genreIdField.val(),
            contentType: 'application/json',
            data: JSON.stringify({
                Name: genreNameField.val(),
                Description: genreDescriptionField.val()
            }),
            success: function (data) {
                alert("Ajax POST success");
                console.log(data);
                alert(data);
                location.reload();
            },
            error: function (error) {
                alert("Ajax POST error");
                console.log(error);
                alert(error)
            }
        });
    });
});