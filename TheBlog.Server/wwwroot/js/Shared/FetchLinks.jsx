const FetchLinks = {
    Login: '/UserAccess/Login',
    Register: '/UserAccess/Register',
    ForgotPassword: '/UserAccess/ForgotPassword',
    ResetPassword: '/UserAccess/ResetPassword',

    Articles: '/Article/Articles',
    Article: '/Article/Article',

    Logout: '/UserProfile/Logout',
    UserProfile: '/UserProfile/UserProfile',
    UpdateProfile: '/UserProfile/UpdateProfile',
    UpdatePassword: '/UserProfile/UpdatePassword',

    UserProfiles: '/UserProfile/UserProfiles',
    ProfileWithRoles: '/UserProfile/ProfileWithRoles?username=:username',
    ChangePermissions: '/Admin/ChangeUserRoles',
};

export default FetchLinks;