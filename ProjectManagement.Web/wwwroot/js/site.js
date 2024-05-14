// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    google.charts.load('current', { 'packages': ['table', 'bar'] });
    google.charts.setOnLoadCallback(function () {
        drawUsersTable(1);
        drawChartBar("user");
    });

    $("#fromDate, #toDate").on("change", function () {
        drawUsersTable(1);
        let userOrProjectValue = $('input[type=radio][name="user-or-project"]:checked').val();
        if (userOrProjectValue == 'user') {
            drawChartBar(userOrProjectValue);
        } else {
            $('input[name="user-or-project"]').trigger("change");
        }
    });
    $('input[name="user-or-project"]').on("change", function () {
        let userOrProjectValue = $('input[type=radio][name="user-or-project"]:checked').val();
        drawChartBar(userOrProjectValue);
    });

    function drawUsersTable(pageNumberValue) {
        let fromDateValue = $("#fromDate").val();
        let toDateValue = $("#toDate").val();

        $.ajax({
            async: false,
            url: 'Users/Get',
            type: 'GET',
            data: { fromdate: fromDateValue, toDate: toDateValue, pageNumber: pageNumberValue },
        }).done(function (response) { loadUsersTable(response, pageNumberValue); });
    }

    function drawChartBar(userOrProjectValue) {
        let fromDateValue = $("#fromDate").val();
        let toDateValue = $("#toDate").val();

        $.ajax({
            async: false,
            url: 'Users/GetBarChart',
            type: 'GET',
            data: { fromdate: fromDateValue, toDate: toDateValue, userOrProject: userOrProjectValue },
        }).done(function (response) { loadChartBar(response, userOrProjectValue); });
    }

    function loadChartBar(response, userOrProjectValue) {
        let fromDateValue = $("#fromDate").val();
        let toDateValue = $("#toDate").val();

        let rows = [];
        rows[0] = ['User', 'Hours'];

        for (let i = 0; i < response.length; i++) {
            rows[i + 1] = [response[i].email, response[i].hours]
        }

        let data = google.visualization.arrayToDataTable(rows);

        let options = {
            chart: {
                title: 'User Performance',
                subtitle: fromDateValue + '-' + toDateValue,
            },
            bars: 'horizontal',
            hAxis: { format: 'decimal' },
            height: 400,
            width: '100%',
            colors: ['#1b9e77', '#d95f02', '#7570b3']
        };

        let chart = new google.charts.Bar(document.getElementById('user-performance-chart'));
        chart.draw(data, google.charts.Bar.convertOptions(options));
    }

    function loadUsersTable(response, pageNumberValue) {
        let data = new google.visualization.DataTable();
        let rows = [];

        for (let i = 0; i < response.length; i++) {
            rows[i] = [response[i].id, response[i].email]
        }

        data.addColumn('number', 'Id');
        data.addColumn('string', 'Email');
        data.addRows(rows);

        let table = new google.visualization.Table(document.getElementById('users-table'));
        table.draw(data, { width: '100%', height: '100%' });

        $('#pages').bootpag({
            total: rows.length,
            page: pageNumberValue,
            maxVisible: 3,
            leaps: true,
            firstLastUse: true,
            first: '←',
            last: '→',
            wrapClass: 'pagination',
            activeClass: 'active',
            disabledClass: 'disabled',
            nextClass: 'next',
            prevClass: 'prev',
            lastClass: 'last',
            firstClass: 'first'
        }).on("page", function (event, pageNumber) {
            $("#pageNumber").html("Page " + pageNumber);
            drawUsersTable(pageNumber);
        });
    }
});