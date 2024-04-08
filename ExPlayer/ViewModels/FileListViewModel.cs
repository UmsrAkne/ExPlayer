using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExPlayer.Models;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FileListViewModel : BindableBase
    {
        private FileInfoWrapper selectedItem;
        private bool includeAllFiles = true;

        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();

        public FileInfoWrapper SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        /// <summary>
        /// ReplaceFileInfoWrappers() で値を入力する際、サウンドファイルやディレクトリ以外のファイルもリストに入力するかを設定します。
        /// </summary>
        /// <value>
        /// デフォルトは true です。この状態は、全てのファイルの入力を受け付けます。
        /// </value>
        public bool IncludeAllFiles { get => includeAllFiles; set => SetProperty(ref includeAllFiles, value); }

        /// <summary>
        /// Files のリストを、入力したリストに置き換えます。
        /// IncludeAllFiles == true の場合、不要なファイルはリストから除外されます。
        /// </summary>
        /// <param name="fileInfoWrappers">FileInfoWrapper のリストを入力します</param>
        public void ReplaceFileInfoWrappers(IEnumerable<FileInfoWrapper> fileInfoWrappers)
        {
            if (!IncludeAllFiles)
            {
                fileInfoWrappers = fileInfoWrappers.Where(f => f.IsDirectory || f.IsSoundFile());
            }

            Files.Clear();
            Files.AddRange(fileInfoWrappers);
        }
    }
}