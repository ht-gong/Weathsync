using GeoChemApp.Models;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GeoChemApp
{
    public class GetService
    { 
        public async Task RunService(CancellationToken token)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(documents))
                Directory.CreateDirectory(documents);
            var file = Path.Combine(documents, "GeoChemDatapoints.xml");
            var GetService = new APIServices.APIService();


            await Task.Run(async () => {

                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    var list = new List<Env_Datapoint>();

                    list = await GetService.GetLatest();
                    var oldlist = new List<Env_Datapoint>();

                    if(!File.Exists(file))
                    {
                        File.Create(file).Dispose();
                        WritetoXML(file, list);
                        Device.BeginInvokeOnMainThread(() => {
                            CrossLocalNotifications.Current.Show("地化所实时监测", "您收到了新的数据更新");
                        });
                        goto End;
                    }

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(file);
                    if (xDoc.ChildNodes.Count == 0)
                    {
                        WritetoXML(file, list);
                        Device.BeginInvokeOnMainThread(() => {
                            CrossLocalNotifications.Current.Show("地化所实时监测", "您收到了新的数据更新");
                        });
                        goto End;
                    }

                    oldlist = ReadfromXML(file);
                    if(!EqualsAll(oldlist,list))
                    {
                         Device.BeginInvokeOnMainThread(() => {
                                CrossLocalNotifications.Current.Show("地化所实时监测", "您收到了新的数据更新");
                            });

                        WritetoXML(file, list);
                            
                    }

                    End:
                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
            }, token);
        }

        public static List<Env_Datapoint> ReadfromXML(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Env_Datapoint>));
                TextReader textReader = new StreamReader(path);
                List<Env_Datapoint> res =  (List<Env_Datapoint>) serializer.Deserialize(textReader);
                textReader.Dispose();
                return res;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public static void WritetoXML(string path, List<Env_Datapoint> list)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Env_Datapoint>));
                TextWriter textWriter = new StreamWriter(path);
                serializer.Serialize(textWriter, list);
                textWriter.Dispose();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static bool EqualsAll(List<Env_Datapoint> a, List<Env_Datapoint> b)
        {
            if (a == null || b == null)
                return (a == null && b == null);

            if (a.Count != b.Count)
                return false;


            for(int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }
            return true;
        }
    }
}
