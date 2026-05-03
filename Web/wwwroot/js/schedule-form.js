document.addEventListener('DOMContentLoaded', function(){
    const type = document.querySelector('[name="ScheduleType"], #ScheduleType');
    const dayBox = document.querySelector('[data-schedule-day]');
    const dateBox = document.querySelector('[data-schedule-date]');
    function refresh(){
        const value = (type?.value || '').toLowerCase();
        if(dayBox) dayBox.style.display = value.includes('weekly') || value.includes('day') ? '' : 'none';
        if(dateBox) dateBox.style.display = value.includes('specific') || value.includes('date') ? '' : 'none';
    }
    type?.addEventListener('change', refresh);
    refresh();
});
