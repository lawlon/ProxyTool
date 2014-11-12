using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
//using skmFeedFormatters; // for RSS1.0, 
using System.ServiceModel.Syndication; // for RSS2.0
using System.Threading.Tasks;
using System.Diagnostics;

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
	/// <summary>
	/// Check proxy is working event arguments.
	/// </summary>
	public class CheckProxyIsWorkingEventArgs 
	{
		public bool handled = false;
		public HTTPProxy httpp; 
	}
	
	/// <summary>
	/// Proxy manager.
	/// </summary>
	public class ProxyManager
	{
		private static object syncRoot = new object ();
        private static volatile ProxyManager instance;
		private List<HTTPProxy> HTTPProxies = new List<HTTPProxy>();
		private List<HTTPProxy> GoodProxies = new List<HTTPProxy>();
		public delegate void CheckProxyIsWorkingEventHandler(object sender, CheckProxyIsWorkingEventArgs e);
		public event CheckProxyIsWorkingEventHandler CheckingIfProxyIsWorking;
		
		public TextWriter Log = TextWriter.Null;

        private ProxyManager ()
        {
        }
		
        public static ProxyManager Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ProxyManager ();
                    }

                    return instance;
                }
            }
        }
					
		/// <summary>
		/// Loads the HTTP proxies from RSS feed.
		/// </summary>
		private void LoadHTTPProxiesFromRSSFeed()
		{
			lock(syncRoot)
			{				
				using(WebClient wc = new WebClient())
				{			
					wc.Headers.Add ("User-Agent", SettingsManager.Instance.settings.HTTPUserAgent);
					
				 	using(XmlReader reader = XmlReader.Create (wc.OpenRead(SettingsManager.Instance.settings.ProxyRSSList)))
		            {
						 // Create a new Rss10FeedFormatter object
	                    //Rss10FeedFormatter formatter = new Rss10FeedFormatter ();
						// TODO mine uses 2.
						Rss20FeedFormatter formatter = new Rss20FeedFormatter();
						
	                    // Parse the feed!
	                    formatter.ReadFrom (reader);
	
	                    foreach (var item in formatter.Feed.Items)
	                    {
							string[] spl = item.Title.Text.Split(':');							
							HTTPProxies.Add(new HTTPProxy { url = spl[0], port = int.Parse(spl[1]) });
						}
					}
				}							
			}
		}
		
		/// <summary>
		/// Blacklists the proxy.
		/// </summary>
		/// <param name='prx'>
		/// Prx.
		/// </param>
		private void AddProxyToUsableList(HTTPProxy prx)
		{
			lock(syncRoot)
			{
				GoodProxies.Add(prx);
			}
		}
		
		/// <summary>
		/// Gets the working proxy.
		/// </summary>
		/// <returns>
		/// The working proxy.
		/// </returns>
		public void GetWorkingHttpProxies()
		{
		begin:
			
			Stopwatch stopWatch = new Stopwatch ();
            stopWatch.Start ();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = "";
			
			Log.WriteLine("GetWorkingHttpProxy called");
			if(HTTPProxies.Count() == 0)
			{
				LoadHTTPProxiesFromRSSFeed();
			}
			
			Parallel.ForEach(HTTPProxies, new ParallelOptions { MaxDegreeOfParallelism = 100 }, prx =>
			{
				try 
				{					
					CheckProxyIsWorkingEventArgs args = new CheckProxyIsWorkingEventArgs { handled = false, httpp = prx };
					CheckingIfProxyIsWorking(this, args);
					if(args.handled)
					{
						//Log.WriteLine("found working proxy, logging: " + prx.url + ":" + prx.port.ToString());						
						AddProxyToUsableList(prx);							
					}
					else
					{												
						// throw it on the ground						
					}	
				}
				catch(Exception ex)
				{
					Console.WriteLine("unknown error: " + ex.Message);
				}
			});
		    
			stopWatch.Stop ();
            ts = stopWatch.Elapsed;
            
			elapsedTime = String.Format ("{0:00}:{1:00}:{2:00}.{3:00}",
                                         ts.Hours, ts.Minutes, ts.Seconds,
                                         ts.Milliseconds / 10);
			
			Log.WriteLine("finished parsing proxies: " + elapsedTime);
			
			// keep going
			lock(syncRoot)
			{
				HTTPProxies = new List<HTTPProxy>();
			}
			LoadHTTPProxiesFromRSSFeed();				
			goto begin;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="JobFinder.ProxyManager"/> class.
		/// </summary>
		public void Prime()
		{
			lock(syncRoot)
			{
				LoadHTTPProxiesFromRSSFeed();
			}
		}
	}
}

