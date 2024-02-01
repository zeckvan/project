using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace StudentLearningHistory.Helpers
{
    public class ZipHelper
    {
        public byte[] ZipData(MemoryStream inputStream, string fileName)
        {
            using (MemoryStream zipStream = new())
            {
                using (var zipArchive = ArchiveFactory.Create(ArchiveType.Zip))
                {
                    zipArchive.AddEntry(fileName, inputStream, true);

                    var options = new WriterOptions(CompressionType.Deflate);

                    zipArchive.SaveTo(zipStream, options);
                }
                return zipStream.ToArray();
            }
        }

        public byte[] UnzipData(byte[] zip)
        {
            using (MemoryStream msEntry = new())
            {
                using (MemoryStream msZip = new(zip))
                {
                    using (IArchive archive = ArchiveFactory.Open(msZip))
                    {
                        archive.Entries.ToList().ForEach(entry =>
                        {
                            using (Stream entryStream = entry.OpenEntryStream())
                            {
                                entryStream.CopyTo(msEntry);
                            }
                        });
                    }
                }

                return msEntry.ToArray();
            }
        }

        public List<(string name, byte[] fileBytes)> UnzipData(byte[] zip, string password)
        {
            List<(string name, byte[] mStream)> dict = new();

            ReaderOptions readerOptions = new()
            {
                Password = password,
            };

            using (MemoryStream msZip = new(zip))
            {
                using (IArchive archive = ArchiveFactory.Open(msZip, readerOptions))
                {
                    archive.Entries.ToList().ForEach(entry =>
                    {
                        //e.FullName可取得完整路徑
                        if (string.IsNullOrEmpty(entry.Key)) return;

                        using (Stream entryStream = entry.OpenEntryStream())
                        {
                            using (MemoryStream msEntry = new())
                            {
                                entryStream.CopyTo(msEntry);
                                dict.Add((entry.Key, msEntry.ToArray()));
                            }
                        }
                    });
                }
            }

            return dict;
        }
    }
}
