using System.Threading.Tasks;

namespace Adafy.ApiFramework.Plugins.Epassi
{
    public interface IEpassiBrowser
    {
        Task<PuppeteerSharp.Browser> GetMyBrowser();
    }
}