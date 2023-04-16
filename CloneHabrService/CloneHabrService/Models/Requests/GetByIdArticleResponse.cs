namespace CloneHabrService.Models.Requests
{
    public class GetByIdArticleResponse
    {

        public GetByIdArticleStatus Status { get; set; }
        public ArticleDto articleDto { get; set; }
    }
}
