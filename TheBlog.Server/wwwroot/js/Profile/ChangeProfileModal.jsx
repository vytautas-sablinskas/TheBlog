import axios from 'axios';
import React, { useState, useEffect } from 'react';
import SuccessAlert from '../Shared/SuccessAlert';
import FetchLinks from '../Shared/FetchLinks';

const ChangeProfileModal = ({ isOpen, toggleModal, updateUsername, currentUsername }) => {
    const [username, setUsername] = useState(currentUsername);
    const [errors, setErrors] = useState({});
    const [fetchError, setFetchError] = useState(null);
    const [showSuccess, setShowSuccess] = useState(false);

    useEffect(() => {
        if (!isOpen) {
            setFetchError(null);
            setUsername(currentUsername);
        }
    }, [isOpen, currentUsername]);

    const validateForm = () => {
        let formErrors = {};
        let formIsValid = true;

        if (!username) {
            formIsValid = false;
            formErrors["username"] = "Username is required.";
        } else if (username.length > 30) {
            formIsValid = false;
            formErrors["username"] = "Username cannot be longer than 30 characters.";
        } else if (!/^[a-zA-Z0-9_]+$/.test(username)) {
            formIsValid = false;
            formErrors["username"] = "Username can only contain letters, numbers, and underscores.";
        } else if (currentUsername === username) {
            formIsValid = false;
            formErrors["username"] = "Can't change to your own username";
        }

        setErrors(formErrors);
        return formIsValid;
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        if (validateForm()) {
            try {
                const response = await axios.post(FetchLinks.UpdateProfile, { UserName: username });

                const data = response.data;
                if (data.success) {
                    updateUsername(username);
                    toggleModal();
                    setShowSuccess(true);
                } else {
                    setFetchError(data.errorMessage);
                }
            } catch (error) {
                setFetchError("Problem with changing information. Try again later!");
            }
        }
    };

    return (
        <>
            {isOpen && (
                <div className="modal-backdrop show" style={{ backgroundColor: 'rgba(0,0,0,0.9)' }}></div>
            )}
            <div className={`modal ${isOpen ? 'd-block' : 'd-none'}`} tabIndex="-1" style={{ display: 'block' }}>
                <div className="modal-dialog modal-dialog-centered">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title">Change Profile Information</h5>
                            <button type="button" className="btn-close" onClick={toggleModal}></button>
                        </div>
                        <div className="modal-body">
                            {fetchError && (
                                <div className={`alert alert-danger text-center mb-3`} role="alert">
                                    {fetchError}
                                </div>
                            )}
                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label htmlFor="username" className="form-label">Username</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                                        id="username"
                                        value={username}
                                        onChange={(e) => setUsername(e.target.value)}
                                    />
                                    <div className="invalid-feedback">
                                        {errors.username}
                                    </div>
                                </div>
                                <div className="modal-footer justify-content-between">
                                    <button type="button" className="btn btn-secondary" onClick={toggleModal}>Cancel</button>
                                    <button type="submit" className="btn btn-primary">Change Information</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
            <SuccessAlert
                message="Profile has been changed successfully!"
                show={showSuccess}
                onClose={() => setShowSuccess(false)}
            />
        </>
    );
};

export default ChangeProfileModal;