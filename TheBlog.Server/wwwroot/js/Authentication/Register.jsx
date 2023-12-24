import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from './AuthProvider';
import Paths from '../Shared/Paths';
import axios from 'axios';
import FetchLinks from '../Shared/FetchLinks';
import Home from '../Home/Home';

function RegisterForm() {
    const navigate = useNavigate();
    const { login, isAuthenticated } = useContext(AuthContext);

    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
    });
    const [errors, setErrors] = useState({});

    function validateForm() {
        const { username, email, password, confirmPassword } = formData;
        let errors = {};
        let formIsValid = true;

        if (!username) {
            formIsValid = false;
            errors["username"] = "Username is required.";
        } else if (username.length > 30) {
            formIsValid = false;
            errors["username"] = "Username cannot be longer than 30 characters.";
        } else if (!/^[a-zA-Z0-9_]+$/.test(username)) {
            formIsValid = false;
            errors["username"] = "Username can only contain letters, numbers, and underscores.";
        }

        if (!email) {
            formIsValid = false;
            errors["email"] = "Email is required.";
        } else if (!/\S+@\S+\.\S+/.test(email)) {
            formIsValid = false;
            errors["email"] = "Invalid Email Address.";
        }

        if (!password) {
            formIsValid = false;
            errors["password"] = "Password is required.";
        } else if (password.length < 6 || password.length > 100) {
            formIsValid = false;
            errors["password"] = "Password must be between 6 and 100 characters.";
        } else if (!/^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$/.test(password)) {
            formIsValid = false;
            errors["password"] = "Password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.";
        }

        if (password !== confirmPassword) {
            formIsValid = false;
            errors["confirmPassword"] = "Password and Confirmation Password must match.";
        }

        setErrors(errors);
        return formIsValid;
    };

    function handleChange(event) {
        const { name, value } = event.target;
        setFormData({
            ...formData,
            [name]: value,
        });
    };

    function handleSubmit(event) {
        event.preventDefault();
        if (validateForm()) {
            const registerViewModel = {
                Username: formData.username,
                Email: formData.email,
                Password: formData.password,
                ConfirmPassword: formData.confirmPassword,
            };

            axios.post(FetchLinks.Register, registerViewModel)
                .then(response => {
                    const data = response.data;
                    if (data.success) {
                        navigate(Paths.Home);
                        login(data.userRoles, registerViewModel.Username);
                    } else {
                        setErrors({ serverError: data.errors });
                    }
                })
                .catch(error => {
                    const serverError = error.response
                        ? (error.response.data.errors || error.response.data.message)
                        : ["There was a problem submitting the form."];
                    setErrors({ serverError });
                });
        }
    };

    if (isAuthenticated) {
        return (
            <Home />
        );
    }

    return (
        <div className="d-flex justify-content-center align-items-center mt-3 mb-3">
            <form
                action="/UserAccess/Register"
                method="post"
                onSubmit={handleSubmit}
                className="px-4 pb-4 shadow-sm bg-dark mx-auto rounded"
                style={{ width: '500px', maxWidth: '500px' }}
            >
                <h4 className="text-center mb-3 mt-3 pt-3 pb-3 text-light">Register a new account</h4>
                {errors.serverError && (
                    <div className="alert alert-danger text-center mb-3" role="alert">
                        {errors.serverError}
                    </div>
                )}
                <div className="form-group mb-3">
                    <input
                        type="text"
                        name="username"
                        value={formData.username}
                        onChange={handleChange}
                        className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                        placeholder="Username"
                    />
                    {errors.username && <div className="invalid-feedback">{errors.username}</div>}
                </div>

                <div className="form-group mb-3">
                    <input
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                        placeholder="Email"
                    />
                    {errors.email && <div className="invalid-feedback">{errors.email}</div>}
                </div>

                <div className="form-group mb-3">
                    <input
                        type="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                        placeholder="Password"
                    />
                    {errors.password && <div className="invalid-feedback">{errors.password}</div>}
                </div>

                <div className="form-group mb-3">
                    <input
                        type="password"
                        name="confirmPassword"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                        placeholder="Repeat password"
                    />
                    {errors.confirmPassword && <div className="invalid-feedback">{errors.confirmPassword}</div>}
                </div>

                <div className="form-group d-grid col-6 mx-auto">
                    <button type="submit" className="btn btn-primary">
                        Sign up
                    </button>
                </div>
            </form>
        </div>
    );
};

export default RegisterForm;