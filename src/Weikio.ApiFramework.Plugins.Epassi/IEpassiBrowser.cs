using System.Threading.Tasks;

namespace Weikio.ApiFramework.Plugins.Epassi
{
    public interface IEpassiBrowser
    {
        Task<PuppeteerSharp.Browser> GetMyBrowser();
    }
}