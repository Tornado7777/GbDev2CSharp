namespace CloneHabr.Dto.Requests
{
    public class GetByIdArticleResponse
    {

        public GetByIdArticleStatus Status { get; set; }
        public ArticleDto articleDto { get; set; }
    }
}
