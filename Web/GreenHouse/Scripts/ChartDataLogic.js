function LoadChartData (chartId, chartData, chartTitle)
{
    
    var chartConfig =
    {
        type: 'line',
        data: {
            labels: chartData.Timestamps,
            datasets: chartData.DataSets
    },
    options: {
            responsive: true,
            legend: {
            position: 'bottom',
            },
        hover: {
                mode: 'label'
        },
        scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Time'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Value'
                    }
                }]
        },
        title: {
                display: true,
                text: chartTitle
        }
    }
    };

    
    var ctx = document.getElementById(chartId).getContext("2d");

    var chartObject = new Chart(ctx, chartConfig);

    return { chart: chartObject, config: chartConfig };
}
function LoadAll(callback)
{
    var baseUrl = "/home/";
    var now = baseUrl + "GetSensorDataNow";
    var day = baseUrl + "GetSensorDataDay";
    var week = baseUrl + "GetSensorDataWeek";
    var month = baseUrl + "GetSensorDataMonth";
    var year = baseUrl + "GetSensorDataYear";

    LoadData(now, "Now", "Now", true,callback);
    LoadData(day, "Day",  "Day");
    LoadData(week, "Week","Week");
    LoadData(month, "Month", "Month");
    LoadData(year, "Year", "Year");

}
function LoadData(url, chartId, title, isNow, callback)
{
    $.get(url, function(data) {
        var chartData = LoadChartData(chartId, data, title);
        if (isNow) {
            window.Now = chartData.chart;
            window.NowConfig = chartData.config;
            callback();
        }
    }, "json");
}
