import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import axios from 'axios';
import Loading from '../Shared/Loading';
import PageNotFound from '../Shared/PageNotFound';
import FetchLinks from '../Shared/FetchLinks';

const ResetPassword = () => {
    const [errors, setErrors] = useState({});
    const [responseMessage, setResponseMessage] = useState(null);
    const [isSuccess, setIsSuccess] = useState(false);

    const [isValid, setIsValid] = useState(null);
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);

    const [formData, setFormData] = useState({
        token: encodeURIComponent(queryParams.get('token')),
        email: encodeURIComponent(queryParams.get('email')),
        newPassword: '',
        confirmPassword: ''
    });

    useEffect(() => {
        if (!formData.token || !formData.email) {
            setIsValid(false);
            return;
        }

        const checkValidity = async () => {
            try {
                const response = await axios.get(
                    `${FetchLinks.ResetPassword}?token=${formData.token}&email=${formData.email}`
                );

                const data = response.data;
                setIsValid(data.isTokenAndEmailValid);
            } catch (error) {
                setIsValid(false);
            }
        };

        checkValidity();
    }, []);

    function validateForm() {
        const { newPassword, confirmPassword } = formData;
        let errors = {};
        let formIsValid = true;

        if (!newPassword) {
            formIsValid = false;
            errors["newPassword"] = "New Password is required.";
        } else if (newPassword.length < 6 || newPassword.length > 100) {
            formIsValid = false;
            errors["newPassword"] = "New Password must be between 6 and 100 characters.";
        } else if (!/^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$/.test(newPassword)) {
            formIsValid = false;
            errors["newPassword"] = "New Password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.";
        }

        if (newPassword !== confirmPassword) {
            formIsValid = false;
            errors["confirmPassword"] = "New Password and Confirm Password must match.";
        }

        setErrors(errors);
        return formIsValid;
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };
    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateForm()) {
            return;
        }

        try {
            const response = await axios.post('/UserAccess/ResetPassword', formData);
            setResponseMessage(response.data.message);
            setIsSuccess(response.data.success);
        } catch (error) {
            setResponseMessage("Failed resetting password. Try again later!");
            setIsSuccess(false);
        }
    };

    if (isValid === null) {
        return (
            <Loading />
        );
    }

    if (isValid === false) {
        return <PageNotFound />;
    }

    return (
        <div className="d-flex justify-content-center align-items-center mt-3 mb-3">
            <form
                onSubmit={handleSubmit}
                className="px-4 pb-4 shadow-sm bg-dark mx-auto rounded"
                style={{ width: '500px', maxWidth: '500px' }}
            >
                <h4 className="text-center mb-3 mt-3 pt-3 pb-3 text-light">Reset Password</h4>
                {responseMessage && (
                    <div className={`alert ${isSuccess ? 'alert-success' : 'alert-danger'} text-center mb-3`} role="alert">
                        {responseMessage}
                    </div>
                )}
                <div className="form-group mb-3">
                    <input
                        type="password"
                        name="newPassword"
                        value={formData.newPassword}
                        onChange={handleChange}
                        className={`form-control ${errors.newPassword ? 'is-invalid' : ''}`}
                        placeholder="New Password"
                    />
                    {errors.newPassword && <div className="invalid-feedback">{errors.newPassword}</div>}
                </div>

                <div className="form-group mb-3">
                    <input
                        type="password"
                        name="confirmPassword"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                        placeholder="Confirm Password"
                    />
                    {errors.confirmPassword && <div className="invalid-feedback">{errors.confirmPassword}</div>}
                </div>

                <div className="form-group d-grid col-6 mx-auto">
                    <button type="submit" className="btn btn-primary">
                        Reset Password
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ResetPassword;