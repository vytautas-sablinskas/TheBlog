using AutoMapper;
using TheBlog.Data.Entities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;
using TheBlog.MVC.ViewModels.Authentication;
using TheBlog.MVC.ViewModels.Profile;

namespace TheBlog.MVC.Mappings
{
    public class TheBlogProfile : Profile
    {
        public TheBlogProfile()
        {
            CreateMap<RegisterViewModel, User>();
            CreateMap<User, UserProfileViewModel>();
            CreateMap<UserNewProfileViewModel, User>();
            CreateMap<Article, SimplifiedArticleViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
            CreateMap<Article, ArticleViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
            CreateMap<AddArticleViewModel, Article>()
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<EditArticleViewModel, Article>();
            CreateMap<UserProfileViewModel, UserProfileWithRolesViewModel>();
            CreateMap<ArticleUserRatingViewModel, ArticleUserRating>();
            CreateMap<Comment, ArticleCommentViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
            CreateMap<AddArticleCommentViewModel, Comment>();
            CreateMap<EditArticleCommentViewModel, Comment>();
            CreateMap<AddOrUpdateReportCommentViewModel, ReportedComment>();
            CreateMap<ReportedComment, ReportedCommentViewModel>();
        }
    }
}