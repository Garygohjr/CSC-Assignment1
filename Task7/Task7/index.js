var clarifaiApiKey = '';
var workflowId = '';
var imageData = '';
var keys;
$(document).ready(function () {
    $.getJSON("./task7Keys.json", function (data) {
        keys = data;
        console.log(keys)
    });
});
// Handles image upload
function uploadImage() {
    console.log(keys);
    var app = new Clarifai.App({
        apiKey: keys.generalKey.key
    });
    var preview = document.getElementById('img');
    var file = document.querySelector('input[type=file]').files[0];
    var reader = new FileReader();
    $(".analysis").html("");
    $(".cost").html("");
    document.getElementById("loader").style.display = "unset";
    preview.style.display = "none";
    reader.addEventListener("load", function () {
        imageData = reader.result;
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
        app.models.initModel({ id: keys.receiptKey.key, version: keys.receiptKey.version })
            .then(generalModel => {
                return generalModel.predict(imageData);
            })
            .then(
                function (response) {
                    var concepts = response['outputs'][0]['data']['concepts'];
                    if (concepts[0].value > 0.5) {
                        $(".analysis").html("<p> is probably a receipt </p>");
                        receiptAmount();
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

function receiptAmount() {
    var receiptApp = new Clarifai.App({
        apiKey: keys.visualKey.key
    });
    receiptApp.workflow.predict(keys.visualKey.workflowid, { base64: imageData }).then(
        function (response) {
            var cost = 'Total cost is probably:'
            var outputs = response.results[0].outputs[2].data.regions;
            outputs.forEach(function (output) {
                if (output.data.text.raw.includes('.')){
                    cost += "<br>" + output.data.text.raw;
                }
            })
            $(".cost").html(cost);
        },
        function (err) {
            console.log(err)
        }
    )
}