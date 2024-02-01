using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace StudentLearningHistory.Helpers
{
    public class FileHelper
    {
        private readonly IConfiguration _configuration;
        public FileHelper(IConfiguration configuration)
        {

            _configuration = configuration;

        }
        public bool CheckFileExtensoinWithByte(MemoryStream stream, out string extension)
        {
            bool rt = false;
            extension = string.Empty;

            if (stream.Length > 0)//資料是否為空
            {
                byte[] fileByte = stream.ToArray().Take(4).ToArray();
                byte[] fileMp4Byte = stream.ToArray().Skip(4).Take(4).ToArray();

                int TxtFileLengthLimit = _configuration.GetSection("setting").GetValue<int>("TxtFileLengthLimit");
                int VidioFileLengthLimit = _configuration.GetSection("setting").GetValue<int>("VidioFileLengthLimit");

                string fileFlag = fileByte[0].ToString() + fileByte[1].ToString() + fileByte[2].ToString() + fileByte[3].ToString();
                string fileMp4Flag = fileMp4Byte[0].ToString() + fileMp4Byte[1].ToString() + fileMp4Byte[2].ToString() + fileMp4Byte[3].ToString();

                string[] fileTypeStr = { "255216255224", "137807871", "37806870", };//對應格式jpg,png,pdf
                string[] fileTypeStr2 = { "736851", "255251", "255243", "255242" };//對應格式mp3

                if (fileTypeStr.Contains(fileFlag))
                {
                    if (stream.Length >= TxtFileLengthLimit)
                    {
                        return rt;
                    }

                    switch (Array.IndexOf(fileTypeStr, fileFlag))
                    {
                        case 0:
                            extension = "jpg";
                            break;
                        case 1:
                            extension = "png";
                            break;
                        case 2:
                            extension = "pdf";
                            break;
                    }
                }
                else
                {
                    if (stream.Length >= VidioFileLengthLimit)
                    {
                        return rt;
                    }

                    if (fileMp4Flag.Equals("102116121112"))//對應格式mp4
                    {
                        extension = "mp4";
                    }
                    else if (fileTypeStr2.Contains(fileFlag.Substring(0, 6)))
                    {
                        extension = "mp3";
                    }
                    else
                    {
                        return rt;
                    }
                }
            }

            rt = true;

            return rt;
        }
        public string GetMD5HashFromFile(byte[] file)
        {
            try
            {
                // FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                //ile.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        public async  Task<string> GetFileContentTypeAsync(string fileName)
        {
            return await Task.Run(() =>
            {
                string suffix = Path.GetExtension(fileName);
                var provider = new FileExtensionContentTypeProvider();
                var contentType = provider.Mappings[suffix];
                return contentType;
            });
        }
    }
}
