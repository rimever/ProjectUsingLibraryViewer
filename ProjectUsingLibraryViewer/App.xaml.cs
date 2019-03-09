using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using Serilog.Formatting.Compact;

namespace ProjectUsingLibraryViewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        public App()
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(new CompactJsonFormatter(), @"..\log\log-.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true).CreateLogger();
        }
    }
}
