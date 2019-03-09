using System;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace ProjectUsingLibraryViewer.Utilities
{
    /// <summary>
    /// プロジェクトファイルに対するユーティリティクラスです。
    /// </summary>
    public static class ProjectFileUtility
    {
        /// <summary>
        /// プロジェクトファイルで宣言されている.NETFrameworkないし、.NETCoreのバージョンを取得します。
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <returns></returns>
        public static string GetTargetFramework(string projectFilePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(projectFilePath);
            string netFrameworkVersion = xmlDocument.SelectSingleNode("Project/PropertyGroup/TargetFrameworkVersion")?.InnerText;
            if (!string.IsNullOrEmpty(netFrameworkVersion))
            {
                return netFrameworkVersion;
            }
            string netCoreVersion = xmlDocument.SelectSingleNode("Project/PropertyGroup/TargetFramework")?.InnerText;
            if (!string.IsNullOrEmpty(netCoreVersion))
            {
                return netCoreVersion.Replace("netcoreapp", ".NETCore ");
            }
            throw new NotSupportedException($"{projectFilePath}からはフレームワークバージョンを取得できませんでした。");
        }
    }
}
