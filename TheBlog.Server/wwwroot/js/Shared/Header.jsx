import React, { useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../Authentication/AuthProvider';
import Paths from './Paths';
import axios from 'axios';
import FetchLinks from './FetchLinks';
import Roles from './Roles';

function Header() {
    const { isAuthenticated, logout, userRoles } = useContext(AuthContext);
    const navigate = useNavigate();
    const hasAdminRole = userRoles.includes(Roles.Admin);

    const onLogout = async (e) => {
        e.preventDefault();
        e.stopPropagation();

        console.log(FetchLinks.Logout);
        const response = await axios.post(FetchLinks.Logout, {});
        const data = await response.data;
        console.log(data);
        if (data.success) {
            logout();
            navigate(Paths.Home);
        }
    }

    return (
        <header>
            <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow">
                <div className="container-fluid">
                    <Link className="navbar-brand" to={Paths.Home}>The Blog</Link>
                    {hasAdminRole ? (
                        <>
                            <Link className="navbar-brand" to={Paths.AllUsers}>User Management</Link>
                            <Link className="navbar-brand" to={Paths.ReportedComments}>Reported Comments</Link>
                        </>
                    ) : (
                        <Link className="navbar-brand" to={Paths.Articles}>Articles</Link>
                    )}
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                        <ul className="navbar-nav">
                            {isAuthenticated && (
                                <>
                                    <li className="nav-item">
                                        <Link className="nav-link text-dark" to={Paths.UserProfile}>Profile</Link>
                                    </li>
                                    <li className="nav-item">
                                        <Link className="nav-link text-dark" onClick={(e) => onLogout(e)}>Logout</Link>
                                    </li>
                                </>
                            )}
                            {!isAuthenticated && (
                                <>
                                    <li className="nav-item">
                                        <Link className="nav-link text-dark" to={Paths.Login}>Login</Link>
                                    </li>
                                    <li className="nav-item">
                                        <Link className="nav-link text-dark" to={Paths.Registration}>Register</Link>
                                    </li>
                                </>
                            )}
                        </ul>
                    </div>
                </div>
            </nav>
        </header>
    );
}

export default Header;