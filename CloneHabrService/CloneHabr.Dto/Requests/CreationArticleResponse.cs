namespace CloneHabr.Dto.Requests
{
    public class CreationArticleResponse
    {

        public CreationArticleStatus Status { get; set; }
        public IDictionary<string, string[]> ValidationResult { get; set; }
        public ArticleDto articleDto { get; set; }
    }
}
