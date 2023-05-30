using CloneHabr.Dto;
using CloneHabr.Dto.Requests;

namespace CloneHabrService.Services
{
    public interface IArticleService
    {
        //нужно сопоставить входные и выходны переменные по типу с контроллером, как у Authentificate
        public CreationArticleResponse Create(CreationArticleRequest creationArticleRequest);
        public List<ArticleDto> GetArticlesByTheme(ArticleTheme articlesTheme);
        public List<ArticleDto> GetArticlesByLogin(string login);
        public List<ArticleDto> GetArticlesByText(string text, bool raitingSort);
        public ArticleDto GetById(int id);

        public LikeResponse CreateLikeArticleById(int articleId, string login);

        public CommentResponse CreateCommnet(CommentDto commentDto,  string login);

    }
}
