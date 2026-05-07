document.addEventListener('click', function(e){
    const toggle = e.target.closest('[data-toggle-password]');
    if(!toggle) return;
    const input = document.querySelector(toggle.getAttribute('data-toggle-password'));
    if(input) input.type = input.type === 'password' ? 'text' : 'password';
});
(function () {
    'use strict';

    if (window.jQuery && window.jQuery.validator) {
        window.jQuery.validator.addMethod('endtimeequals', function (value, element, params) {
            if (!value) {
                return true;
            }

if (window.jQuery && window.jQuery.validator) {
    window.jQuery.validator.addMethod('futuredateonly', function (value, element) {
        if (!value) {
            return true;
        }

        const parts = value.split('-');
        if (parts.length !== 3) {
            return true;
        }

        const year = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10);
        const day = parseInt(parts[2], 10);

        if ([year, month, day].some((num) => Number.isNaN(num))) {
            return true;
        }

        const inputDate = new Date(year, month - 1, day);
        if (Number.isNaN(inputDate.getTime())) {
            return true;
        }

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        return inputDate > today;
    });

    window.jQuery.validator.unobtrusive.adapters.add('futuredateonly', [], function (options) {
        options.rules['futuredateonly'] = true;
        options.messages['futuredateonly'] = options.message;
    });
}

            const $form = window.jQuery(element).closest('form');
            const startName = params.start;
            const slotName = params.slot;

            if (!startName || !slotName) {
                return true;
            }

            const startValue = $form.find(`[name="${startName}"]`).val();
            const slotValue = $form.find(`[name="${slotName}"]`).val();

            if (!startValue || !slotValue) {
                return true;
            }

            const slotMinutes = parseInt(slotValue, 10);
            if (Number.isNaN(slotMinutes) || slotMinutes <= 0) {
                return true;
            }

            const parseTime = (val) => {
                const parts = val.split(':');
                if (parts.length < 2) return null;
                const hours = parseInt(parts[0], 10);
                const minutes = parseInt(parts[1], 10);
                if (Number.isNaN(hours) || Number.isNaN(minutes)) return null;
                return hours * 60 + minutes;
            };

            const startMinutes = parseTime(startValue);
            const endMinutes = parseTime(value);

            if (startMinutes === null || endMinutes === null) {
                return true;
            }

            return startMinutes + slotMinutes === endMinutes;
        });

        window.jQuery.validator.unobtrusive.adapters.add('endtimeequals', ['start', 'slot'], function (options) {
            options.rules['endtimeequals'] = {
                start: options.params.start,
                slot: options.params.slot
            };
            options.messages['endtimeequals'] = options.message;
        });
    }
})();
