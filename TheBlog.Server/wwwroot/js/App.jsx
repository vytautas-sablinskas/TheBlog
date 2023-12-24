import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Paths from './Shared/Paths';
import LoginForm from './Authentication/LoginForm';
import RegisterForm from './Authentication/Register';
import ForgotPassword from './Authentication/ForgotPassword';
import ResetPassword from './Authentication/ResetPassword';
import UserProfile from './Profile/UserProfile';
import PageNotFound from './Shared/PageNotFound';
import AuthProvider from './Authentication/AuthProvider';
import Footer from './Shared/Footer';
import Header from './Shared/Header';
import AllUsers from './UserManagement/AllUsers';
import Articles from './Articles/Articles';
import ChangePermissions from './UserManagement/ChangePermissions';
import Article from './Articles/Article';
import Home from './Home/Home';
import ReportedComments from './UserManagement/ReportedComments';

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <div className="d-flex flex-column vh-100">
                    <Header />

                    <Routes>
                        <Route path={Paths.Home} element={<Home />} />
                        <Route path={Paths.Login} element={<LoginForm />} />
                        <Route path={Paths.Registration} element={<RegisterForm />} />
                        <Route path={Paths.ForgotPassword} element={<ForgotPassword />} />
                        <Route path={Paths.ResetPassword} element={<ResetPassword />} />
                        <Route path={Paths.UserProfile} element={<UserProfile />} />
                        <Route path={Paths.Articles} element={<Articles />} />
                        <Route path={Paths.Article} element={<Article />} />

                        <Route path={Paths.AllUsers} element={<AllUsers />} />
                        <Route path={Paths.ChangePermissions} element={<ChangePermissions />} />
                        <Route path={Paths.ReportedComments} element={<ReportedComments />} />

                        <Route path={'/*'} element={<PageNotFound />} />
                    </Routes>
                    <Footer />
                </div>
            </BrowserRouter>
        </AuthProvider>
    );
}

ReactDOM.render(<App />, document.getElementById('root'));