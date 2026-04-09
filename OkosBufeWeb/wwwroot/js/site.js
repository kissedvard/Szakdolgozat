// Kosár darabszámának frissítése az Étlapon
function updateQuantity(productId, operation) {
    fetch(`/Cart/UpdateCartAjax?id=${productId}&operation=${operation}`)
        .then(response => response.json())
        .then(data => {
            if (data.quantity === 0) {
                // Ha 0 lett a darabszám, frissítjük az oldalt, 
                // hogy visszaváltozzon a sima "Kosárba teszem" gombra
                location.reload();
            } else {
                // Különben csak villámgyorsan átírjuk a számot a képernyőn
                document.getElementById(`qty-${productId}`).innerText = data.quantity + ' db';
            }
        })
        .catch(error => console.error('Hiba történt a kosár frissítésekor:', error));
}

// Felugró zöld üzenet automatikus eltüntetése 
document.addEventListener("DOMContentLoaded", function() {
    let sikerUzenet = document.getElementById('toast-alert');
    
    if (sikerUzenet) {
        setTimeout(function() {
            sikerUzenet.classList.remove('toast-appear-animacio');
            sikerUzenet.classList.add('toast-disappear-animacio');
            
            setTimeout(function() {
                sikerUzenet.remove();
            }, 500);
        }, 3000); 
    }
});