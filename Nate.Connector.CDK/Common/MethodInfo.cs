namespace Scribe.Connector.Common
{ 
    internal class MethodInfo
    {
        private string className;

        public MethodInfo()
        {
            className = "";
        }

        public MethodInfo(string className)
        {
            this.className = (className + ".");
        }

        public string GetCurrentMethodName([System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            return string.Format("{0}{1}()", className, callerName);
        }
    }
}