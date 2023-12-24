import React, { useContext, useEffect, useState } from 'react';
import axios from 'axios';
import { AuthContext } from '../Authentication/AuthProvider';
import PageNotFound from '../Shared/PageNotFound';
import Loading from '../Shared/Loading';
import ChangeProfileModal from './ChangeProfileModal';
import ChangePasswordModal from './ChangePasswordModal';
import FetchLinks from '../Shared/FetchLinks';

const UserProfile = () => {
    const [userProfile, setUserProfile] = useState(null);
    const { isAuthenticated } = useContext(AuthContext);
    const [showChangeProfileModal, setShowChangeProfileModal] = useState(false);
    const [showChangePasswordModal, setShowChangePasswordModal] = useState(false);

    useEffect(() => {
        const getUserInformation = async () => {
            try {
                const response = await axios.get(FetchLinks.UserProfile);
                setUserProfile(response.data.userProfile);
            } catch (error) {
            }
        };

        if (isAuthenticated) {
            getUserInformation();
        }
    }, []);

    const toggleChangeProfileModal = () => {
        setShowChangeProfileModal(!showChangeProfileModal);
    };

    const toggleChangePasswordModal = () => {
        setShowChangePasswordModal(!showChangePasswordModal);
    };

    const updateUsername = (newUsername) => {
        setUserProfile(prevState => {
            return {
                ...prevState,
                userName: newUsername
            };
        });
    };

    if (!isAuthenticated) {
        return <PageNotFound />;
    }

    if (!userProfile) {
        return <Loading />;
    }

    return (
        <div className="d-flex justify-content-center align-items-center mt-3 mb-3">
            <div
                className="px-4 pb-4 shadow-sm bg-dark mx-auto rounded"
                style={{ width: '500px', maxWidth: '500px' }}
            >
                <h4 className="text-center mb-3 mt-3 pt-3 pb-3 text-light">Hello {userProfile.userName}</h4>
                <div className="form-group d-grid col-6 mx-auto mb-3">
                    <button onClick={toggleChangeProfileModal} className="btn btn-secondary mb-3">
                        Change profile information
                    </button>
                    <button onClick={toggleChangePasswordModal} className="btn btn-secondary">
                        Change password
                    </button>
                </div>
            </div>
            <ChangeProfileModal
                isOpen={showChangeProfileModal}
                toggleModal={toggleChangeProfileModal}
                updateUsername={updateUsername}
                currentUsername={userProfile.userName}
            />
            <ChangePasswordModal
                isOpen={showChangePasswordModal}
                toggleModal={toggleChangePasswordModal}
            />
        </div>
    );
};

export default UserProfile;