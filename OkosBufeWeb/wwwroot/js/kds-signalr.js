// Összekapcsolódunk a szerver hubjával
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .withAutomaticReconnect() 
    .build();


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

// Kapcsolat indítása
connection.start().catch(function (err) {
    return console.error(err.toString());
});