using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.Status;

namespace CloneHabrService.Services
{
    public interface IArticleService
    {
        //нужно сопоставить входные и выходны переменные по типу с контроллером, как у Authentificate
        public CreationArticleResponse Create(CreationArticleRequest creationArticleRequest);
        public List<ArticleDto> GetArticlesByTheme(ArticleTheme articlesTheme);
        public List<ArticleDto> GetArticlesByLogin(string login);
        public ArticlesLidResponse GetArticlesByStatus(string login, ArticleStatus articleStatus);
        public ArticleResponse ChangeArticleStatusById(string login, ArticleStatus articleStatus, int id);
        public List<ArticleDto> GetArticlesByText(string text, bool raitingSort);
        public ArticleDto GetById(int id);

        public LikeResponse CreateLikeArticleById(int articleId, string login);

        public CommentResponse CreateCommnet(CommentDto commentDto,  string login);
        public CommentResponse ChangeComment(CommentDto commentDto, string login);

    }
}
