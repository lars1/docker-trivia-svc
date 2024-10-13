using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DockerTrivia.API.Features.Questions.Remote
{
    public class TriviaApiService : ITriviaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TriviaApiOptions _config;
        private readonly JsonSerializerOptions _defaultSerializerOptions;

        /// <summary>
        /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#bind-hierarchical-configuration-data-using-the-options-pattern
        /// </summary>
        public TriviaApiService(HttpClient httpClient, IOptions<TriviaApiOptions> configOptions)
        {
            _httpClient = httpClient;
            _config = configOptions.Value;
            _defaultSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Question> GetQuestionAsync()
        {
            var uribuilder = new UriBuilder(_config.ApiUri);
            uribuilder.Query = "?amount=1";
            var response = await _httpClient.GetAsync(uribuilder.Uri);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<TriviaApiResponse>(responseText, _defaultSerializerOptions);

            if (responseObj == null || responseObj.Response_code != 0)
            {
                throw new TriviaApiException($"Bad Trivia API response code: {responseObj?.Response_code}");
            }
            if (responseObj.Results == null || responseObj.Results.Count != 1)
            {
                throw new TriviaApiException($"Malformed Trivia API response JSON: {responseText}");
            }
            var rq = responseObj.Results[0];

            if (responseObj.Results == null 
                || responseObj.Results.Count != 1
                || rq.Type == null 
                || rq.Difficulty == null
                || rq.Category == null
                || rq.Question == null
                || rq.Correct_answer == null
                || rq.Incorrect_answers == null)
            {
                throw new TriviaApiException($"Malformed Trivia API response JSON: {responseText}");
            }

            return new Question()
            {
                Type = rq.Type,
                Difficulty = rq.Difficulty,
                Category = rq.Category,
                QuestionText = rq.Question,
                AnswerAlternatives = (new string[] { rq.Correct_answer }).Concat(rq.Incorrect_answers).ToList(),
                CorrectAnswer = rq.Correct_answer,
            };
        }
    }
}
