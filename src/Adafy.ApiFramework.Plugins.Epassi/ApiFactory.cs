using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Weikio.ApiFramework.Plugins.Browser;
using Weikio.TypeGenerator;

namespace Adafy.ApiFramework.Plugins.Epassi
{
    public class ApiFactory
    {
        private readonly ILogger<ApiFactory> _logger;
        private readonly ILogger<Weikio.ApiFramework.Plugins.Browser.ApiFactory> _browserLogger;

        public ApiFactory(ILogger<ApiFactory> logger, ILogger<Weikio.ApiFramework.Plugins.Browser.ApiFactory> browserLogger)
        {
            _logger = logger;
            _browserLogger = browserLogger;
        }

        public async Task<List<Type>> Create(EpassiOptions configuration)
        {
            var browserFactory = new Weikio.ApiFramework.Plugins.Browser.ApiFactory(_browserLogger);

            var browserType = (await browserFactory.Create(new BrowserOptions()
            {
                ExecutablePath = configuration.Browser.ExecutablePath, BrowserWSEndpoint = configuration.Browser.BrowserWSEndpoint
            })).First();

            var code = string.Empty;

            var sourceWriter = new StringBuilder();
            sourceWriter.UsingNamespace("System.Threading.Tasks");
            sourceWriter.UsingNamespace("PuppeteerSharp");
            sourceWriter.UsingNamespace(browserType.Namespace);

            sourceWriter.Namespace("Weikio.ApiFramework.Plugins.Epassi");

            sourceWriter.StartClass($"EpassiBrowser : {browserType.FullName}, Weikio.ApiFramework.Plugins.Epassi.IEpassiBrowser");

            sourceWriter.Write(
                "public Task<PuppeteerSharp.Browser> GetMyBrowser() { return GetBrowser(); }");

            sourceWriter.FinishBlock(); // Finish the class

            sourceWriter.StartClass($"EpassiApi : EpassiApiBase");

            sourceWriter.Write(
                "protected override IEpassiBrowser CurrentBrowser() { return (IEpassiBrowser) System.Activator.CreateInstance(System.Type.GetType(\"Weikio.ApiFramework.Plugins.Epassi.EpassiBrowser\")); }");

            sourceWriter.FinishBlock(); // Finish the class

            sourceWriter.FinishBlock(); // Finish the namespace

            code = sourceWriter.ToString();

            try
            {
                var generator = new CodeToAssemblyGenerator();
                generator.ReferenceAssemblyContainingType<EpassiOptions>();
                generator.ReferenceAssemblyContainingType<WebBrowser>();
                generator.ReferenceAssemblyContainingType<PuppeteerSharp.Browser>();
                generator.ReferenceAssembly(browserType.Assembly);

                var assembly = generator.GenerateAssembly(code);

                var result = assembly.GetExportedTypes()
                    .Where(x => x.Name.EndsWith("Api"))
                    .ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create EPassi");

                throw;
            }
        }
    }
}
