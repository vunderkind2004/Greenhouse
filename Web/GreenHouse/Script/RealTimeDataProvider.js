//signalR
$(function () {
    var hub = currentHub;
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

        setTemperatureColor();
    };   

    $.connection.hub.start();

    
    $('[data-toggle="tooltip"]').tooltip();

    var setTemperatureColor = function () {
        var temperatureTypeId = "1";
        var temperatureSensors = $("[sensor-type-id='" + temperatureTypeId + "']");
        if (temperatureSensors.length == 0)
            return;

        var min;
        var max;
        var maxValue = 255;

        for (var i=0; i < temperatureSensors.length; i++)
        {
            var v = temperatureSensors[i].innerHTML;
            if (v)
            {
                var value = parseFloat(v);
                if (min) {
                    if (value < min)
                        min = value;
                }
                else
                {
                    min = value;
                }
                if (max) {
                    if (value > max)
                        max = value;
                }
                else
                {
                    max = value;
                }
            }
        }

        var def = 200;

        for (var i=0; i < temperatureSensors.length; i++) {
            if (min && min < max) {
                //style.backgroundColor = 'rgb(' + a + ',' + b + ',' + c + ')';
                var v = temperatureSensors[i].innerHTML;
                if (v) {
                    var temperature = parseFloat(v);
                    var dTMax = max - min;
                    if (dTMax == 0) {
                        temperatureSensors[i].parentNode.style.backgroundColor = 'rgb(' + def + ',' + def + ',' + def + ')';
                    }
                    else
                    {
                        var dt = temperature - min;
                        //if (dt == 0) {
                        //    temperatureSensors[i].parentNode.style.backgroundColor = 'rgb(' + 0 + ',' + 0 + ',' + maxValue + ')';
                        //}
                        //else
                        //{
                            //var b = Math.round( maxValue * Math.cos((Math.PI / 2) * (dt / dTMax)));
                        var b = dt / dTMax < 0.5                            
                            ? Math.round(maxValue * 0.75 * Math.cos(Math.PI * (dt / dTMax)))
                            : 0;

                        var r = dt / dTMax > 0.5                            
                            ? Math.round(maxValue * Math.sin(Math.PI * (dt / dTMax - 0.5)))
                            : 0;

                        var g = Math.round( maxValue * Math.cos((Math.PI / 1.35) * (-0.5 + dt / dTMax)));

                            temperatureSensors[i].parentNode.style.backgroundColor = 'rgb(' + r + ',' + g + ',' + b + ')';
                        //}
                    }
                }
                else
                {
                    temperatureSensors[i].parentNode.style.backgroundColor = 'rgb(' + def + ',' + def + ',' + def + ')';
                }

            }
            else {
                temperatureSensors[i].parentNode.style.backgroundColor = 'rgb(' + 100 + ',' + 100 + ',' + 100 + ')';
            }
        }
        
    };

    


});

