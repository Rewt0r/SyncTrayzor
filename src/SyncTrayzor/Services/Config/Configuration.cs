using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SyncTrayzor.Services.Config
{
    public class FolderConfiguration
    {
        public string ID { get; set; }
        public bool IsWatched { get; set; }

        public FolderConfiguration()
        { }

        public FolderConfiguration(string id, bool isWatched)
        {
            this.ID = id;
            this.IsWatched = isWatched;
        }

        public FolderConfiguration(FolderConfiguration other)
        {
            this.ID = other.ID;
            this.IsWatched = other.IsWatched;
        }

        public override string ToString()
        {
            return String.Format("<Folder ID={0} IsWatched={1}>", this.ID, this.IsWatched);
        }
    }

    public class EnvironmentalVariableCollection : Dictionary<string, string>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // Used to use XElement.Load(reader.ReadSubtree()), but that effectively closed the reader
            // and nothing else would get parsed.
            var root = XElement.Parse(reader.ReadOuterXml());
            foreach (var element in root.Elements("Item"))
            {
                this.Add(element.Element("Key").Value, element.Element("Value").Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var elements = this.Select(item =>
            {
                return new XElement("Item",
                    new XElement("Key", item.Key),
                    new XElement("Value", item.Value)
                );
            });
            foreach (var element in elements)
            {
                element.WriteTo(writer);
            }
        }
    }

    [XmlRoot("Configuration")]
    public class Configuration
    {
        public const int CurrentVersion = 3;
        public const double DefaultSyncthingConsoleHeight = 100;

        [XmlAttribute("Version")]
        public int Version
        {
            get { return CurrentVersion; }
            set
            {
                if (CurrentVersion != value)
                    throw new InvalidOperationException(String.Format("Can't deserialize config of version {0} (expected {1})", value, CurrentVersion));
            }
        }

        public bool ShowTrayIconOnlyOnClose { get; set; }
        public bool MinimizeToTray { get; set; }
        public bool CloseToTray { get; set; }
        public bool ShowSynchronizedBalloon { get; set; }
        public bool ShowDeviceConnectivityBalloons { get; set; }
        public string SyncthingAddress { get; set; }
        public bool StartSyncthingAutomatically { get; set; }
        public string SyncthingApiKey { get; set; }
        public EnvironmentalVariableCollection SyncthingEnvironmentalVariables { get; set; }
        public bool SyncthingUseCustomHome { get; set; }
        public bool SyncthingDenyUpgrade { get; set; }
        public bool SyncthingRunLowPriority { get; set; }

        [XmlArrayItem("Folder")]
        public List<FolderConfiguration> Folders { get; set; }

        public bool NotifyOfNewVersions { get; set; }
        public bool ObfuscateDeviceIDs { get; set; }

        [XmlIgnore]
        public Version LatestNotifiedVersion { get; set; }
        [XmlElement("LatestNotifiedVersion")]
        public string LatestNotifiedVersionRaw
        {
            get { return this.LatestNotifiedVersion == null ? null : this.LatestNotifiedVersion.ToString(); }
            set { this.LatestNotifiedVersion = value == null ? null : new Version(value); }
        }

        public bool UseComputerCulture { get; set; }
        public double SyncthingConsoleHeight { get; set; }
        public WindowPlacement WindowPlacement { get; set; }
        public double SyncthingWebBrowserZoomLevel { get; set; }
        public int LastSeenInstallCount { get; set; }
        public string SyncthingPath { get; set; }
        public string SyncthingCustomHomePath { get; set; }

        public Configuration()
        {
            // Default configuration is for a portable setup.

            this.ShowTrayIconOnlyOnClose = false;
            this.MinimizeToTray = false;
            this.CloseToTray = true;
            this.ShowSynchronizedBalloon = true;
            this.ShowDeviceConnectivityBalloons = true;
            this.SyncthingAddress = "localhost:8384";
            this.StartSyncthingAutomatically = true;
            this.SyncthingApiKey = null;
            this.SyncthingEnvironmentalVariables = new EnvironmentalVariableCollection();
            this.SyncthingUseCustomHome = true;
            this.SyncthingDenyUpgrade = false;
            this.SyncthingRunLowPriority = false;
            this.Folders = new List<FolderConfiguration>();
            this.NotifyOfNewVersions = true;
            this.ObfuscateDeviceIDs = true;
            this.LatestNotifiedVersion = null;
            this.UseComputerCulture = true;
            this.SyncthingConsoleHeight = Configuration.DefaultSyncthingConsoleHeight;
            this.WindowPlacement = null;
            this.SyncthingWebBrowserZoomLevel = 0;
            this.LastSeenInstallCount = 0;
            this.SyncthingPath = @"%EXEPATH%\syncthing.exe";
            this.SyncthingCustomHomePath = @"%EXEPATH%\data\syncthing";
        }

        public Configuration(Configuration other)
        {
            this.ShowTrayIconOnlyOnClose = other.ShowTrayIconOnlyOnClose;
            this.MinimizeToTray = other.MinimizeToTray;
            this.CloseToTray = other.CloseToTray;
            this.ShowSynchronizedBalloon = other.ShowSynchronizedBalloon;
            this.ShowDeviceConnectivityBalloons = other.ShowDeviceConnectivityBalloons;
            this.SyncthingAddress = other.SyncthingAddress;
            this.StartSyncthingAutomatically = other.StartSyncthingAutomatically;
            this.SyncthingApiKey = other.SyncthingApiKey;
            this.SyncthingEnvironmentalVariables = other.SyncthingEnvironmentalVariables;
            this.SyncthingUseCustomHome = other.SyncthingUseCustomHome;
            this.SyncthingDenyUpgrade = other.SyncthingDenyUpgrade;
            this.SyncthingRunLowPriority = other.SyncthingRunLowPriority;
            this.Folders = other.Folders.Select(x => new FolderConfiguration(x)).ToList();
            this.NotifyOfNewVersions = other.NotifyOfNewVersions;
            this.ObfuscateDeviceIDs = other.ObfuscateDeviceIDs;
            this.LatestNotifiedVersion = other.LatestNotifiedVersion;
            this.UseComputerCulture = other.UseComputerCulture;
            this.SyncthingConsoleHeight = other.SyncthingConsoleHeight;
            this.WindowPlacement = other.WindowPlacement;
            this.SyncthingWebBrowserZoomLevel = other.SyncthingWebBrowserZoomLevel;
            this.LastSeenInstallCount = other.LastSeenInstallCount;
            this.SyncthingPath = other.SyncthingPath;
            this.SyncthingCustomHomePath = other.SyncthingCustomHomePath;
        }

        public override string ToString()
        {
            return String.Format("<Configuration ShowTrayIconOnlyOnClose={0} MinimizeToTray={1} CloseToTray={2} ShowSynchronizedBalloon={3} " +
                "ShowDeviceConnectivityBalloons={4} SyncthingAddress={5} StartSyncthingAutomatically={6} SyncthingApiKey={7} SyncthingEnvironmentalVariables=[{8}] " +
                "SyncthingUseCustomHome={9} SyncthingDenyUpgrade={10} SyncthingRunLowPriority={11} Folders=[{12}] NotifyOfNewVersions={13} " +
                "LastNotifiedVersion={14} ObfuscateDeviceIDs={15} UseComputerCulture={16} SyncthingConsoleHeight={17} WindowPlacement={18} " +
                "SyncthingWebBrowserZoomLevel={19} LastSeenInstallCount={20} SyncthingPath={21} SyncthingCustomHomePath={22}>",
                this.ShowTrayIconOnlyOnClose, this.MinimizeToTray, this.CloseToTray, this.ShowSynchronizedBalloon, this.ShowDeviceConnectivityBalloons,
                this.SyncthingAddress, this.StartSyncthingAutomatically, this.SyncthingApiKey, String.Join(" ", this.SyncthingEnvironmentalVariables.Select(x => String.Format("{0}={1}", x.Key, x.Value))),
                this.SyncthingUseCustomHome, this.SyncthingDenyUpgrade, this.SyncthingRunLowPriority, String.Join(", ", this.Folders), this.NotifyOfNewVersions,
                this.LatestNotifiedVersion, this.ObfuscateDeviceIDs, this.UseComputerCulture, this.SyncthingConsoleHeight, this.WindowPlacement,
                this.SyncthingWebBrowserZoomLevel, this.LastSeenInstallCount, this.SyncthingPath, this.SyncthingCustomHomePath);
        }
    }
}
