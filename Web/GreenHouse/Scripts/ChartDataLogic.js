function LoadChartData (chartId, chartObject, chartData, chartTitle, chartConfig)
{
    
    chartConfig =
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

    chartObject = new Chart(ctx, chartConfig);
}
function LoadAll(config)
{
    var baseUrl = "/home/";
    var now = baseUrl + "GetSensorDataNow";
    var day = baseUrl + "GetSensorDataDay";
    var week = baseUrl + "GetSensorDataWeek";
    var month = baseUrl + "GetSensorDataMonth";
    var year = baseUrl + "GetSensorDataYear";

    LoadData(now, "Now", window.Now, "Now",config);
    LoadData(day, "Day", window.Day, "Day");
    LoadData(week, "Week", window.Week, "Week");
    LoadData(month, "Month", window.Month, "Month");
    LoadData(year, "Year", window.Year, "Year");

}
function LoadData(url, chartId, chartObject, title, config)
{
    $.get(url, function (data) { LoadChartData(chartId, chartObject, data, title, config); }, "json");
}
