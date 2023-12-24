using TheBlog.MVC.ViewModels.Home;

namespace TheBlog.MVC.Services
{
    public interface IHomePageService
    {
        HomePageViewModel GetHomePageArticles();
    }
}