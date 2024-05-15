// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
google.charts.load('current', { 'packages': ['table', 'bar'] });
google.charts.setOnLoadCallback(function () {
    $("#fromDate, #toDate").on("change", function () {
        loadUsersTable();
        loadUsersPerformanceChart();
    });

    loadUsersTable();
    loadUsersPerformanceChart();
});

function loadUsersTable() {
    let fromDateValue = $("#fromDate").val();
    let toDateValue = $("#toDate").val();
    let pageNumberValue = $('input[name="users-pagination"]:checked').val();
    let data = {
        fromdate: fromDateValue,
        toDate: toDateValue,
        pageNumber: pageNumberValue
    };

    $.ajax({
        url: 'Users/Get',
        type: 'GET',
        data: data,
    }).done(drawUsersTable);
}

function drawUsersTable(response) {
    let rows = [];

    for (let i = 0; i < response.users.length; i++) {
        rows[i] = [response.users[i].id, response.users[i].email]
    }

    let data = new google.visualization.DataTable();
    data.addColumn('number', 'Id');
    data.addColumn('string', 'Email');
    data.addRows(rows);
    data.setProperty(0, 0, 'style', 'width:50px');

    let table = new google.visualization.Table(document.getElementById('users-table'));
    table.draw(data, { width: '100%', height: 400, allowHtml: true, cssClassNames: { headerCell: 'normal-whitespace' } });

    let btns = $('#users-pagination-btns').html('');

    if (response.pages > 0) {
        for (let i = 1; i <= response.pages; i++) {
            btns.append("<input value='" + i + "' type='radio' class='btn-check' name='users-pagination' id='page-" + i + "' autocomplete='off'" + (i == response.selectedPage ? " checked" : "") + ">");
            btns.append("<label class='btn btn-outline-primary' for='page-" + i + "'>" + i + "</label>");
        }

        $("input[name=users-pagination]").on("change", loadUsersTable);
    }
}

function loadUsersPerformanceChart() {
    let fromDateValue = $("#fromDate").val();
    let toDateValue = $("#toDate").val();
    let projectIdValue = $('input[name="user-or-project"]:checked').val();
    let data = {
        fromdate: fromDateValue,
        toDate: toDateValue,
        projectId: projectIdValue
    };

    $.ajax({
        url: 'Users/GetPerformance',
        type: 'GET',
        data: data,
    }).done(drawUsersPerformanceChart);
}

function drawUsersPerformanceChart(response) {
    let fromDateValue = $("#fromDate").val();
    let toDateValue = $("#toDate").val();
    let rows = [];
    rows[0] = ['User', 'Hours'];

    for (let i = 0; i < response.chartData.length; i++) {
        rows[i + 1] = [response.chartData[i].email, response.chartData[i].hours]
    }

    let data = google.visualization.arrayToDataTable(rows);
    let options = {
        chart: {
            title: 'Users Performance For ' + response.selectedProjectName,
            subtitle: fromDateValue + ' - ' + toDateValue,
        },
        bars: 'horizontal',
        hAxis: { format: 'decimal' },
        height: 400,
        width: '100%',
        colors: ['#1b9e77', '#d95f02', '#7570b3']
    };

    let chart = new google.charts.Bar(document.getElementById('users-performance-chart'));
    chart.draw(data, google.charts.Bar.convertOptions(options));

    let btns = $('#user-or-project-btns').html('');

    if (response.projects.length > 0) {
        for (let i = 0; i <= response.projects.length; i++) {
            btns.append("<input value='" + i + "' type='radio' class='btn-check' name='user-or-project' id='project-" + i + "' autocomplete='off'" + (i == response.selectedProjectId ? " checked" : "") + ">");
            btns.append("<label class='btn btn-outline-primary' for='project-" + i + "'>" + (i == 0 ? "All Projects" : response.projects[i - 1].name) + "</label>");
        }

        $("input[name=user-or-project]").on("change", loadUsersPerformanceChart);
    }
}