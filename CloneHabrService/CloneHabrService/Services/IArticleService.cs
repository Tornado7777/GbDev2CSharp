using CloneHabrService.Models;
using CloneHabrService.Models.Requests;

namespace CloneHabrService.Services
{
    public interface IArticleService
    {
        //нужно сопоставить входные и выходны переменные по типу с контроллером, как у Authentificate
        public CreationArticleResponse Create(CreationArticleRequest creationArticleRequest);
        public List<ArticleDto> GetAll();
        public ArticleDto GetById(int id);
    }
}
