namespace Stockama.Core.Exeptions;

internal class HttpBadRequestException : Exception
{
   public HttpBadRequestException() { }

   public HttpBadRequestException(string message) : base(message) { }
}