using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace SimpleEpubReader.Utilities
{
        public static class FileUtility
        {
            public static void ZIPFiles(string sourceDirectory, string zipFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
            {
                try
                {
                    ZipFile.CreateFromDirectory(sourceDirectory, zipFileName, compressionLevel, includeBaseDirectory);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to create zip file for the source directory " + sourceDirectory, ex);
                }
            }


            public static void UnZIPFiles(string zipPath, string extractPath)
            {
                try
                {
                    using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                    {
                        archive.ExtractToDirectory(extractPath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to unzip package " + zipPath + " to " + extractPath, ex);
                }
            }

            public static void DeleteDirectory(string directoryToDelete)
            {
                System.IO.DirectoryInfo dirInfo = new DirectoryInfo(directoryToDelete);
                dirInfo.Delete(true);
            }
        }
    
}
