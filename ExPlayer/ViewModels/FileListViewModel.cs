using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExPlayer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FileListViewModel : BindableBase
    {
        private FileInfoWrapper selectedItem;
        private bool includeAllFiles = true;
        private int selectedIndex;

        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();

        public int SelectedIndex { get => selectedIndex; set => SetProperty(ref selectedIndex, value); }

        public AudioProvider AudioProvider { get; private set; } = new ();

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
            var list = fileInfoWrappers.ToList();
            if (!IncludeAllFiles)
            {
                list = list.Where(f => f.IsDirectory || f.IsSoundFile()).ToList();
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i].OriginalIndex = i + 1;
                list[i].Index = i + 1;
            }

            Files.Clear();
            Files.AddRange(list);
            AudioProvider.FileInfoWrappers = list;
        }
    }
}