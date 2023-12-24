import React, { useContext, useState } from 'react';
import { AuthContext } from './AuthProvider';
import FetchLinks from '../Shared/FetchLinks';
import axios from 'axios';
import Home from '../Home/Home';

const ForgotPassword = () => {
    const { isAuthenticated } = useContext(AuthContext);
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [message, setMessage] = useState(null);
    const [isSuccess, setIsSuccess] = useState(null);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const formData = {
            Username: username,
            Email: email
        };

        try {
            const response = await axios.post(FetchLinks.ForgotPassword, formData);

            const data = await response.data;

            setMessage(data.message);
            setIsSuccess(data.success);
        } catch (error) {
            setMessage('An error occurred while trying to send reset link.');
            setIsSuccess(false);
        }
    }

    if (isAuthenticated) {
        return (
            <Home />
        );
    }

    return (
        <div className="container mt-5 pt-5">
            <div className="row">
                <div className="col-12 col-sm-8 col-md-6 m-auto">
                    <div className="card border-0 shadow bg-dark box-shadow">
                        <h4 className="text-center mb-2 mt-3 text-light">Forgot your password?</h4>
                        <div className="card-body text-center">
                            {message && (
                                <div className={`alert ${isSuccess ? 'alert-success' : 'alert-danger'}`}>
                                    {message}
                                </div>
                            )}

                            <form onSubmit={handleSubmit}>
                                <div className="form-outline">
                                    <input
                                        type="username"
                                        className="form-control"
                                        placeholder="Username"
                                        value={username}
                                        onChange={(e) => setUsername(e.target.value)}
                                        required
                                    />
                                </div>

                                <div className="form-outline my-4 py-2">
                                    <input
                                        type="email"
                                        className="form-control"
                                        placeholder="Email Address"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required
                                    />
                                </div>

                                <button type="submit" className="btn btn-primary">Submit</button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ForgotPassword;