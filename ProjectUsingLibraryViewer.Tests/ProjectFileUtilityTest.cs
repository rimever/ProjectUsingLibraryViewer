using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectUsingLibraryViewer.Utilities;

namespace ProjectUsingLibraryViewer.Tests
{
    /// <summary>
    /// <see cref="ProjectFileUtility"/>をテストするクラスです。
    /// </summary>
    public class ProjectFileUtilityTest
    {
        /// <summary>
        /// <see cref="ProjectFileUtility.GetTargetFramework"/>をテストします。
        /// </summary>
        /// <returns></returns>
        [TestCaseSource(nameof(GetTargetFrameworkTestCaseSource))]
        public string GetTargetFramework(string projectFilePath)
        {
            return ProjectFileUtility.GetTargetFramework(projectFilePath);
        }

        /// <summary>
        /// <see cref="GetTargetFramework"/>のテストケースです。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> GetTargetFrameworkTestCaseSource()
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"../..","TestData","Utilities","ProjectUtility");
            yield return new TestCaseData(Path.Combine(directoryPath, "ConsoleApplication.csproj")).Returns(".NETCore 2.1");
            yield return new TestCaseData(Path.Combine(directoryPath, "WindowsFormsMemoryCheck.csproj")).Returns(".NETFramework 4.6.1");
        }
    }
}
