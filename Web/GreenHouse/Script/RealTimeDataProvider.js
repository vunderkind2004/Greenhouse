//signalR
$(function () {
    var hub = $.connection.sensorDataHub;
    hub.client.send = function (message) {
        console.log(message);
    }

    hub.client.updateSensorValues = function (data) {
        //console.log("update data");

        for (var i = 0; i < data.length; i++)
        {
            var sensorInfo = data[i];

            var sensorId = sensorInfo.SensorId;
            var sensorValue = sensorInfo.Value;

            $("#" + sensorId).html(sensorValue);
        }
    };   

    $.connection.hub.start();

    
    $('[data-toggle="tooltip"]').tooltip();
    

    });