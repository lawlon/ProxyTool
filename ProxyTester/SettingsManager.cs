using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

/// Author : Paige Thompson (paigeadele@gmail.com)
/// Proxy Tool
/// Copyright 2014
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.

/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.

/// You should have received a copy of the GNU General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
/// </summary>
namespace Netcrave.ProxyTool
{
    public class SettingsManager
    {
        public Settings settings
        {
            set;
            get;
        }

        private static volatile SettingsManager instance;
        private static object syncRoot = new object ();
        public TextWriter Log = TextWriter.Null;

        private SettingsManager ()
        {
            LoadSettings();
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public void LoadSettings()
        {
            bool newConfig = false;

            if (!File.Exists ("proxytool_settings.xml"))
            {
                newConfig = true;
            }

            using (FileStream file = File.Open("proxytool_settings.xml", FileMode.OpenOrCreate))
            {
                if (newConfig)
                {
                    settings = new Settings
                    {                   
						ProxyRSSList = "http://tools.rosinstrument.com/proxy/plab100.xml",
						HTTPUserAgent = "Mozilla/5.0 (X11; Linux x86_64; rv:31.0) Gecko/20100101 Firefox/31.0",
						UseAnonymousProxiesOnly = true, // zealous spam filters check X-Forward-For
						MaxThreads = 30,
                    };
                    var serializer = new XmlSerializer (typeof(Settings));
                    using (XmlWriter writer = XmlWriter.Create(file, new XmlWriterSettings { Indent = true, NewLineOnAttributes = true}))
                    {
                        serializer.Serialize (writer, settings);
                    }
                }

                else
                {
                    var serializer = new XmlSerializer (typeof(Settings));
                    using (XmlReader xr = XmlReader.Create(file))
                    {
                        this.settings = (Settings)serializer.Deserialize (xr);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void SaveSettings ()
        {
            throw new NotImplementedException();
        }

        public static SettingsManager Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new SettingsManager ();
                    }

                    return instance;
                }
            }
        }
    }
}

