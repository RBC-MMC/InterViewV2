var tomSelectInstances = {};
var table;
$(document).ready(function () {

    SetSideMenuActive();

    if ($('.select-search').length > 0) {
        $('.select-search').each(function (index, element) {
            var selectInstance = new TomSelect(element, {
                create: false,
                allowEmptyOption: true,
            });
            tomSelectInstances[element.id] = selectInstance;
        });
    }


    if ($('.dynamic-select-search').length > 0) {
        $('.dynamic-select-search').each(function () {
            new TomSelect($(this), {
                create: true,
                allowEmptyOption: true,
                //sortField: {
                //    field: "text",
                //    direction: "asc"
                //}
            });
        });
    }
    if ($('.dynamic-select-serach-multiple').length > 0) {
        $('.dynamic-select-serach-multiple').each(function () {
            new TomSelect($(this), {
                create: true,
                persist: false,
                allowEmptyOption: true,
                createOnBlur: true,
            });
        });
    }

    if ($('.select-serach-multiple').length > 0) {
        $('.select-serach-multiple').each(function () {
            new TomSelect($(this), {
                persist: false,
                allowEmptyOption: true,
                createOnBlur: true,
                create: false
            });
        });
    }

    $(".datepicker").flatpickr({
        enableTime: false,
        dateFormat: "d/m/Y",
    });
    $("#monthlyReport").flatpickr({
        plugins: [
            new monthSelectPlugin({
                shorthand: false,
                dateFormat: "m/Y",
                altFormat: "F Y",
                altInput: true
            })
        ],
        altInput: true,
        defaultDate: new Date(),
        minDate: "2024-08",
    });

    $(".monthpicker").flatpickr({
        plugins: [
            new monthSelectPlugin({
                shorthand: false,
                dateFormat: "m/Y",
                altFormat: "F Y",
                altInput: true
            })
        ],
        altInput: true,
        defaultDate: new Date()
    });
    $(".timepicker").flatpickr({
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true
    });

    var table = $('table.remote-data-table').DataTable({
        "dom": "<'dt--top-section'<'row'<'col-12 col-sm-6 d-flex justify-content-sm-start justify-content-center'l><'col-12 col-sm-6 d-flex justify-content-sm-end justify-content-center mt-sm-0 mt-3'f>>>" +
            "<'dt--pages-count  mb-sm-0 mb-1'i>"+
            "<'table-responsive'tr>" +
            "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>",
        language: {
            search: "<div class=\"row\" style=\"display:filex;width:auto\">" +
                "_INPUT_" +
                "<div class=\"dt-search-column\">" +
                "<select id=\"dt-col-search\"><option value=\"-1\" selected>All</option></select>" +
                "<\/div>" +
                //"<div class=\"dt-visible-column\">" +
                //"<select id=\"dt-col-visible\" class=\"select-search\" multiple> </select>" +
                //"<\/div>" +
                "<\/div>",
            sSearchPlaceholder: "Search...",
            lengthMenu: '<span>Show:</span> _MENU_',
            "sLengthMenu": "Results :  _MENU_ ",
            "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
            "oPaginate": {
                "sPrevious": '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-left"><line x1="19" y1="12" x2="5" y2="12"></line><polyline points="12 19 5 12 12 5"></polyline></svg>',
                "sNext": '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-right"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>'
            },
        },
        processing: true,
        stateSave: true,
        "stripeClasses": [],
        "lengthMenu": [25, 50, 100, 150, 250, 500],
        "pageLength": 50,
        scrollCollapse: true,
        //scrollY: 500,
        //scrollX: true, 
        order: []
    });
    var searchInput = $('#DataTables_Table_0_filter').find('input[type="search"]');

    //var savedSearch = localStorage.getItem('dataTableSearch');
    //if (savedSearch) {
    //    table.search(savedSearch).draw();
    //}

    table.columns().every(function () {
        var column = this;
        var columnIndex = column.index();
        var columnHeader = $(column.header()).text();
        var isSearched = $(column.header()).hasClass('dt-not-search');
        var isVisibled = $(column.header()).hasClass('dt-not-visible');

        if (!isSearched) {
            $('#dt-col-search').append('<option value="' + columnIndex + '">' + columnHeader + '</option>');
        }
        if (!isVisibled) {
            if (column.visible()) {
                $('#dt-col-visible').append('<option seleted value="' + columnIndex + '">' + columnHeader + '</option>');
            }
            else {
                $('#dt-col-visible').append('<option value="' + columnIndex + '">' + columnHeader + '</option>');
            }
        }

    });
    $('#dt-col-search').on('change', function () {
        var selectedColumn = $(this).val();
        var searchTerm = $(searchInput).val();
        if (selectedColumn == '-1') {
            table.columns().search('');
            table.search(searchTerm).draw();
            $(searchInput).attr("placeholder", "Search...")

        } else {
            table.columns().search('');
            table.column(selectedColumn).search(searchTerm).draw();
            var text = $('#dt-col-search option:selected').text();
            $(searchInput).attr("placeholder", "Search by " + text + "...");
        }

    });
    $(searchInput).on('keyup', function () {
        var searchTerm = $(this).val();
        var selectedColumn = $('#dt-col-search').val();
        if (selectedColumn == '-1') {
            table.search(searchTerm).draw();
        } else {
            table.columns().search('');
            table.column(selectedColumn).search(searchTerm).draw();
        }
        //localStorage.setItem('dataTableSearch', searchTerm);
    });





    var validator = $('.form-validate').validate({
        ignore: 'input[type=hidden], .select2-search__field',
        errorClass: 'validation-invalid-label text-danger',
        successClass: 'validation-valid-label text-success',
        validClass: 'validation-valid-label text-success',
        highlight: function (element, errorClass) {
            $(element).removeClass(errorClass);
        },
        unhighlight: function (element, errorClass) {
            $(element).removeClass(errorClass);
        },
        success: function (label) {
            label.addClass('validation-valid-label').text('Success.');
        },

        errorPlacement: function (error, element) {

            if (element.parents().hasClass('form-check')) {
                error.appendTo(element.parents('.form-check').parent().addClass('text-danger'));
            }

            else if (element.parents().hasClass('form-group-feedback') || element.hasClass('select2-hidden-accessible')) {
                error.appendTo(element.parent().addClass('text-danger'));
            }

            else if (element.parent().is('.uniform-uploader, .uniform-select') || element.parents().hasClass('input-group')) {
                error.appendTo(element.parent().parent().addClass('text-danger'));
            }

            else {
                error.insertAfter(element);
            }
        },
        rules: {
            
        },
        messages: {
           
        }
    });

    $('input[required], select[required], textarea[required]').each(function () {
        var name = $(this).attr('name');
        if (name && validator) {
            validator.settings.rules[name] = {
                required: true
            };
        }
    });

});


