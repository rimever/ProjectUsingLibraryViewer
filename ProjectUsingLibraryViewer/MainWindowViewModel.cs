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
            //TODO:SDK Version���擾
            //TODO:package.config���烉�C�u�����ƃo�[�W�������擾
            yield break;
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