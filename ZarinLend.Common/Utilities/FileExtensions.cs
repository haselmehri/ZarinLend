using Common.Exceptions;
using System.IO.Compression;
using static Common.Enums;
using Microsoft.AspNetCore.StaticFiles;

namespace Common.Utilities
{
    public class FileExtensions
    {
        
        public static string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";
             
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
        public static async Task<MemoryStream> GetZipArchive(List<InMemoryFile> files)
        {
            var archiveStream = new MemoryStream();
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open())
                        await zipStream.WriteAsync(file.Content.ToArray(), 0, file.Content.ToArray().Length);
                }
            }
            archiveStream.Seek(0, SeekOrigin.Begin);
            return archiveStream;
        }

        public static async Task<MemoryStream> GetZipArchive(List<FilePath> files)
        {
            var archiveStream = new MemoryStream();
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in files)
                {
                    var fileInfo = new FileInfo(filePath.PhysicalPath);
                    if (!fileInfo.Exists) continue;
                    var zipArchiveEntry = archive.CreateEntry(filePath.FileName, CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        using (var fs = new FileStream(path: filePath.PhysicalPath, FileMode.Open, FileAccess.Read))
                        using (var binaryReader = new BinaryReader(fs))
                        {
                            await zipStream.WriteAsync(binaryReader.ReadBytes((int)fileInfo.Length), 0, (int)fileInfo.Length);
                        }
                    }
                }
            }
            archiveStream.Seek(0, SeekOrigin.Begin);
            return archiveStream;
        }

        public static FileType GetFileType( string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new AppException("{path} cannot be null pr empty!");

            var extension = Path.GetExtension(path).ToLower();
            if (extension == ".jpg" ||
                extension == ".jpeg" ||
                extension == ".png" ||
                extension == ".gif" ||
                extension == ".bmp")
                return FileType.Image;

            if (extension == ".pdf")
                return FileType.Pdf;

            if (extension == ".xls" || extension == ".xlsx")
                return FileType.Excel;

            if (extension == ".doc" || extension == ".docx")
                return FileType.Docs;

            if (extension == ".doc" || extension == ".docx")
                return FileType.Docs;

            if (extension == ".json")
                return FileType.Json;

            if (extension == ".xml")
                return FileType.Xml;

            if (extension == ".mp4")
                return FileType.Video;

            return FileType.Unknown;
        }

        public class InMemoryFile
        {
            public string FileName { get; set; }
            public MemoryStream Content { get; set; }
        }

        public class FilePath
        {
            public string FileName { get; set; }
            public string PhysicalPath { get; set; }
        }
    }
}
