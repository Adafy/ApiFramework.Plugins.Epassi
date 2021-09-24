namespace Adafy.ApiFramework.Plugins.Epassi
{
    public class EpassiOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public BrowserOptions Browser { get; set; }
        
        public class BrowserOptions
        {
            public string ExecutablePath { get; set; }
            public string BrowserWSEndpoint { get; set; }
        }

    }
}
