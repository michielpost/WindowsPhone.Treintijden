using System;
using System.Net;
using AgFx;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services.ViewModels.Context
{
    public class NSWebLoadRequest : WebLoadRequest
    {
        public NSWebLoadRequest(LoadContext loadContext, Uri uri)
            : base(loadContext, uri)
        {

        }

        public NSWebLoadRequest(LoadContext loadContext, Uri uri, string method, string data)
            : base(loadContext, uri, method, data)
        {

        }


        //protected override HttpWebRequest CreateWebRequest()
        //{
        //    var baseRequest = base.CreateWebRequest();
        //    //baseRequest.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

        //    return baseRequest;
        //}
    }

    public class NSWebLoadRequest_Auth : WebLoadRequest
    {
        public NSWebLoadRequest_Auth(LoadContext loadContext, Uri uri)
            : base(loadContext, uri)
        {

        }

        public NSWebLoadRequest_Auth(LoadContext loadContext, Uri uri, string method, string data)
            : base(loadContext, uri, method, data)
        {

        }


        protected override HttpWebRequest CreateWebRequest()
        {
            var baseRequest = base.CreateWebRequest();
            baseRequest.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            return baseRequest;
        }
    }
}
