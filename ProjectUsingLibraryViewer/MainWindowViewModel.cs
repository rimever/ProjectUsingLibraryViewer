#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using ProjectUsingLibraryViewer.Utilities;
using Reactive.Bindings;

#endregion

namespace ProjectUsingLibraryViewer
{
    /// <summary>
    /// <see cref="MainWindow"/>のViewModelです。
    /// </summary>
    public class MainWindowViewModel
    {
        /// <inheritdoc />
        public MainWindowViewModel(Window owner)
        {
            ProjectFilePath = new ReactiveProperty<string>();
            ProjectFilePathValidation =
                ProjectFilePath.Select(path => ValidateProjectFilePath(path).Item2).ToReactiveProperty();
            SelectProjectCommand = new ReactiveCommand();
            SelectProjectCommand.Subscribe(_ =>
            {
                var dialog = new OpenFileDialog
                {
                    DefaultExt = "*.csproj",
                    Filter = "ソリューションファイル(*.sln)|*.sln"
                };
                if (dialog.ShowDialog(owner) == true)
                {
                    ProjectFilePath.Value = dialog.FileName;
                }
            });
            CopyMarkdownCommand = new ReactiveCommand();
            CopyMarkdownCommand.Subscribe(_ => { Clipboard.SetText(ProjectSummary.Value); });
            ProjectSummary = ProjectFilePath.Select(GetProjectSummary).ToReactiveProperty();
            SummaryVisibility = ProjectFilePath
                .Select(p => ValidateProjectFilePath(p).Item1 ? Visibility.Visible : Visibility.Hidden)
                .ToReactiveProperty();
        }

        /// <summary>
        /// プロジェクトファイルパス
        /// </summary>
        public ReactiveProperty<string> ProjectFilePath { get; set; }

        /// <summary>
        /// <see cref="ProjectFilePath"/>の検証結果
        /// </summary>
        public ReactiveProperty<string> ProjectFilePathValidation { get; set; }

        /// <summary>
        /// プロジェクト選択処理
        /// </summary>
        public ReactiveCommand SelectProjectCommand { get; set; }

        /// <summary>
        /// クリップボードにコピーする処理
        /// </summary>
        public ReactiveCommand CopyMarkdownCommand { get; set; }

        /// <summary>
        /// プロジェクトの使用ライブラリサマリ
        /// </summary>
        public ReactiveProperty<string> ProjectSummary { get; set; }

        /// <summary>
        /// 使用ライブラリサマリのコンポーネントを表示するかを取得・設定します。
        /// </summary>
        public ReactiveProperty<Visibility> SummaryVisibility { get; set; }

        /// <summary>
        /// プロジェクトサマリを取得・設定します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetProjectSummary(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Extension == ".sln")
            {
                return "* " + string.Join(Environment.NewLine + "* ", EnumerableUsingLibraryOnSolutionFile(fileInfo));
            }

            return "* " + string.Join(Environment.NewLine + "* ", EnumerableUsingLibrary(path));
        }

        private IEnumerable<string> EnumerableUsingLibraryOnSolutionFile(FileInfo fileInfo)
        {
            if (fileInfo.Directory == null)
            {
                yield break;
            }

            using (var stream = new StreamReader(fileInfo.FullName, Encoding.UTF8))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line != null && line.StartsWith("Project("))
                    {
                        var data = line.Split(',');
                        if (data.Length >= 2)
                        {
                            string projectFilePath = Path.Combine(fileInfo.Directory.FullName,
                                data[1].Trim().Replace(@"""", string.Empty));
                            foreach (var text in EnumerableUsingLibrary(projectFilePath))
                            {
                                yield return text;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 使用ライブラリ情報を列挙します
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<string> EnumerableUsingLibrary(string path)
        {
            yield return ProjectFileUtility.GetTargetFramework(path);
            var fileInfo = new FileInfo(path);
            if (fileInfo.Directory?.Exists == true)
            {
                string packagePath = Path.Combine(fileInfo.Directory.FullName, "packages.config");
                foreach (var package in ProjectFileUtility.EnumerablePackageLibrary(packagePath))
                {
                    yield return $"{package.Name} {package.Version}";
                }
            }
        }

        private Tuple<bool, string> ValidateProjectFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new Tuple<bool, string>(false, "プロジェクトファイル名を指定してください。");
            }

            if (!File.Exists(path))
            {
                return new Tuple<bool, string>(false, "指定されたファイルが見つかりません。");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}