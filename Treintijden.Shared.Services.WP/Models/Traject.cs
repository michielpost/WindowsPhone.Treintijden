using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Models
{
    public class Traject
    {
        public Station From { get; set; }
        public Station To { get; set; }
        public Station Via { get; set; }

        public string UniqueId
        {
            get
            {
                var id = string.Format("{0}_{1}", From.Code, To.Code);
                if (Via != null)
                    id += "_" + Via.Code;

                return id;
            }
        }

        public string FullName
        {
            get
            {
                var id = string.Format("{0} - {1}", From.Name, To.Name);
                if (Via != null)
                    id += " via " + Via.Name;

                return id;
            }
        }
    }
}
