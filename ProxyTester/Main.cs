using System;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
//using Netcrave.ifconfig.me.schema;

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
/// 
namespace Netcrave.ProxyTool.Standalone
{
	class MainClass
	{
		internal static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			SettingsManager.Instance.Log = Console.Out;
			ProxyManager.Instance.Log = Console.Out;
			ProxyManager.Instance.CheckingIfProxyIsWorking += HandleCheckingIfProxyIsWorking;
			ProxyManager.Instance.GetWorkingHttpProxies();
		}
		
		static void HandleCheckingIfProxyIsWorking (object sender, CheckProxyIsWorkingEventArgs e)
        {
			try
			{		
				// First lets check if we can get a connection out to ifconfig.me, get an XML result and unserialize it
				// it fails an exception will likely be thrown and if nothing else the object will have no meaningful data				
				if(!ProxyManager.Instance.TestConnectivityToIfconfigMe(e))
					return;
				else
				{
					Console.WriteLine("here's a working proxy: " + e.httpp.url + ":" + e.httpp.port.ToString());
					e.handled = true;
				}
			}
			catch(Exception wex)
			{
				e.handled = false;
				if(wex.Message.Contains("Forbidden") || wex.Message.Contains("timed out"))
				{					
					Console.WriteLine("Proxy test failed: " + wex.Message);					
				}
				else
				{
					Console.WriteLine("\n-----\n");
					Console.WriteLine("Proxy test failed: " + wex.Message + " : " + wex.StackTrace);					
					Console.WriteLine("\n-----\n");
				}
			}
        }			
	}
}
