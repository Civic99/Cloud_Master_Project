import React, { useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";
import '../NotificationComponent.css'; // Import CSS file for styling

const NotificationComponent = () => {
    const [connection, setConnection] = useState(null);
    const [notifications, setNotifications] = useState([]);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:8717/notificationHub")
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start().then(() => {
                console.log('SignalR Connected');
                connection.on("ReceiveNotification", (message) => {
                    console.log('Received Notification:', message);
                    setNotifications(prevNotifications => [...prevNotifications, message]);
                });
            }).catch(err => console.error(err));
        }
    }, [connection]);

    return (
        <div className="notification-container">
            {notifications.map((notification, index) => (
                <div key={index} className="notification">
                    {notification}
                </div>
            ))}
        </div>
    );
};

export default NotificationComponent;