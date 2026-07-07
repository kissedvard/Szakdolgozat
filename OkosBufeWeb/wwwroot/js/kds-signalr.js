// Összekapcsolódunk a szerver hubjával
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .withAutomaticReconnect() 
    .build();

// Büfé nézet, új rendelés érkezett
connection.on("UpdateOrderDisplay", function () {
    
    // Lekérjük a frissített cetliket tartalmazó HTML-t a Controllertől
    // Mivel itt nem működik a Razor, fixen megadjuk az útvonalat:
    fetch('/Admin/GetActiveOrdersPartial')
        .then(response => response.text())
        .then(html => {
            document.getElementById("ordersContainer").innerHTML = html;
            
            console.log("Új rendelés érkezett, a kijelző frissítve!");
        })
        .catch(error => console.error("Hiba a frissítés során:", error));
});

// Vásárlói nézet, rendelés elkészült
connection.on("OrderCompleted", function (completedOrderId) {
    
    // Megkeressük az adott rendelés státusz jelvényét
    const badge = document.getElementById("status-badge-" + completedOrderId);
    
    // Csak akkor módosítjuk, ha a felhasználó épp a saját Profil oldalán van
    // és látja ezt a konkrét rendelést
    if (badge) {
        badge.innerText = "Kész";
        badge.classList.remove("bg-warning", "text-dark");
        badge.classList.add("bg-success", "text-white");
        
        // Egy kis figyelemfelkeltő animáció
        badge.style.transform = "scale(1.2)";
        setTimeout(() => { badge.style.transform = "scale(1)"; }, 300);
    }

    // Sikeres rendelés jelzése a vásárlói nézeten
    const successOrderNumber = document.getElementById("success-order-number-" + completedOrderId);
    const successStatusText = document.getElementById("success-status-text-" + completedOrderId);

    if (successOrderNumber && successStatusText) {
        
        // A szám zöldre festése
        successOrderNumber.classList.remove("text-dark");
        successOrderNumber.classList.add("text-success");
        
        // Dobbanó animáció a számnak
        successOrderNumber.style.transform = "scale(1.2)";
        setTimeout(() => { successOrderNumber.style.transform = "scale(1)"; }, 300);

        // A szöveg átírása és zöldre festése
        successStatusText.innerText = "A rendelésed elkészült! Fáradj a pulthoz!";
        successStatusText.classList.remove("text-muted");
        successStatusText.classList.add("text-success", "fw-bold");
    }
    console.log("A #" + completedOrderId + " rendelés elkészült!");
});

// Kapcsolat indítása
connection.start().catch(function (err) {
    return console.error(err.toString());
});

