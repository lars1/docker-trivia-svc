using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DockerTrivia.API.Features.Questions
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private ITriviaApiService _apiService;

        public QuestionsController(ITriviaApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var question = await _apiService.GetQuestionAsync();
            return JsonSerializer.Serialize(question, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
