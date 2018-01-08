using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace Rlc.Utilities
{
	public static class IpLocator
	{
		public class Location
		{
			public bool Status { get; set; }
			public string IP { get; set; }
			public string CountryCode { get; set; }
			public string CountryName { get; set; }
			public string RegionCode { get; set; }
			public string RegionName { get; set; }
			public string City { get; set; }
			public string ZipCode { get; set; }
			public double Latitude { get; set; }
			public double Longitude { get; set; }

			public override string ToString()
			{
				if (Status)
				{
					return CountryName + ", " + RegionName + ", " + City + ", " + IP;
				}
				return "Location Not Found, " + IP;
			}
		}

		private static readonly Dictionary<string, Location> _cachedIps = new Dictionary<string,Location>();

		/// <summary>
		/// Finds the location of specified ip address.
		/// 
		/// Notice: This method requires access to the internet.
		/// </summary>
		/// <param name="ipAddress">The ip address.</param>
		/// <returns></returns>
		public static Location Find(string ipAddress)
		{
			Location location = null;

			// Clean up the input parameter by parsing
			//	and recasting it to a string.
			//
			IPAddress ip = IPAddress.Parse(ipAddress);
			if (ip != null)
			{
				string ipString = ip.ToString();

				// Check the cache...if found, exit early.
				//
				if (_cachedIps.ContainsKey(ipString))
				{
					return _cachedIps[ipString];
				}

				string result = null;
				if (ipString == "127.0.0.1")
				{
					// This is xml in the following format.
					//	This is used when testing on localhost.
					//
					result = @"
				<?xml version='1.0' encoding='UTF-8'?>
				<Response>
					<Status>true</Status>
					<Ip>97.87.12.242</Ip>
					<CountryCode>US</CountryCode>
					<CountryName>United States</CountryName>
					<RegionCode>WI</RegionCode>
					<RegionName>Wisconsin</RegionName>
					<City>Verona</City>
					<ZipCode>53593</ZipCode>
					<Latitude>42.9925</Latitude>
					<Longitude>-89.5679</Longitude>
				</Response>";
					result = result.Replace("\t", "").Replace("\n", "").Replace("\r", "");
				}
				else
				{
					// Need to fetch from service
					//
					try
					{
						using (WebClient webClient = new WebClient())
						{
							string url = string.Format("http://freegeoip.appspot.com/xml/{0}", ipString);
							result = webClient.DownloadString(url);
						}
					}
					catch (Exception ex)
					{
						// Could be the service is down...
						//
					}
				}

				// Need to parse the information
				//
				location = new Location();
				if (!string.IsNullOrEmpty(result))
				{
					try
					{
						XDocument xmlResponse = XDocument.Parse(result);
						XElement responseNode = xmlResponse.Element("Response");
						location.Status = bool.Parse(responseNode.Element("Status").Value);
						location.IP = responseNode.Element("Ip").Value;
						location.CountryCode = responseNode.Element("CountryCode").Value;
						location.CountryName = responseNode.Element("CountryName").Value;
						location.RegionCode = responseNode.Element("RegionCode").Value;
						location.RegionName = responseNode.Element("RegionName").Value;
						location.City = responseNode.Element("City").Value;
						location.ZipCode = responseNode.Element("ZipCode").Value;
						location.Latitude = double.Parse(responseNode.Element("Latitude").Value);
						location.Longitude = double.Parse(responseNode.Element("Longitude").Value);
					}
					catch (Exception ex)
					{
						// Looks like the format changed!
						location = null;
					}
				}
				else
				{
					location = null;
				}

				// Cache the result
				//
				if (location != null)
				{
					_cachedIps.Add(ipString, location);
				}

			}
			return location;
		}
	}
}