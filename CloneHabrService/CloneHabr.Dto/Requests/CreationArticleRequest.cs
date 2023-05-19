namespace CloneHabr.Dto.Requests
{
    public class CreationArticleRequest
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string LoginUser { get; set; }

        public int ArticleTheme { get; set; }

    }
}
