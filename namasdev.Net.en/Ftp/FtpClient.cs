using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    public class FtpClient
    {
        private const int FILE_UPLOAD_BUFFER_SIZE = 2048;
        private const int FILE_DOWNLOAD_BUFFER_SIZE = 2048;

        public FtpClient(Uri uri)
        {
            Validator.ValidateRequiredArgumentAndThrow(uri, nameof(uri));

            this.Uri = uri;
        }

        public Uri Uri { get; private set; }
        public ICredentials Credentials { get; set; }

        public IFtpEntry[] GetEntries(string directoryName)
        {
            var entryNames = GetEntryNames(directoryName);

            var entries = new List<IFtpEntry>();
            string entryNameWithoutDirectory = null;
            Uri entryUri = null;
            foreach (var entryName in entryNames)
            {
                entryNameWithoutDirectory = FtpEntryHelper.GetEntryNameWithoutDirectory(entryName);
                entryUri = FtpEntryHelper.BuildEntryUri(this.Uri, directoryName, entryNameWithoutDirectory);

                if (FtpEntryHelper.IsFile(entryName))
                {
                    entries.Add(new FtpFile(entryUri));
                }
                else
                {
                    entries.Add(new FtpDirectory(entryUri, GetEntries(FtpEntryHelper.CombineUriParts(directoryName, entryNameWithoutDirectory))));
                }
            }

            return entries.ToArray();
        }

        private string[] GetEntryNames(string directory = null)
        {
            var request = BuildRequestWithCredentials(new Uri(this.Uri, directory), WebRequestMethods.Ftp.ListDirectory);

            var res = new List<string>();

            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    res.Add(reader.ReadLine());
                }

                return res.ToArray();
            }
        }

        public void DownloadFile(Uri uri, string destinationPath)
        {
            var request = BuildRequestWithCredentials(uri, WebRequestMethods.Ftp.DownloadFile);

            request.UseBinary = true;
            request.Proxy = null;

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidateResponseStatusCode(response);

                using (var responseStream = response.GetResponseStream())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    byte[] buffer = new byte[FILE_DOWNLOAD_BUFFER_SIZE];
                    while(true)
                    {
                        int readCount = responseStream.Read(buffer, 0, buffer.Length);
                        if (readCount == 0)
                        {
                            break;
                        }

                        fileStream.Write(buffer, 0, readCount);
                    }
                }
            }
        }

        public void DeleteFile(Uri uri)
        {
            var request = BuildRequestWithCredentials(uri, WebRequestMethods.Ftp.DeleteFile);

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidateResponseStatusCode(response);
            }
        }

        public void DeleteDirectory(Uri uri)
        {
            var request = BuildRequestWithCredentials(uri, WebRequestMethods.Ftp.RemoveDirectory);
            
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidateResponseStatusCode(response);
            }
        }

        public void CreateDirectoryIfNotExists(string[] directories)
        {
            var uri = FtpEntryHelper.BuildDirectoryUri(
                uriBase: this.Uri,
                directoryFullAddress: FtpEntryHelper.GetDirectoryFullAddress(directories)
            );

            try
            {
                var request = BuildRequestWithCredentials(uri, WebRequestMethods.Ftp.MakeDirectory);

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    ValidateResponseStatusCode(response, FtpStatusCode.PathnameCreated);
                }
            }
            catch (WebException ex)
            {
                // This happens when the folder is already created...
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw;
                }
            }
        }

        public void UploadFile(string filePath,
            string fileName = null, 
            string[] directories = null)
        {
            var uri = FtpEntryHelper.BuildEntryUri(
                uriBase: this.Uri,
                directoryName: FtpEntryHelper.CombineUriParts(directories),
                entryName: fileName ?? Path.GetFileName(filePath)
            );

            var request = BuildRequestWithCredentials(uri, WebRequestMethods.Ftp.UploadFile);

            request.UseBinary = true;
            request.Proxy = null;

            using (var requestStream = request.GetRequestStream())
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[FILE_UPLOAD_BUFFER_SIZE];
                while (true)
                {
                    int readCount = fileStream.Read(buffer, 0, buffer.Length);
                    if (readCount == 0)
                    {
                        break;
                    }

                    requestStream.Write(buffer, 0, readCount);
                }
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidateResponseStatusCode(response, FtpStatusCode.ClosingData);
            }
        }

        private FtpWebRequest BuildRequestWithCredentials(Uri uri, string method)
        {
            var request = (FtpWebRequest)FtpWebRequest.CreateDefault(uri);
            request.Method = method;

            if (this.Credentials != null)
            {
                request.Credentials = this.Credentials;
            }

            return request;
        }

        private void ValidateResponseStatusCode(FtpWebResponse response,
            FtpStatusCode expectedStatusCode = FtpStatusCode.FileActionOK)
        {
            if (response.StatusCode != expectedStatusCode)
            {
                throw new FtpException(response.StatusCode, response.StatusDescription);
            }
        }

        public override string ToString()
        {
            return this.Uri.AbsoluteUri;
        }
    }
}
