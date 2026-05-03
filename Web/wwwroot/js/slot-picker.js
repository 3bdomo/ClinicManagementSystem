document.addEventListener('DOMContentLoaded', function(){
    const doctor = document.querySelector('#DoctorId,[name="DoctorId"]');
    const date = document.querySelector('#AppointmentDate,[name="AppointmentDate"]');
    const container = document.querySelector('#availableSlots,[data-slot-container]');
    const hidden = document.querySelector('#SelectedSlot,[name="SelectedSlot"]');

    async function loadSlots(){
        if(!container || !doctor?.value || !date?.value) return;
        container.innerHTML = '<p class="text-sm text-slate-500">Loading slots...</p>';
        try{
            const res = await fetch(`/Appointment/GetAvailableSlots?doctorId=${encodeURIComponent(doctor.value)}&date=${encodeURIComponent(date.value)}`);
            const slots = res.ok ? await res.json() : [];
            const fallback = ['09:00','09:30','10:00','10:30','11:00','11:30','12:00','12:30'];
            render(slots.length ? slots : fallback);
        }catch{
            render(['09:00','09:30','10:00','10:30','11:00','11:30','12:00','12:30']);
        }
    }
    function render(slots){
        container.innerHTML = '';
        slots.forEach(slot => {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.textContent = typeof slot === 'string' ? slot : (slot.time || slot.start || slot.label);
            btn.className = 'slot-button rounded-lg border border-outline-variant bg-white px-4 py-2 font-bold hover:bg-blue-50';
            btn.addEventListener('click', () => {
                container.querySelectorAll('.slot-button').forEach(b => b.classList.remove('is-selected'));
                btn.classList.add('is-selected');
                if(hidden) hidden.value = btn.textContent;
            });
            container.appendChild(btn);
        });
    }
    doctor?.addEventListener('change', loadSlots);
    date?.addEventListener('change', loadSlots);
});
