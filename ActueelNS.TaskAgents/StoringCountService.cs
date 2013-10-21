using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Net.NetworkInformation;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Phone.Shell;
using ActueelNS.TaskAgents.AsyncWorkloadHelper;
using Treintijden.PCL.Api.Constants;

namespace ActueelNS.TaskAgents
{
    public class StoringCountService
    {

        public static void GetStoringen(ExtendedShellInfo tileData, WorkloadInfo<ExtendedShellInfo, int?> token)
        {
            try
            {
                if (DeviceNetworkInformation.IsNetworkAvailable
                    && (DeviceNetworkInformation.IsWiFiEnabled || DeviceNetworkInformation.IsCellularDataEnabled))
                {
                    string url = NSApi.BaseUrl + "/ns-api-storingen?actual=true&planned=false";

                    if (!string.IsNullOrEmpty(tileData.Name))
                        url = string.Format(NSApi.BaseUrl + "/ns-api-storingen?actual=true&planned=false&station={0}", tileData.Name);

                    Uri address = new Uri(url, UriKind.Absolute);
                    WebClient webclient = new WebClient();
                    //webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

                    webclient.DownloadStringCompleted += (s, e) => webclient_DownloadStringCompleted(e, token);
                    webclient.DownloadStringAsync(address);
                }
                else
                {
                    token.NotifySuccess(null);
                }
            }
            catch (Exception ex)
            {
                token.NotifySuccess(null);
            }
        }

        static void webclient_DownloadStringCompleted(DownloadStringCompletedEventArgs eventArgs, WorkloadInfo<ExtendedShellInfo, int?> token)
        {
            //XElement storingenXmlElement;

            int? resultInt = null;

            try
            {
                if (eventArgs.Error == null &&!string.IsNullOrEmpty(eventArgs.Result))
                {
                    //storingenXmlElement = XElement.Parse(result);

                    int begin = eventArgs.Result.IndexOf("<Ongepland");
                    int end = eventArgs.Result.IndexOf("</Ongepland");

                    if (begin > 0
                        && end > 0
                        && end > begin)
                    {
                        resultInt = CountStringOccurrences(eventArgs.Result.Substring(begin, end - begin), "<Storing");

#if DEBUG
                        //resultInt = 1;
#endif

                        //resultInt = storingenXmlElement.Element("Ongepland").Descendants("Storing").Count();

                        if (resultInt.HasValue && resultInt > 0)
                        {
                            var liveTile = new StandardTileData
                            {
                                Count = resultInt
                            };

                            token.Parameter.ShellTile.Update(liveTile);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
            }
            finally
            {
                //storingenXmlElement = null;
            }

            token.NotifySuccess(resultInt);
            token = null;
        }


        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

       


    }
}