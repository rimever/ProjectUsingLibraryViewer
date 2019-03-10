using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using Microsoft.Build.Evaluation;

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
            string netCoreVersion = xmlDocument.SelectSingleNode("Project/PropertyGroup/TargetFramework")?.InnerText;
            if (!string.IsNullOrEmpty(netCoreVersion))
            {
                return netCoreVersion.Replace("netcoreapp", ".NETCore ");
            }
            var project = new Project(projectFilePath);
            foreach (var group in project.Xml.PropertyGroups)
            {
                foreach (var propertyElement in group.Properties)
                {
                    if (propertyElement.Name == "TargetFrameworkVersion")
                    {
                        return propertyElement.Value.Replace("v",".NETFramework ");
                    }   
                }
            }

            throw new NotSupportedException($"{projectFilePath}からはフレームワークバージョンを取得できませんでした。");
        }

        /// <summary>
        /// 宣言されているパッケージを取得します。
        /// </summary>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public static IEnumerable<PackageLibrary> EnumerablePackageLibrary(string packagePath)
        {
            var xml = new XmlDocument();
            xml.Load(packagePath);
            foreach (XmlNode node in xml.SelectNodes("packages/package"))
            {
                XmlElement xmlElement = (XmlElement) node;
                yield return new PackageLibrary()
                {
                    Name = xmlElement.GetAttribute("id"),
                    Version = xmlElement.GetAttribute("version")
                };
            }
        }
    }

    /// <summary>
    /// パッケージ宣言されたライブラリのクラス
    /// </summary>
    public class PackageLibrary
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }
    }
}
