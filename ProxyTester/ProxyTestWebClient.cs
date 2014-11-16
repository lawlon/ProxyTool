using System;
using System.Net;

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
	public class ProxyTestWebClient : WebClient
	{
		private int _TimerMilliseconds;
		
		public int TimerMilliseconds
		{
			get
			{
				return _TimerMilliseconds;
			}
			set
			{
				_TimerMilliseconds = value;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="JobFinder.ProxyTestWebClient"/> class.
		/// </summary>
		public ProxyTestWebClient()
		{
			//_TimerMilliseconds = ResetTimeout();
		}
		
		/// <Docs>
		/// To be added.
		/// </Docs>
		/// <returns>
		/// To be added.
		/// </returns>
		/// <summary>
		/// Gets the web request.
		/// </summary>
		/// <param name='uri'>
		/// URI.
		/// </param>
		protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
			//w.Timeout = _TimerMilliseconds;
            return w;
        }
		
		/// <summary>
		/// Resets the timeout
		/// </summary>
		/// <returns>
		/// The timer.
		/// </returns>
		public int ResetTimeout()
		{
			//TODO grab from settings
			return 8000;
		}
	}
}

