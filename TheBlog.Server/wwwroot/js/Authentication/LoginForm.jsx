import React, { useContext, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Paths from '../Shared/Paths';
import { AuthContext } from './AuthProvider';
import FetchLinks from '../Shared/FetchLinks';
import axios from 'axios';
import Home from '../Home/Home';

function LoginForm() {
    const navigate = useNavigate();
    const { login, isAuthenticated } = useContext(AuthContext);

    const [formData, setFormData] = useState({
        username: '',
        password: '',
    });
    const [errors, setErrors] = useState({});

    function handleChange(event) {
        const { name, value } = event.target;
        setFormData(prevFormData => ({
            ...prevFormData,
            [name]: value,
        }));
    }

    function handleSubmit(event) {
        event.preventDefault();
        const loginViewModel = {
            Username: formData.username,
            Password: formData.password,
        };

        axios.post(FetchLinks.Login, loginViewModel)
            .then(response => {
                const data = response.data;
                if (data.success) {
                    login(data.userRoles, loginViewModel.Username);
                    navigate(Paths.Home);
                } else {
                    setErrors({ serverError: data.errorMessage });
                }
            })
            .catch(error => {
                const message = error.response
                    ? error.response.data.errorMessage
                    : 'There was a problem submitting the form.';
                setErrors({ serverError: message });
            });
    }

    if (isAuthenticated) {
        return (
            <Home />
        );
    }

    return (
        <div className="d-flex justify-content-center align-items-center mt-3 mb-3">
            <form
                onSubmit={handleSubmit}
                className="px-4 pb-4 shadow-sm bg-dark mx-auto rounded"
                style={{ width: '500px', maxWidth: '500px' }}
            >
                <h4 className="text-center mb-3 mt-3 pt-3 pb-3 text-light">Login to your account</h4>
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
                        className="form-control"
                        placeholder="Username"
                    />
                </div>

                <div className="form-group mb-3">
                    <input
                        type="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        className="form-control"
                        placeholder="Password"
                    />
                </div>

                <div className="form-group d-grid col-6 mx-auto">
                    <button type="submit" className="btn btn-primary mb-3">
                        Login
                    </button>
                    <Link className="btn btn-secondary" to={Paths.ForgotPassword}>Forgot password?</Link>
                </div>
            </form>
        </div>
    );
}

export default LoginForm;