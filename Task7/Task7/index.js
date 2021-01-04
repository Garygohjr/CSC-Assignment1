var clarifaiApiKey = '1f3855a8b0b64c01b5d3a1a1816af4f9';
var workflowId = 'General';

// Handles image upload
function uploadImage() {
    var app = new Clarifai.App({
        apiKey: clarifaiApiKey
    });

    var preview = document.getElementById('img');
    var file = document.querySelector('input[type=file]').files[0];
    var reader = new FileReader();
    $(".analysis").html("");
    document.getElementById("loader").style.display = "unset";
    preview.style.display = "none";
    reader.addEventListener("load", function () {
        veryfiImage(reader.result);
        var imageData = reader.result;
        imageData = imageData.replace(/^data:image\/(.*);base64,/, '');
        app.inputs.create([
            {
                base64: imageData
            }
        ]).then(
            function (response) {
                console.log(response);
            },
            function (err) {
                console.log(err);
                alert("Not an image");
            }
        )
        app.models.initModel({ id: "Receipt Learner", version: "536d183d26574b3d9d8d926dee0466cf" })
            .then(generalModel => {
                return generalModel.predict(imageData);
            })
            .then(
                function (response) {
                    var concepts = response['outputs'][0]['data']['concepts'];
                    if (concepts[0].value > 0.5) {
                        $(".analysis").html("<p> is probably a receipt </p>");
                    } else {
                        $(".analysis").html("<p> is probably not a receipt </p>");
                    }
                    preview.src = reader.result;
                    document.getElementById("loader").style.display = "none";
                    preview.style.display = "inherit";
                },
                function (err) {
                    console.log(err);
                    document.getElementById("loader").style.display = "none";
                }
        )
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
}

function getModels() {
    app.models.list().then(
        function (response) {
            var ids = response.rawData;

            console.log(response);
            console.log(ids);
        },
        function (err) {
            alert(err);
        }
    );
}

function veryfiImage(base64string) {
    //$.ajax({
    //    type: 'POST',
    //    url: 'api/v1/verify/verifi',
    //    dataType: 'string',
    //    data: base64string
    //}).done(function (response) {
    //    console.log(response);
    //}).fail(function (error) {
    //    console.log(error);
    //});

    $.getJSON('api/v1/verify/verifi?' + base64string)
        .done(function (data) {
            console.log(data);
        }).fail(function (error) {
            console.log(error);
        });
}