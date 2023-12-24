using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.ViewModels.Profile;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper _mapper;
        private readonly ISignInManagerWrapper<User> _signInManagerWrapper;
        private readonly IUserManagerWrapper<User> _userManagerWrapper;
        private readonly IRoleService _roleService;
        private readonly IRepository<User> _userRepository;

        public UserProfileService(IMapper mapper, ISignInManagerWrapper<User> signInManagerWrapper, IUserManagerWrapper<User> userManagerWrapper, IRoleService roleService, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _signInManagerWrapper = signInManagerWrapper;
            _userManagerWrapper = userManagerWrapper;
            _roleService = roleService;
            _userRepository = userRepository;
        }

        public async Task LogoutAsync()
        {
            await _signInManagerWrapper.SignOutAsync();
        }

        public async Task<UserProfileViewModel> GetUserProfileViewModelAsync(string username)
        {
            var user = await _userManagerWrapper.FindByNameAsync(username);
            return _mapper.Map<UserProfileViewModel>(user);
        }

        public async Task<IdentityResult> UpdateProfileAsync(string currentUsername, UserNewProfileViewModel newProfile)
        {
            var user = await _userManagerWrapper.FindByNameAsync(currentUsername);
            var usernameToChangeTo = await _userManagerWrapper.FindByNameAsync(newProfile.UserName);
            if (user == null || usernameToChangeTo != null)
            {
                return IdentityResult.Failed();
            }

            _mapper.Map(newProfile, user);

            var result = await _userManagerWrapper.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManagerWrapper.SignOutAsync();
                await _signInManagerWrapper.SignInAsync(user, isPersistent: true);
            }

            return result;
        }

        public async Task<IdentityResult> UpdatePasswordAsync(string currentUsername, UserNewPasswordViewModel viewModel)
        {
            var user = await _userManagerWrapper.FindByNameAsync(currentUsername);
            if (user == null)
            {
                return IdentityResult.Failed();
            }

            return await _userManagerWrapper.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);
        }

        public List<UserProfileViewModel> GetAllUserProfileViewModels()
        {
            var users = _userRepository.GetAll().ToList();

            return users.Select(_mapper.Map<UserProfileViewModel>).ToList();
        }

        public async Task<UserProfileWithRolesViewModel> GetUserProfileWithRolesViewModelAsync(string username)
        {
            var user = await _userManagerWrapper.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }

            var userProfileViewModel = await GetUserProfileViewModelAsync(username);
            var userProfileWithRolesViewModel = _mapper.Map<UserProfileWithRolesViewModel>(userProfileViewModel);

            var userRoles = await _roleService.GetUserRolesByUsernameAsync(username);
            userProfileWithRolesViewModel.Roles = userRoles;

            return userProfileWithRolesViewModel;
        }
    }
}