using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Adafy.ApiFramework.Plugins.Epassi
{
    public abstract class EpassiApiBase
    {
        protected abstract IEpassiBrowser CurrentBrowser();

        public EpassiOptions Configuration { get; set; }

        public async Task<FileInfo> GetLunchExcel(DateTime from, DateTime to)
        {
            using (var browser = await CurrentBrowser().GetMyBrowser())
            {
                using (var page = await browser.NewPageAsync())
                {
                    var url = "https://services.epassi.fi";
                    await page.GoToAsync(url);

                    await page.TypeAsync("#username", Configuration.UserName);
                    await page.TypeAsync("input[name='password']", Configuration.Password);

                    await page.ClickAsync("#login-button");
                    await page.WaitForSelectorAsync("#popnavi-container");

                    var s =
                        $@"company/statistics/export?end_date={to:yyy-MM-dd}&scope=all&start_date={from:yyy-MM-dd}&stat_mode=people&tt={DateTime.Now.Ticks}";

                    var cookies = await page.GetCookiesAsync();

                    var baseAddress = new Uri("https://services.epassi.fi");
                    var cookieContainer = new CookieContainer();

                    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                    {
                        foreach (var cookie in cookies)
                        {
                            cookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                        }

                        var tempPath = Path.GetTempPath();
                        
                        var res = await client.GetAsync(s);

                        if (res.IsSuccessStatusCode)
                        {
                            var result = await res.Content.ReadAsByteArrayAsync();
                            
                            var fileName = res.Content.Headers?.ContentDisposition?.FileName ?? Guid.NewGuid() + ".xls";
                            var filePath = Path.Combine(tempPath, fileName.Trim('"'));
                            
                            await File.WriteAllBytesAsync(filePath, result);

                            return new FileInfo(filePath);
                        }
                        
                        throw new Exception("Unable to download report");
                    }
                }
            }
        }
    }
}
