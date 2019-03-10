using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Win32;
using ProjectUsingLibraryViewer.Utilities;
using Reactive.Bindings;

namespace ProjectUsingLibraryViewer
{
    /// <summary>
    /// <see cref="MainWindow"/>��ViewModel�ł��B
    /// </summary>
    public class MainWindowViewModel
    {
        /// <summary>
        /// �v���W�F�N�g�t�@�C���p�X
        /// </summary>
        public ReactiveProperty<string> ProjectFilePath { get; set; }

        /// <summary>
        /// <see cref="ProjectFilePath"/>�̌��،���
        /// </summary>
        public ReactiveProperty<string> ProjectFilePathValidation { get; set; }

        /// <summary>
        /// �v���W�F�N�g�I������
        /// </summary>
        public ReactiveCommand SelectProjectCommand { get; set; }
        /// <summary>
        /// �N���b�v�{�[�h�ɃR�s�[���鏈��
        /// </summary>
public ReactiveCommand CopyMarkdownCommand { get; set; }

        /// <summary>
        /// �v���W�F�N�g�̎g�p���C�u�����T�}��
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
                    Filter = "�v���W�F�N�g�t�@�C��(*.csproj)|*.csproj"
                };
                if (dialog.ShowDialog(owner) == true)
                {
                    ProjectFilePath.Value = dialog.FileName;
                }
            });
            CopyMarkdownCommand = new ReactiveCommand();
            CopyMarkdownCommand.Subscribe(_ => { Clipboard.SetText(ProjectSummary.Value); });
            ProjectSummary = ProjectFilePath.Select(GetProjectSummary).ToReactiveProperty();
        }

        private string GetProjectSummary(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            return "* " + string.Join(Environment.NewLine + "* ", EnumerableUsingLibrary(path));
        }
        /// <summary>
        /// �g�p���C�u��������񋓂��܂�
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

        private Tuple<bool,string> ValidateProjectFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new Tuple<bool, string>(false, "�v���W�F�N�g�t�@�C�������w�肵�Ă��������B");
            }

            if (!File.Exists(path))
            {
                return new Tuple<bool, string>(false, "�w�肳�ꂽ�t�@�C����������܂���B");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}