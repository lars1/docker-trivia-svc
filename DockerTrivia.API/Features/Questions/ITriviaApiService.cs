namespace DockerTrivia.API.Features.Questions
{
    public interface ITriviaApiService
    {
        Task<Question> GetQuestionAsync();
    }
}