function customConfirm(title, message) {
    return new Promise(function (resolve, reject) {
        if (title !== '' && title !== undefined) {
            $('#confirmModalLabel').text(title);
        }
        if (message !== '' && message !== undefined) {
            $('#confirmModalMessage').html(message);
        } else {
            $('#confirmModalMessage').hide();
        }

        $('#confirmModal-btn').trigger('click');

        $('#confirmModal-confirm-btn').one('click', function () {
            $('#confirmModal').modal('hide');
            resolve(true);
        });

        $('#confirmModal').one('hidden.bs.modal', function () {
            resolve(false);
        });
    });
}


var isActived = false
function SetSideMenuActive() {
    var currentUrl = window.location.pathname;
    $('#accordionExample a').each(function () {
        var href = $(this).attr('href');
        if (href === currentUrl) {
            $('#accordionExample li.active').removeClass('active');
            $(this).closest('li').addClass('active');
            $(this).closest('li.menu').addClass('active');
            isActived = true
            return;
        }
        if (!isActived) {
            var splittedUrl = currentUrl.split('/');
            var main = '/' + splittedUrl[1];
            if (href === main) {
                $('#accordionExample li.active').removeClass('active');
                $(this).closest('li').addClass('active');
                $(this).closest('li.menu').addClass('active');
            }
            return;
        }
    });
}