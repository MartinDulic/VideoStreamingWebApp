$(document).ready(function () {
    document.getElementById('ajax-delete').addEventListener("click", async (evt) => {
        var genreIdField = $('#genreId');
        alert("event inbound");

        if (!genreIdField.val()) {
            alert("Genre ID required!");
            evt.preventDefault();
            return false;
        }

        $.ajax({
            type: "POST",
            url: "/Genre/Delete/" + genreIdField.val(),
            contentType: 'application/json',
            data: JSON.stringify({
                Name: "a",
                Description: "a"
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