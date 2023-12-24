import React, { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { AuthContext } from '../Authentication/AuthProvider';
import Roles from '../Shared/Roles';
import PageNotFound from '../Shared/PageNotFound';
import Loading from '../Shared/Loading';
import FetchLinks from '../Shared/FetchLinks';

const ChangePermissions = () => {
    const { isAuthenticated, userRoles } = useContext(AuthContext);
    const { username } = useParams();
    const [isLoading, setIsLoading] = useState(true);
    const [userProfile, setUserProfile] = useState(null);
    const [selectedRoles, setSelectedRoles] = useState([]);
    const [addRoles, setAddRoles] = useState([]);
    const [removeRoles, setRemoveRoles] = useState([]);
    const [showNotification, setShowNotification] = useState(false);
    const [notificationMessage, setNotificationMessage] = useState('');
    const [userIsFound, setUserIsFound] = useState(false);

    useEffect(() => {
        const fetchUserProfileAndRoles = async () => {
            try {
                const response = await axios.get(FetchLinks.ProfileWithRoles.replace(':username', username));
                const data = response.data;

                setUserProfile(data.userProfile);
                setSelectedRoles(data.userProfile.roles);
                setUserIsFound(true);
            } catch (error) {
                setNotificationMessage("Failed to get user roles.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchUserProfileAndRoles();
    }, [username]);

    useEffect(() => {
        setAddRoles([]);
        setRemoveRoles([]);
    }, [userRoles]);

    const handleRoleChange = (role, isChecked) => {
        if (isChecked) {
            setRemoveRoles(prevRemoveRoles => prevRemoveRoles.filter(r => r !== role));
            if (!userProfile.roles.includes(role)) {
                setAddRoles(prevAddRoles => [...prevAddRoles, role]);
            } else {
                setAddRoles(prevAddRoles => prevAddRoles.filter(r => r !== role));
            }
        } else {
            setAddRoles(prevAddRoles => prevAddRoles.filter(r => r !== role));

            if (userProfile.roles.includes(role)) {
                setRemoveRoles(prevRemoveRoles => [...prevRemoveRoles, role]);
            } else {
                setRemoveRoles(prevRemoveRoles => prevRemoveRoles.filter(r => r !== role));
            }
        }

        setSelectedRoles(
            isChecked
                ? [...selectedRoles, role].filter((v, i, a) => a.indexOf(v) === i)
                : selectedRoles.filter(r => r !== role)
        );
    };

    const submitRoleChanges = async () => {
        if (addRoles.length === 0 && removeRoles.length === 0) {
            setNotificationMessage("No roles were changed.");
            setShowNotification(true);
        } else {
            const payload = {
                UserName: username,
                RolesToAdd: addRoles,
                RolesToRemove: removeRoles
            };

            const response = await axios.post(FetchLinks.ChangePermissions, payload);

            if (response.data.success) {
                setUserProfile(prevUserProfile => ({
                    ...prevUserProfile,
                    roles: [...prevUserProfile.roles.filter(role => !removeRoles.includes(role)), ...addRoles]
                }));

                setSelectedRoles(prevSelectedRoles => [
                    ...prevSelectedRoles.filter(role => !removeRoles.includes(role)),
                    ...addRoles
                ]);

                setNotificationMessage("Roles updated successfully.");
            } else {
                setNotificationMessage("Failed to update roles.");
            }

            setShowNotification(true);
        }
    };

    if (isLoading) {
        return <Loading />;
    }

    if (!isAuthenticated || !userRoles.includes(Roles.Admin) || !userIsFound) {
        return <PageNotFound />;
    }

    return (
        <div className="d-flex justify-content-center align-items-center" style={{ height: "100vh" }}>
            <div className="border border-1 rounded-2 p-3">
                <h1 className="mb-3">User: {userProfile?.userName}</h1>
                {showNotification &&
                    <div className="alert alert-info mb-3" role="alert">
                        {notificationMessage}
                    </div>
                }
                <div>
                    {Object.values(Roles).map((role, index) => (
                        <div key={index} className="form-check">
                            <input
                                className="form-check-input"
                                type="checkbox"
                                checked={selectedRoles.includes(role)}
                                onChange={(e) => handleRoleChange(role, e.target.checked)}
                                id={`role-${index}`}
                            />
                            <label className="form-check-label" htmlFor={`role-${index}`}>
                                {role}
                            </label>
                        </div>
                    ))}
                </div>
                <button className="btn btn-primary d-block w-100 mt-3" onClick={submitRoleChanges}>Change Roles</button>
            </div>
        </div>
    );
};

export default ChangePermissions;