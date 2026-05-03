document.addEventListener('DOMContentLoaded', function(){
    const el = document.getElementById('fullCalendar');
    if(!el || !window.FullCalendar) return;
    const calendar = new FullCalendar.Calendar(el, {
        initialView: 'dayGridMonth',
        events: '/Appointment/GetCalendarEvents',
        eventDidMount: info => {
            if(info.event.extendedProps.type === 'Operation') info.el.classList.add('operation-event');
        }
    });
    calendar.render();
});
