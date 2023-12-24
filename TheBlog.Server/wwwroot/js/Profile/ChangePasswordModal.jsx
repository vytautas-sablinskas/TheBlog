import axios from 'axios';
import React, { useState, useEffect } from 'react';
import SuccessAlert from '../Shared/SuccessAlert';
import FetchLinks from '../Shared/FetchLinks';

const ChangePasswordModal = ({ isOpen, toggleModal }) => {
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmNewPassword, setConfirmNewPassword] = useState('');
    const [errors, setErrors] = useState({});
    const [fetchError, setFetchError] = useState(null);
    const [showSuccess, setShowSuccess] = useState(false);

    useEffect(() => {
        if (!isOpen) {
            setFetchError(null);
            setCurrentPassword('');
            setNewPassword('');
            setConfirmNewPassword('');
        }
    }, [isOpen]);

    const validateForm = () => {
        let formErrors = {};
        let formIsValid = true;

        if (!currentPassword) {
            formIsValid = false;
            formErrors["currentPassword"] = "Current password is required.";
        }

        if (!newPassword) {
            formIsValid = false;
            formErrors["newPassword"] = "New password is required.";
        } else if (newPassword.length < 6 || newPassword.length > 100) {
            formIsValid = false;
            formErrors["newPassword"] = "New password must be between 6 and 100 characters.";
        } else if (!/^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$/.test(newPassword)) {
            formIsValid = false;
            formErrors["newPassword"] = "New password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.";
        }

        if (newPassword !== confirmNewPassword) {
            formIsValid = false;
            formErrors["confirmNewPassword"] = "New password and confirmation password must match.";
        }

        setErrors(formErrors);
        return formIsValid;
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        if (validateForm()) {
            try {
                const response = await axios.post(FetchLinks.UpdatePassword, {
                    CurrentPassword: currentPassword,
                    NewPassword: newPassword,
                    ConfirmPassword: confirmNewPassword
                });

                const data = response.data;
                if (data.success) {
                    toggleModal();
                    setShowSuccess(true);
                } else {
                    setFetchError(data.errorMessage);
                }
            } catch (error) {
                setFetchError("There was a problem updating the password. Try again later.");
            }
        }
    };

    return (
        <>
            {isOpen && (
                <div className="modal-backdrop show" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}></div>
            )}
            <div className={`modal ${isOpen ? 'd-block' : 'd-none'}`} tabIndex="-1" style={{ display: 'block' }}>
                <div className="modal-dialog modal-dialog-centered">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title">Change Password</h5>
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
                                    <label htmlFor="currentPassword" className="form-label">Current Password</label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.currentPassword ? 'is-invalid' : ''}`}
                                        id="currentPassword"
                                        value={currentPassword}
                                        onChange={(e) => setCurrentPassword(e.target.value)}
                                    />
                                    <div className="invalid-feedback">
                                        {errors.currentPassword}
                                    </div>
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="newPassword" className="form-label">New Password</label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.newPassword ? 'is-invalid' : ''}`}
                                        id="newPassword"
                                        value={newPassword}
                                        onChange={(e) => setNewPassword(e.target.value)}
                                    />
                                    <div className="invalid-feedback">
                                        {errors.newPassword}
                                    </div>
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="confirmNewPassword" className="form-label">Confirm New Password</label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.confirmNewPassword ? 'is-invalid' : ''}`}
                                        id="confirmNewPassword"
                                        value={confirmNewPassword}
                                        onChange={(e) => setConfirmNewPassword(e.target.value)}
                                    />
                                    <div className="invalid-feedback">
                                        {errors.confirmNewPassword}
                                    </div>
                                </div>
                                <div className="modal-footer justify-content-between">
                                    <button type="button" className="btn btn-secondary" onClick={toggleModal}>Cancel</button>
                                    <button type="submit" className="btn btn-primary">Change Password</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
            <SuccessAlert
                message="Password was changed successfully!"
                show={showSuccess}
                onClose={() => setShowSuccess(false)}
            />
        </>
    );
};

export default ChangePasswordModal;