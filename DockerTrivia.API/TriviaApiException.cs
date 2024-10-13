using System.Runtime.Serialization;

namespace DockerTrivia.API
{
    public class TriviaApiException : Exception
    {
        public TriviaApiException()
        {
        }

        public TriviaApiException(string? message) : base(message)
        {
        }

        public TriviaApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
