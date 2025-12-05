using System;
using System.IO;
using System.Linq;

namespace namasdev.Net.Ftp
{
    public static class FtpEntryHelper
    {
        private const string URI_SEPARATOR = "/";

        public static bool IsFile(string entryName)
        {
            return Path.HasExtension(entryName);
        }

        public static string GetFileNameFromUri(Uri uri)
        {
            return GetEntryNameFromUri(uri);
        }

        public static string GetDirectoryNameFromUri(Uri uri)
        {
            return GetEntryNameFromUri(uri);
        }

        private static string GetEntryNameFromUri(Uri uri)
        {
            return GetEntryNameWithoutDirectory(uri.LocalPath);
        }

        public static string GetEntryNameWithoutDirectory(string entryName)
        {
            return Path.GetFileName(entryName);
        }

        public static string GetDirectoryFullAddress(string[] directoryNames)
        {
            if (directoryNames == null)
            {
                return String.Empty;
            }

            return CombineUriParts(directoryNames
                .Where(it => !String.IsNullOrWhiteSpace(it))
                .Select(it => it.TrimEnd(URI_SEPARATOR.ToCharArray()))
                .ToArray());
        }

        public static Uri BuildEntryUri(Uri uriBase, string directoryName, string entryName)
        {
            return new Uri(uriBase, CombineUriParts(directoryName, entryName));
        }

        public static Uri BuildDirectoryUri(Uri uriBase, string directoryFullAddress)
        {
            if (String.IsNullOrWhiteSpace(directoryFullAddress))
            {
                return uriBase;
            }

            if (!directoryFullAddress.EndsWith(URI_SEPARATOR))
            {
                directoryFullAddress += URI_SEPARATOR;
            }

            return new Uri(uriBase, directoryFullAddress);
        }

        public static string CombineUriParts(params string[] uriParts)
        {
            return String.Join(URI_SEPARATOR, uriParts);
        }
    }
}
