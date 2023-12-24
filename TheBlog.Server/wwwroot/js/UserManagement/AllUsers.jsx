import React, { useState, useEffect, useContext } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { AuthContext } from '../Authentication/AuthProvider';
import Roles from '../Shared/Roles';
import PageNotFound from '../Shared/PageNotFound';
import Loading from '../Shared/Loading';
import Paths from '../Shared/Paths';
import FetchLinks from '../Shared/FetchLinks';

const AllUsers = () => {
    const { isAuthenticated, userRoles } = useContext(AuthContext);
    const [userProfiles, setUserProfiles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(false);

    useEffect(() => {
        const fetchUserProfiles = async () => {
            try {
                setIsLoading(true);
                setError(false);
                const response = await axios.get(FetchLinks.UserProfiles);
                const data = response.data;

                setUserProfiles(data.allUserProfiles || []);
            } catch (error) {
                setError(true);
            } finally {
                setIsLoading(false);
            }
        };

        if (isAuthenticated && userRoles.includes(Roles.Admin)) {
            fetchUserProfiles();
        }
    }, [isAuthenticated, userRoles]);

    if (!isAuthenticated || !userRoles.includes(Roles.Admin)) {
        return <PageNotFound />;
    }

    if (isLoading) {
        return <Loading />;
    }

    if (error) {
        return (
            <div className="container">
                <div className="row justify-content-center mt-4">
                    <div className="col-auto">
                        <div className="alert alert-danger" style={{ width: '500px' }} role="alert">
                            User fetching failed.
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container">
            <div className="row justify-content-center align-items-center" style={{ minHeight: '100vh' }}>
                <div className="col-md-auto" style={{ width: '500px' }}>
                    <ul className="list-group">
                        <li className="list-group-item d-flex justify-content-between">
                            <strong>Username</strong>
                            <strong>Actions</strong>
                        </li>
                        {userProfiles.map(user => (
                            <li key={user.id} className="list-group-item d-flex justify-content-between align-items-center">
                                {user.userName}
                                <Link to={Paths.ChangePermissions.replace(":username", user.userName)} className="btn btn-primary">
                                    Change Permissions
                                </Link>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default AllUsers;