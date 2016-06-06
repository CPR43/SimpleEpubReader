using Microsoft.Win32;
using SimpleEpubReader.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
//using System.Windows.Shapes;

namespace SimpleEpubReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _tempPath;
        private string _baseMenuXmlDiretory;
        private List<string> _menuItems;
        private int _currentPage;

        public MainWindow()
        {
            InitializeComponent();
            _menuItems = new List<string>();
            NextButton.Visibility = Visibility.Hidden;
            PreviousButton.Visibility = Visibility.Hidden;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Epub files (*.epub)|*.epub|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                if (!Directory.Exists("Library"))
                {
                    Directory.CreateDirectory("Library");
                }
                File.Copy(openFileDialog.FileName, Path.Combine("Library", fileName + ".zip"), true);
                _tempPath = Path.Combine("Library", fileName);
                if (Directory.Exists(_tempPath))
                {
                    FileUtility.DeleteDirectory(_tempPath);
                }
                FileUtility.UnZIPFiles(Path.Combine("Library", fileName + ".zip"), Path.Combine("Library", fileName));

                var containerReader = XDocument.Load(ConvertToMemmoryStream(Path.Combine("Library", fileName, "META-INF", "container.xml")));

                var baseMenuXmlPath = containerReader.Root.Descendants(containerReader.Root.GetDefaultNamespace() + "rootfile").First().Attribute("full-path").Value;
                XDocument menuReader = XDocument.Load(Path.Combine(_tempPath, baseMenuXmlPath));
                _baseMenuXmlDiretory = Path.GetDirectoryName(baseMenuXmlPath);
                var menuItemsIds = menuReader.Root.Element(menuReader.Root.GetDefaultNamespace() + "spine").Descendants().Select(x => x.Attribute("idref").Value).ToList();
                _menuItems = menuReader.Root.Element(menuReader.Root.GetDefaultNamespace() + "manifest").Descendants().Where(mn => menuItemsIds.Contains(mn.Attribute("id").Value)).Select(mn => mn.Attribute("href").Value).ToList();
                _currentPage = 0;
                string uri = GetPath(0);
                epubDisplay.Navigate(uri);
                NextButton.Visibility = Visibility.Visible;
            }
        }

        public MemoryStream ConvertToMemmoryStream(string fillPath)
        {
            var xml = File.ReadAllText(fillPath);
            byte[] encodedString = Encoding.UTF8.GetBytes(xml);

            // Put the byte array into a stream and rewind it to the beginning
            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;

            return ms;
        }

        public string GetPath(int index)
        {
            return String.Format("file:///{0}", Path.GetFullPath(Path.Combine(_tempPath, _baseMenuXmlDiretory, _menuItems[index])));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _menuItems.Count - 1)
            {
                _currentPage++;
            }
            else
            {
                NextButton.Visibility = Visibility.Hidden;
            }
            if (_currentPage == _menuItems.Count - 1)
            {
                NextButton.Visibility = Visibility.Hidden;
            }
            if (_currentPage > 0)
            {

                PreviousButton.Visibility = Visibility.Visible;
            }
            string uri = GetPath(_currentPage);
            epubDisplay.Navigate(uri);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage >= 1)
            {
                _currentPage--;
            }
            else
            {

                PreviousButton.Visibility = Visibility.Hidden;
            }
            if (_currentPage == 1)
            {
                PreviousButton.Visibility = Visibility.Hidden;
            }
            if (_currentPage <= _menuItems.Count - 1)
            {
                NextButton.Visibility = Visibility.Visible;
            }
            string uri = GetPath(_currentPage);
            epubDisplay.Navigate(uri);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
