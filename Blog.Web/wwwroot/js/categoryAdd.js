$(document).ready(function () {
    $("#btnSave").click(function (event) {
        event.preventDefault();
        var addUrl = app.Urls.categoryAddUrl;
        var redirectUrl = app.Urls.articleAddUrl;
        var categoryAddDto = {
            Name: $("input[id=categoryName]").val()
        }
        var JsonData = JSON.stringify(categoryAddDto);
        console.log(JsonData);
        $.ajax({
            url: addUrl,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: JsonData,
            success: function (data) {
                setTimeout(function () {
                    window.location.href = redirectUrl;
                }, 1500);
            },
            error: function () {
                toast.error("one error.", "filed");
            }
        });
    });
})