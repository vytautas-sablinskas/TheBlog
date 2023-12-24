import React, { useEffect } from 'react';

const SuccessAlert = ({ message, show, onClose }) => {
    useEffect(() => {
        let timer;
        if (show) {
            timer = setTimeout(() => {
                onClose();
            }, 3000);
        }

        return () => clearTimeout(timer);
    }, [show, onClose]);

    if (!show) {
        return null;
    }

    return (
        <div
            className="alert alert-success alert-dismissible fade show"
            role="alert"
            style={{
                position: 'fixed',
                top: '0.5rem',
                left: '50%',
                transform: 'translateX(-50%)',
                zIndex: 1050
            }}
        >
            <strong>{message}</strong> 
        </div>
    );
};

export default SuccessAlert;