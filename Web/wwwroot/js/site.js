document.addEventListener('click', function(e){
    const toggle = e.target.closest('[data-toggle-password]');
    if(!toggle) return;
    const input = document.querySelector(toggle.getAttribute('data-toggle-password'));
    if(input) input.type = input.type === 'password' ? 'text' : 'password';
});
