using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wygwam.Windows.Managers;

namespace Wygwam.Windows.Phone.Managers
{
    /// <summary>
    /// Managed the manifest Data
    /// </summary>
    /// <remarks>This class is not ended/remarks>
    public sealed class ManifestManager : BaseManifestManager
    {
        protected override Task<IManifestManager> OnLoading()
        {
            return Load("WMAppManifest.xml");
        }

        public override Task<IManifestManager> Load(string filename)
        {
            return LoadXDocument(XDocument.Load(filename));
        }

        public override Task<IManifestManager> Load(System.IO.Stream data)
        {
            return LoadXDocument(XDocument.Load(data));
        }

        private async Task<IManifestManager> LoadXDocument(XDocument doc)
        {
#warning NOT ENDED

            if (doc != null)
            {
                try
                {
                    var appElement = doc.Root.Descendants(XName.Get("App")).FirstOrDefault();

                    this._manifestInformation.Add("ProductID", null);
                    this._manifestInformation.Add("Title", null);
                    this._manifestInformation.Add("Version", null);
                    this._manifestInformation.Add("Author", null);
                    this._manifestInformation.Add("Description", null);
                    this._manifestInformation.Add("Publisher", null);
                    this._manifestInformation.Add("PublisherID", null);

                    var keys = this._manifestInformation.Keys.ToArray();
                    foreach (var data in keys)
                    {
                        this._manifestInformation[data] = GetAppAttibute(appElement, data);
                    }

                    this.ProductId = this["ProductID"];
                    this.PublisherID = this["PublisherID"];
                    this.PublisherDisplayName = this["Publisher"];
                    this.Version = Version.Parse(this["Version"]);
                    this.DisplayName = this["Title"];
                    this.Description = this["Description"];

                    var ico = doc.Root.Descendants(XName.Get("IconPath")).FirstOrDefault();
                    //this.SplashScreenImage = new Uri("");
                    UriKind kind = UriKind.Relative;

                    var relative = ico.Attribute(XName.Get("IsRelative"));
                    if (relative != null && !Boolean.Parse(relative.Value))
                        kind = UriKind.Absolute;
                    this.Logo = new Uri(ico.Value, kind);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }


            return this;
        }

        private string GetAppAttibute(XElement appElement, string name)
        {
            return appElement.Attributes(XName.Get(name)).FirstOrDefault().Value;
        }
    }
}
