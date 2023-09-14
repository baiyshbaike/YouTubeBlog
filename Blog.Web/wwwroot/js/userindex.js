$(document).ready(function () {
    $('#usersTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        buttons: [
        ],
        language: {
            "sDecimal": ",",
            "sEmptyTable": "No data available in the table",
            "sInfo": "Showing records from _TOTAL_ to _START_ and _END_",
            "sInfoEmpty": "No record",
            "sInfoFiltered": "(_MAX_ found in the registry)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Show _MENU_ record on page",
            "sLoadingRecords": "Loading...",
            "sProcessing": "Processing...",
            "sSearch": "Search:",
            "sZeroRecords": "No matching records found",
            "oPaginate": {
                "sFirst": "First",
                "sLast": "Last",
                "sNext": "Next",
                "sPrevious": "Previous"
            },
            "oAria": {
                "sSortAscending": ": enable ascending column sort",
                "sSortDescending": ": enable descending column sort"
            },
            "select": {
                "rows": {
                    "_": "%d record selected",
                    "0": "",
                    "1": "1 record selected"
                }
            }
        }
    });
});