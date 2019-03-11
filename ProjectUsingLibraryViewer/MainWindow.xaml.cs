#region

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ProjectUsingLibraryViewer.ViewModels;
using DataFormats = System.Windows.DataFormats;

#endregion

namespace ProjectUsingLibraryViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <inheritdoc />
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel(this);
            DataContext = viewModel;
            TextBlockFilePath.PreviewDragOver += (sender, args) =>
                {
                    args.Effects = (args.Data.GetDataPresent(DataFormats.FileDrop)
                        ? DragDropEffects.Copy
                        : DragDropEffects.None);
                    args.Handled = true;
                };
            TextBlockFilePath.PreviewDrop += (sender, args) =>
            {
                if (!args.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    return;
                }

                string[] fileNames = (string[]) args.Data.GetData(DataFormats.FileDrop);
                if (fileNames == null)
                {
                    return;
                }
                foreach (var fileName in fileNames)
                {
                    if (!File.Exists(fileName))
                    {
                        continue;                        
                    }
                    var fileInfo = new FileInfo(fileName);
                    if (fileInfo.Extension == ".sln"
                        || fileInfo.Extension == ".csproj")
                    {
                        viewModel.ProjectFilePath.Value = fileInfo.FullName;
                    }
                }
            };
        }




    }
}