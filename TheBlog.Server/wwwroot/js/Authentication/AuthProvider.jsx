import React, { useState, useEffect, createContext } from "react";

export const AuthContext = createContext(null);

function AuthProvider({ children }) {
    const [isAuthenticated, setIsAuthenticated] = useState(window.isAuthenticated);
    const [userRoles, setUserRoles] = useState(window.userRoles);
    const [username, setUsername] = useState(window.username);

    useEffect(() => {
        console.log(`Updated user roles: ${userRoles}`);
        console.log(`Updated username: ${username}`);
    }, [userRoles, username]);

    const login = (userRoles, username) => {
        setIsAuthenticated(true);
        setUserRoles(userRoles);
        setUsername(username);
    };

    const logout = () => {
        setIsAuthenticated(false);
        setUserRoles([]);
        setUsername('');
    }

    return (
        <AuthContext.Provider value={{ isAuthenticated, login, logout, userRoles, username }}>
            {children}
        </AuthContext.Provider>
    );
}

export default AuthProvider;