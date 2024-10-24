using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HtmlReportViewer
{
    internal class ViewModel : ObservableObject
    {
        public string Folder { get { return Get<string>(); } set { Set(value); } }
        public string FilterText { get { return Get<string>(); } set { if (Set(value)) { filterUpdated(); } } }
        public string FilesLabel { get { return Get<string>(); } set { Set(value); } }
        public File SelectedFile { get; set; }

        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();
        public ICollectionView FilteredFiles { get; set; }
        public int FilteredFilesCount { get; set; }

        public ViewModel()
        {
            FilteredFilesCount = 0;
            FilteredFiles = CollectionViewSource.GetDefaultView(Files);
            FilteredFiles.Filter = filteredFilesFilter;

            Folder = "C:\\Program Files\\Unity\\Hub\\Editor\\2021.3.23f1\\Editor\\Data\\Documentation\\en\\Manual";
            FilterText = "";
            FilesLabel = "Files: 0";
        }

        private void FilteredFiles_CurrentChanged(object sender, EventArgs e)
        {
            // SelectedFile = (File)FilteredFiles.CurrentItem;
        }

        public async Task LoadHtmlFilesFromFolder(string folder)
        {
            Folder = folder;

            List<File> filesTemp = new List<File>();

            await Task.Run(() =>
            {
                FilesLabel = String.Format("Files: enumerating...");

                var possibleHtmlFiles = new DirectoryInfo(folder).GetFiles("*.htm*");
                var htmlFiles = possibleHtmlFiles.Where(hf => { var ext = hf.Extension.ToLower(); return ((ext == ".htm") || (ext == ".html")); });

                foreach (var file in htmlFiles)
                {
                    filesTemp.Add(new File(file));
                    FilesLabel = String.Format("Files: {0} loaded...", filesTemp.Count);
                }
            });

            Files.Clear();
            foreach(var file in filesTemp)
            {
                Files.Add(file);
            }

            filterUpdated();
        }

        private void filterUpdated()
        {
            FilteredFilesCount = 0;
            FilesLabel = String.Format("Files: {0} total, filtering...", Files.Count);
            FilteredFiles.Refresh();
            FilesLabel = String.Format("Files: {0} total, {1} matching", Files.Count, FilteredFilesCount);
        }

        private bool filteredFilesFilter(object obj)
        {
            var file = (File)obj;
            
            if(file.Contents.Contains(FilterText))
            {
                FilteredFilesCount++;
                return true;
            }
            return false;
        }

        internal class File
        {
            public string Name { get; set; }
            public string Contents { get; set; }
            public FileInfo Info { get; set; }

            public File(FileInfo fileInfo)
            {
                Name = fileInfo.Name;
                Info = fileInfo;
                Contents = System.IO.File.ReadAllText(fileInfo.FullName);
            }
        }
        
    }
}
