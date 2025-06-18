document.addEventListener('DOMContentLoaded', function () {

    // Date variable
    var newDate = new Date();

    /** 
     * 
     * @getDynamicMonth() fn. is used to validate 2 digit number and act accordingly 
     * 
    */
    function getDynamicMonth() {
        getMonthValue = newDate.getMonth();
        _getUpdatedMonthValue = getMonthValue + 1;
        if (_getUpdatedMonthValue < 10) {
            return `0${_getUpdatedMonthValue}`;
        } else {
            return `${_getUpdatedMonthValue}`;
        }
    }

    // Modal Elements
    var calendarsEvents = {
        Planned: 'primary',
        InProgress: 'warning',
        Completed: 'success',
        Cancelled: 'danger',
    }

    // Calendar Elements and options
    var calendarEl = document.querySelector('.calendar');
    var deatisLink = document.getElementById(`event-details-link`);
    var deleteLink = document.getElementById(`event-delete-link`);
    var editLink = document.getElementById(`event-edit-link`);
    var getModalStatusInputEl = document.getElementById(`event-status`);
    var checkWidowWidth = function () {
        if (window.innerWidth <= 1199) {
            return true;
        } else {
            return false;
        }
    }

    var calendarHeaderToolbar = {
        left: 'prev next today',
        center: 'title',
        right: 'dayGridMonth,listWeek'
    }
    var calendarEventsList = [
        {
            id: 1,
            title: 'Planned POB change',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-03`,
            extendedProps: { calendar: 'Planned', transportMode: 'Supply Boat' }
        },
        {
            id: 22,
            title: 'Planned POB change',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-05`,
            extendedProps: { calendar: 'Planned', transportMode: 'Supply Boat' }
        },
        {
            id: 333,
            title: 'Planned POB change',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-07`,
            extendedProps: { calendar: 'Completed', transportMode: 'Helicopter' }
        },
        {
            id: 444,
            title: 'Emergency Evac',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-16`,
            extendedProps: { calendar: 'Cancelled', transportMode: 'Fast Crew Boat' }
        },
        {
            id: 5555,
            title: 'Planned POB change',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-11`,
            extendedProps: { calendar: 'InProgress', transportMode: 'Supply Boat' }
        },
        {
            id: 666666,
            title: 'Planned POB change',
            start: `${newDate.getFullYear()}-${getDynamicMonth()}-13`,
            extendedProps: { calendar: 'InProgress', transportMode: 'Fast Crew Boat' }
        },
        //{
        //    id: 10,
        //    title: 'Emergency Evac',
        //    url: 'http://google.com/',
        //    start: `${newDate.getFullYear()}-${getDynamicMonth()}-28`,
        //    extendedProps: { calendar: 'Completed', transportMode: 'Helicopter' }
        //}
    ]

    // Calendar Select fn.
    //var calendarSelect = function(info) {
    //    getModalAddBtnEl.style.display = 'block';
    //    getModalUpdateBtnEl.style.display = 'none';
    //    myModal.show()
    //    getModalStartDateEl.value = info.startStr;
    //    getModalEndDateEl.value = info.endStr;
    //}

    // Calendar AddEvent fn.
    //var calendarAddEvent = function () {
    //    var url = '/POBChange/Create';
    //    window.open(url);
    //}

    // Calendar eventClick fn.
    var calendarEventClick = function (info) {
        var eventObj = info.event;
        if (eventObj.url) {
            window.open(eventObj.url);
            info.jsEvent.preventDefault();
        } else {
            var getModalEventId = eventObj._def.publicId;
            var getModalEventLevel = eventObj._def.extendedProps['calendar'];
            
            if (getModalEventLevel === 'InProgress') {
                getModalStatusInputEl.value = 'In-Progress';
            }
            else {
                getModalStatusInputEl.value = getModalEventLevel;
            }
            document.getElementById(`event-id`).value = getModalEventId;
            document.getElementById(`event-type`).value = eventObj.title;
            document.getElementById(`event-transportMode`).value = eventObj._def.extendedProps['transportMode'];
            deatisLink.href += getModalEventId;
            editLink.href += getModalEventId;
            deleteLink.href += getModalEventId;

            myModal.show();
        }
    }


    // Activate Calender    
    var calendar = new FullCalendar.Calendar(calendarEl, {
        selectable: false,
        height: checkWidowWidth() ? 400 : 652,
        initialView: checkWidowWidth() ? 'listWeek' : 'dayGridMonth',
        initialDate: `${newDate.getFullYear()}-${getDynamicMonth()}-07`,
        headerToolbar: calendarHeaderToolbar,
        events: calendarEventsList,
        //select: function () {
        //    alert("salamm")
        //},
        unselect: function () {
            console.log('unselected')
        },
        //customButtons: {
        //    addEventButton: {
        //        text: 'Add POB Change',
        //        click: calendarAddEvent
        //    }
        //},
        eventClassNames: function ({ event: calendarEvent }) {
            const getColorValue = calendarsEvents[calendarEvent._def.extendedProps.calendar];
            return [
                'event-fc-color fc-bg-' + getColorValue
            ];
        },

        eventClick: calendarEventClick,
        windowResize: function (arg) {
            if (checkWidowWidth()) {
                calendar.changeView('listWeek');
                calendar.setOption('height', 900);
            } else {
                calendar.changeView('dayGridMonth');
                calendar.setOption('height', 1052);
            }
        }
    });

    //// Add Event
    //getModalAddBtnEl.addEventListener('click', function () {

    //    var getModalCheckedRadioBtnEl = document.querySelector('input[name="event-level"]:checked');

    //    var getTitleValue = getModalTitleEl.value;
    //    var setModalStartDateValue = getModalStartDateEl.value;
    //    var setModalEndDateValue = getModalEndDateEl.value;
    //    var getModalCheckedRadioBtnValue = (getModalCheckedRadioBtnEl !== null) ? getModalCheckedRadioBtnEl.value : '';

    //    calendar.addEvent({
    //        id: uuidv4(),
    //        title: getTitleValue,
    //        start: setModalStartDateValue,
    //        end: setModalEndDateValue,
    //        allDay: true,
    //        extendedProps: { calendar: getModalCheckedRadioBtnValue }
    //    })
    //    myModal.hide()
    //})



    //// Update Event
    //getModalUpdateBtnEl.addEventListener('click', function () {
    //    var getPublicID = this.dataset.fcEventPublicId;
    //    var getTitleUpdatedValue = getModalTitleEl.value;
    //    var getEvent = calendar.getEventById(getPublicID);
    //    var getModalUpdatedCheckedRadioBtnEl = document.querySelector('input[name="event-level"]:checked');

    //    var getModalUpdatedCheckedRadioBtnValue = (getModalUpdatedCheckedRadioBtnEl !== null) ? getModalUpdatedCheckedRadioBtnEl.value : '';

    //    getEvent.setProp('title', getTitleUpdatedValue);
    //    getEvent.setExtendedProp('calendar', getModalUpdatedCheckedRadioBtnValue);
    //    myModal.hide()
    //})

    calendar.render();

    var myModal = new bootstrap.Modal(document.getElementById('exampleModal'))
    //var modalToggle = document.querySelector('.fc-addEventButton-button ')

    document.getElementById('exampleModal').addEventListener('hidden.bs.modal', function (event) {
        deatisLink.href = deatisLink.href.substring(0, deatisLink.href.lastIndexOf("/") + 1);
        editLink.href = editLink.href.substring(0, editLink.href.lastIndexOf("/") + 1);
        deleteLink.href = deleteLink.href.substring(0, deleteLink.href.lastIndexOf("/") + 1);
        document.getElementById(`event-status`).value = '';
        document.getElementById(`event-type`).value = '';
        document.getElementById(`event-transportMode`).value = '';
    })
});