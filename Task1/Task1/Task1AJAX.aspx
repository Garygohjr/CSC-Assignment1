<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Task1AJAX.aspx.cs" Inherits="Task1.Task1AJAX" %>

<!DOCTYPE html>

<html>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>

<body>
    <button style="width: 150px; height: 50px;" onclick="getWeather()">Click me</button>
    <p id="area_metadata">Click button to get data</p>
    <p id="items"></p>
    <p id="api_info"></p>
</body>
<script>
    function getWeather() {
        $.ajax({
            type: "GET",
            url: "https://api.data.gov.sg/v1/environment/2-hour-weather-forecast",
            contentType: "application/json; charset-utf-8",
            dataType: "JSON",
            success: function (data) {
                document.getElementById("area_metadata").innerHTML = "Area metadata: \n" + JSON.stringify(data.area_metadata);
                document.getElementById("items").innerHTML = "Items: \n" + JSON.stringify(data.items);
                document.getElementById("api_info").innerHTML = "Api Info: \n" + JSON.stringify(data.api_info);
            },
            failure: function (response) {
                alert(response.d);
            }
        });
    };
</script>

</html>
