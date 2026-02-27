namespace Stockama.Core.Exeptions;

public class CustomValidationException : Exception
{
   public CustomValidationException() { }
   public CustomValidationException(string message) : base(message) { }
}