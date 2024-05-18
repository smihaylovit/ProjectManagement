// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let forms = document.querySelectorAll('.needs-validation');

Array.prototype.slice.call(forms).forEach(function (form) {
    form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }

        form.classList.add('was-validated');
    }, false);
});

let dateFormat = "dd-M-yy";
let options = {
    dateFormat: dateFormat,
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    closeText: "Close",
    firstDay: 1,
    hideIfNoPrevNext: true,
    maxDate: 0
};

let fromDate = $("#fromDate").datepicker(options);
let toDate = $("#toDate").datepicker(options);

fromDate.datepicker("option", "beforeShow", function () {
    let maxValue = toDate.datepicker("getDate");
    maxValue = maxValue == null ? 0 : maxValue;
    return {
        maxDate: maxValue
    }
});

fromDate.datepicker("option", "onSelect", function () {
    setTimeout(function () {
        if (toDate.datepicker("getDate") == null) {
            toDate.datepicker("show");
        } else {
            loadUsersTable();
            loadUsersPerformanceChart();
        }
    });
});

toDate.datepicker("option", "beforeShow", function () {
    let minValue = fromDate.datepicker("getDate");
    return {
        minDate: minValue
    }
});

toDate.datepicker("option", "onSelect", function () {
    setTimeout(function () {
        if (fromDate.datepicker("getDate") == null) {
            fromDate.datepicker("show");
        } else {
            loadUsersTable();
            loadUsersPerformanceChart();
        }
    });
});

google.charts.load('current', { 'packages': ['corechart', 'table', 'bar'] });
google.charts.setOnLoadCallback(function () {
    loadUsersTable();
    loadUsersPerformanceChart();
});

let userToCompareId = 0;

function loadUsersTable() {
    let fromDateValue = fromDate.datepicker("getDate");
    let toDateValue = toDate.datepicker("getDate");
    let pageNumberValue = $("input[name='page-number']:checked").val();
    let data = {
        fromdate: fromDateValue,
        toDate: toDateValue,
        pageNumber: pageNumberValue
    };

    $.ajax({
        type: 'GET',
        url: 'Users/Get',
        data: data,
        beforeSend: function () {
            clearChartsHtml();
            $('#users-container .loader-icon').show();
        },
        success: function (response) {
            if (response.users.length > 0) {
                drawUsersTable(response);
            }
        },
        error: function () {
        },
        complete: function () {
            $('#users-container .loader-icon').hide();
        }
    });
}

function drawUsersTable(response) {
    let rows = [];

    for (let i = 0; i < response.users.length; i++) {
        rows[i] = [response.users[i].id, response.users[i].email, "<input style='color:#dc3545;' type='button' value='Compare' onclick='compareUser(" + response.users[i].id + ")' />"]
    }

    let data = new google.visualization.DataTable();
    data.addColumn('number', 'Id');
    data.addColumn('string', 'Email');
    data.addColumn('string', '');
    data.addRows(rows);
    data.setProperty(0, 0, 'style', 'width:50px');

    let table = new google.visualization.Table(document.getElementById('users-table'));
    table.draw(data, { width: '100%', height: 400, allowHtml: true, cssClassNames: { headerCell: 'normal-whitespace' } });

    let btns = $('#users-pagination-btns').html('');

    if (response.pages > 0) {
        for (let i = 1; i <= response.pages; i++) {
            btns.append("<input value='" + i + "' type='radio' class='btn-check' name='page-number' id='page-" + i + "' autocomplete='off'" + (i == response.selectedPage ? " checked" : "") + ">");
            btns.append("<label class='btn btn-outline-primary' for='page-" + i + "'>" + i + "</label>");
        }

        $("input[name='page-number']").on("change", function () {
            loadUsersPerformanceChart();
            loadUsersTable();
        });
    }
}

function loadUsersPerformanceChart() {
    let fromDateValue = fromDate.datepicker("getDate");
    let toDateValue = toDate.datepicker("getDate");
    let projectIdValue = $("input[name='project-id']:checked").val();
    let data = {
        userToCompareId: userToCompareId,
        fromdate: fromDateValue,
        toDate: toDateValue,
        projectId: projectIdValue
    };

    $.ajax({
        type: 'GET',
        url: 'Users/GetPerformance',
        data: data,
        beforeSend: function () {
            $('#performance-container .loader-icon').show();
        },
        success: function (response) {
            if (response.chartData.length > 0 && response.projects.length > 0) {
                $(".no-data").addClass("d-none");
                $(".no-database").addClass("d-none");
                drawUsersPerformanceChart(response);
            } else {
                clearChartsHtml();
                $(".no-data").removeClass("d-none");
            }
        },
        error: function () {
            clearChartsHtml();
            $(".no-database").removeClass("d-none");
        },
        complete: function () {
            $('#performance-container .loader-icon').hide();
        }
    });
}

function drawUsersPerformanceChart(response) {
    let fromDateValue = $("#fromDate").val();
    let toDateValue = $("#toDate").val();
    let rows = [];
    rows[0] = ['User', 'Hours', { role: 'style' }];

    for (let i = 0; i < response.chartData.length; i++) {
        let color = '#1b9e77';

        if (response.userToCompareChartData &&
            response.chartData[i].email == response.userToCompareChartData.email) {
            color = '#dc3545';
        }

        rows[i + 1] = [response.chartData[i].email, response.chartData[i].hours, color];
    }

    let period = fromDateValue != '' && toDateValue != '' ? ' (' + fromDateValue + ' - ' + toDateValue + ')' : ''; 
    let data = google.visualization.arrayToDataTable(rows);
    let options = {
        title: 'Users Performance For ' + response.selectedProjectName + period,
        bars: 'horizontal',
        hAxis: {
            title: 'Hours',
            format: 'decimal',
            minValue: 0
        },
        height: 400,
        width: '100%',
    };

    let chart = new google.visualization.BarChart(document.getElementById('users-performance-chart'));
    chart.draw(data, options);

    let btns = $('#projects-btns').html('');

    if (response.projects.length > 0) {
        btns.append("<input value='0' type='radio' class='btn-check' name='project-id' id='project-0' autocomplete='off'" + (response.selectedProjectId == 0 ? " checked" : "") + ">");
        btns.append("<label class='btn btn-outline-primary' for='project-0'>All Projects</label>");
        for (let i = 0; i < response.projects.length; i++) {
            btns.append("<input value='" + response.projects[i].id + "' type='radio' class='btn-check' name='project-id' id='project-" + response.projects[i].id + "' autocomplete='off'" + (response.projects[i].id == response.selectedProjectId ? " checked" : "") + ">");
            btns.append("<label class='btn btn-outline-primary' for='project-" + response.projects[i].id + "'>" + response.projects[i].name + "</label>");
        }

        $("input[name='project-id']").on("change", loadUsersPerformanceChart);
    }
}

function clearChartsHtml() {
    $("#users-table").html('');
    $("#users-pagination-btns").html('');
    $("#users-performance-chart").html('');
    $("#projects-btns").html('');
}

function compareUser(userId) {
    userToCompareId = userId;
    loadUsersPerformanceChart();
}

$("#initialize-db-btn").on("click", function () {
    clearChartsHtml();
});