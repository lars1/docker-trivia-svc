namespace DockerTrivia.API.Features.Questions.Remote
{
    public class TriviaApiResponse
    {
        public int Response_code { get; set; }
        public List<TriviaApiQuestion>? Results { get; set; }
    }
}
