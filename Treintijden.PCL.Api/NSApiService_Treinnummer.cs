using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Constants;
using System.Net.Http;
using Treintijden.PCL.Api.Helpers;
using Newtonsoft.Json;

namespace Treintijden.PCL.Api
{
	public partial class NSApiService : INSApiService
	{
		public async Task<TreinInfo> GetTreinInfo(string treinnummer)
		{
			var date = DateTime.Now;
			string stringDateTime = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T" + date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
			string query = string.Format("{0}?datetime={1}", treinnummer, stringDateTime);


			Uri address = new Uri(string.Format(NSApi.BaseUrl + "/v1/trein/{0}", query), UriKind.Absolute);
			HttpClient webclient = new HttpClient();

			string response = await webclient.GetStringAsync(address);

			return JsonConvert.DeserializeObject<TreinInfo>(response);

		}

	}
}
