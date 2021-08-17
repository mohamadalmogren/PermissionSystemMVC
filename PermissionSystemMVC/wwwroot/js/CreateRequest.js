function changeToTime() {
    $("#ToTime").val($("#FromTime").val());
}
function changeFromTime() {
    if ($("#ToTime").val() < $("#FromTime").val()) {

        $("#FromTime").val($("#ToTime").val());
    }
}

function convertTime(FullTime) {
    var hours = (FullTime / 60);
    var rhours = Math.floor(hours);
    var minutes = (hours - rhours) * 60;
    var rminutes = Math.round(minutes);

    var time = rhours + ":" + rminutes;
    return (time);
}

function calculateTime() {
    var Fromtime = $("#FromTime").val();
    var Totime = $("#ToTime").val();
    var perType = $("#PrmisssionType").val();


    var FulllTime = Totime - Fromtime;
    var hours = (FulllTime / 60);
    var rhours = Math.floor(hours);
    var minutes = (hours - rhours) * 60;
    var rminutes = Math.round(minutes);

    var time = rhours + ":" + rminutes;
    var texttime = time;

    if (Fromtime > Totime) {
        $("#ErrorTime").text("To Time most be Greater than From Time!");

    } else {
        $("#TotalTime").text("Total Time is : " + texttime);
        $("#ErrorTime").text(null);

    }

    if (FulllTime > 180) {
        $("#TotalTime").text("Total Time is : " + texttime);

        if (perType == 1) {
            $("#ErrorTime").text("Total Time most be less than or equls 3 Hours for Personal !");
        }
    }
}

$(document).ready(function () {

    var req = $.ajax({
        "url": '/Employee/GetRequestsForCreate',
        "type": 'get',
        "datatype": 'json',
        success: function (data) {
            list = data.list;
            var totalMinutes = 0;
            list.forEach(function (value, index, arr) {
                var minutes = parseInt(value.toTime) - parseInt(value.fromTime);
                totalMinutes = totalMinutes + minutes;

            });
            $('#TotalRequestHours').append('Total time request in this month for personal prmisssion : <b>'
                + convertTime(totalMinutes) + ' Hours</b>');
            $('#oldRequestsDiv').append('<p> Total personal Requests  in this month is:  <b>'
                + data.numberOfRequests + '</b> </p>');
        }
    });

    //FromTime ToTime values AND tests
    var startHour = 7;
    var endHour = 19;
    var cuurentHour = startHour;

    var coounter = 0;
    for (j = 420; j <= 1140; j = j + 15) {
        var timeStr = "";
        switch (coounter) {
            case 0:
                timeStr = cuurentHour + ":00";
                break;
            case 1:
                timeStr = cuurentHour + ":15";

                break;
            case 2:
                timeStr = cuurentHour + ":30";

                break;
            case 3:
                timeStr = cuurentHour + ":45";
                cuurentHour++;
                coounter = -1;
                break;

            default:
                break;
        }
        coounter++;

        var opt = document.createElement('option');
        opt.value = j;
        opt.innerHTML = timeStr;

        document.getElementById("FromTime").appendChild(opt);
        document.getElementById("ToTime").appendChild(opt.cloneNode(true));
    }
})