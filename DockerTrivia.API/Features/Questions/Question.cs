namespace DockerTrivia.API.Features.Questions
{
    public class Question
    {
        public required string Type { get; set; }
        public required string Difficulty { get; set; }
        public required string Category { get; set; }
        public required string QuestionText { get; set; }
        public required List<string> AnswerAlternatives { get; set; }
        public required string CorrectAnswer { get; set; }
    }
}
