using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"../..","TestData","Utilities","ProjectUtility","GetTargetFramework");
            yield return new TestCaseData(Path.Combine(directoryPath, "NetCore.csproj")).Returns(".NETCore 2.1");
            yield return new TestCaseData(Path.Combine(directoryPath, "NetFramework.csproj")).Returns(".NETFramework 4.6.1");
        }
        [Test]
        public void EnumerablePackageLibrary()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"../..","TestData","Utilities","ProjectUtility","EnumerablePackageLibrary","packages.config");
            var packages = ProjectFileUtility.EnumerablePackageLibrary(path).ToList();
            Assert.AreEqual(packages.Count, 4);
        }
        [Test]
        public void Test()
        {
            string projectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"../..","TestData","Utilities","ProjectUtility","NetCore.csproj");
            var version = GetVersion(projectFilePath);
            Console.WriteLine(version);
        }

        private Version GetVersion(string projectFilePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(projectFilePath);
            var node = xmlDocument.SelectSingleNode("Project/PropertyGroup/AssemblyVersion");
            // .NET Coreのcsprojは初期状態ではノード自体が存在しない
            if (node == null)
            {
                return new Version(1, 0, 0, 0);
            }

            return new Version(node.InnerText);
        }
    }
}
