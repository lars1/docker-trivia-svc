namespace DockerTrivia.API.Features.Questions.Remote
{
    public class TriviaApiQuestion
    {
        public string? Type { get; set; }
        public string? Difficulty { get; set; }
        public string? Category { get; set; }
        public string? Question { get; set; }
        public string? Correct_answer { get; set; }
        public List<string>? Incorrect_answers { get; set; }
    }
}
