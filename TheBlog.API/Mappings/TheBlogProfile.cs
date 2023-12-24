using AutoMapper;
using TheBlog.Data.Entities;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Mappings
{
    public class TheBlogProfile : Profile
    {
        public TheBlogProfile()
        {
            CreateMap<Article, SimplifiedArticleViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
            CreateMap<Article, ArticleViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
            CreateMap<AddArticleViewModel, Article>()
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<EditArticleViewModel, Article>();
            CreateMap<ArticleUserRatingViewModel, ArticleUserRating>();
            CreateMap<Comment, ArticleCommentViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.User.UserName));
        }
    }
}