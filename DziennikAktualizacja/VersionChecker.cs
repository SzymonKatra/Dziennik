using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace DziennikAktualizacja
{
    public static class VersionChecker
    {
        public class VersionInfo
        {
            public Version NewestVersion { get; set; }
            public string DownloadLink { get; set; }
        }

        public static VersionInfo CheckVersion(string versionInfoLink)
        {
            return ProcessCheckVersion(versionInfoLink);
        }
        public static void CheckVersionAsync(string versionInfoLink, Action<VersionInfo> callback)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                VersionInfo result = ProcessCheckVersion(versionInfoLink);
                callback(result);
            });
        }

        private static VersionInfo ProcessCheckVersion(string versionInfoLink)
        {
            HttpWebResponse response = null;
            VersionInfo result = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(versionInfoLink);
                request.UserAgent = "DziennikAktualizacja";
                request.Timeout = 10000;

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                string resultString = null;
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    resultString = reader.ReadToEnd();
                }

                XDocument document = XDocument.Parse(resultString);
                XElement root = document.Root;

                result = new VersionInfo();
                result.NewestVersion = Version.Parse(root.Element("version").Value);
                result.DownloadLink = root.Element("download").Value;
            }
            catch
            {
                result = null;
            }
            finally
            {
                if (response != null) response.Close();
            }

            return result;
        }
    }
}
