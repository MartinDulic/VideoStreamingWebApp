async function sendNotifications() {
    console.log("Notifications sent")
    if (await getNumberOfUnsentNotifications() > 0) {
        fetch('/api/Notifications/SendAllNotifications', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({})
        })
        .then(response => {
            if (response.ok) {
                alert('Notifications sent successfully!');
                location.reload()
            } else {
                alert('Failed to send notifications.');
            }
        })
        .catch(error => {
            alert('An error occurred while sending notifications.');
        });
    } else {
        alert('No unsent notifications to send.');
    }
}


async function getNumberOfUnsentNotifications() {
    let unsentNotficationsCount;

    await fetch('api/Notifications/GetNumberOfUnsentNotifications')
    .then(response => response.json())
    .then(data =>{
        unsentNotficationsCount = data;
    })
    .catch(error => {
        console.error('Error retrieving unsent notifications count:', error);
    });

    return unsentNotficationsCount;
}

async function updateNotificationCount() {
    document.getElementById("notificationsCounter").innerText = `Unsent Notifications: ${await getNumberOfUnsentNotifications()}`    
}


document.getElementById("btnNotifications").addEventListener("click", function () {
    sendNotifications();
});

document.addEventListener("DOMContentLoaded", function () {
    updateNotificationCount();
});