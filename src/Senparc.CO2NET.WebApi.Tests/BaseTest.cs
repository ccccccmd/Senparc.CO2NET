using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Senparc.CO2NET.RegisterServices;
using System;
using System.IO;
using System.Text;

namespace Senparc.CO2NET.WebApi.Tests
{
    [TestClass]
    public class BaseTest
    {
        public IServiceCollection ServiceCollection { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public IConfiguration Configuration { get; set; }
        public IMvcCoreBuilder MvcCoreBuilder { get; set; }

        protected IRegisterService registerService;
        protected SenparcSetting _senparcSetting;

        protected Mock<Microsoft.Extensions.Hosting.IHostEnvironment /*IHostingEnvironment*/> _env;

        public BaseTest()
        {
            _env = new Mock<Microsoft.Extensions.Hosting.IHostEnvironment/*IHostingEnvironment*/>();
            _env.Setup(z => z.ContentRootPath).Returns(() => Path.GetFullPath("..\\..\\..\\"));

            //Init();
        }

        public void Init()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            RegisterServiceCollection();
            RegisterServiceStart();
            Console.WriteLine("完成 TaseTest 初始化");
        }

        /// <summary>
        /// 注册 IServiceCollection 和 MemoryCache
        /// </summary>
        public void RegisterServiceCollection()
        {
            ServiceCollection = new ServiceCollection();
            var configBuilder = new ConfigurationBuilder();
            //configBuilder.AddJsonFile("appsettings.json", false, false);
            var config = configBuilder.Build();
            Configuration = config;


            ServiceCollection.AddSenparcGlobalServices(config);

            _senparcSetting = new SenparcSetting() { IsDebug = true };
            config.GetSection("SenparcSetting").Bind(_senparcSetting);

            ServiceCollection.AddMemoryCache();//使用内存缓存


            MvcCoreBuilder = ServiceCollection.AddMvcCore();

            //WebApiEngine wae = new WebApiEngine(new FindWeixinApiService(), 400);
            //wae.InitDynamicApi(builder, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "App_Data"));

            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// 注册 RegisterService.Start()
        /// </summary>
        public void RegisterServiceStart(bool autoScanExtensionCacheStrategies = false)
        {
            IApplicationBuilder app = new ApplicationBuilder(ServiceProvider);

            // 启动 CO2NET 全局注册，必须！
            app.UseSenparcGlobal(_env.Object, _senparcSetting, register =>
            {
            });
        }
    }
}
