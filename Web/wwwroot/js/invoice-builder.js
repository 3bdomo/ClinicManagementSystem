document.addEventListener('click', function(e){
    const add = e.target.closest('[data-add-invoice-row]');
    const remove = e.target.closest('[data-remove-invoice-row]');
    const list = document.querySelector('[data-invoice-rows]');
    if(add && list){
        const row = document.createElement('div');
        row.className = 'invoice-row';
        row.innerHTML = '<input class="clinic-input" placeholder="Service"/><input class="clinic-input" type="number" placeholder="Qty"/><input class="clinic-input" type="number" placeholder="Price"/><button type="button" data-remove-invoice-row class="px-3 py-2 rounded-lg border">Remove</button>';
        list.appendChild(row);
    }
    if(remove){ remove.closest('.invoice-row')?.remove(); }
});
