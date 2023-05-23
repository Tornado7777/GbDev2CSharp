using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class ArticlesLidResponse
    {
        public ArtclesLidStatus Status { get; set;}
        public List<ArticleDto> Articles { get; set;}
    }
}
