using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using Reactive.Bindings;

namespace ProjectUsingLibraryViewer
{
    /// <summary>
    /// <see cref="MainWindow"/>のViewModelです。
    /// </summary>
    public class MainWindowViewModel
    {
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
        /// プロジェクトの使用ライブラリサマリ
        /// </summary>
        public ReactiveProperty<string> ProjectSummary { get; set; }

        /// <inheritdoc />
        public MainWindowViewModel(Window owner)
        {
            ProjectFilePath = new ReactiveProperty<string>();
            ProjectFilePathValidation =
                ProjectFilePath.Select(path => ValidateProjectFilePath(path).Item2).ToReactiveProperty();
            SelectProjectCommand = new ReactiveCommand();
            SelectProjectCommand.Subscribe(_ =>
            {
                var dialog = new OpenFileDialog()
                {
                    DefaultExt = "*.csproj",
                    Filter = "プロジェクトファイル(*.csproj)|*.csproj"
                };
                if (dialog.ShowDialog(owner) == true)
                {
                    ProjectFilePath.Value = dialog.FileName;
                }
            });
            ProjectSummary = ProjectFilePath.Select(path => GetProjectSummary(path)).ToReactiveProperty();
        }

        private string GetProjectSummary(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            return "* " + string.Join(Environment.NewLine + "* ", EnumerableUsingLibrary(path));
        }

        private IEnumerable<string> EnumerableUsingLibrary(string path)
        {
            //TODO:SDK Versionを取得
            //TODO:package.configからライブラリとバージョンを取得
            yield break;
        }

        private Tuple<bool,string> ValidateProjectFilePath(string path)
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