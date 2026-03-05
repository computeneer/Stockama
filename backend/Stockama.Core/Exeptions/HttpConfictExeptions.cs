namespace Stockama.Core.Exeptions;

public class HttpConfictExeptions : Exception
{
   public HttpConfictExeptions() { }

   public HttpConfictExeptions(string message) : base(message) { }
}