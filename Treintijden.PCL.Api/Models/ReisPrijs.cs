
namespace Treintijden.PCL.Api.Models
{
    public class ReisPrijs
    {
        public string Enkel_1_Vol { get; set; }
        public string Enkel_1_20 { get; set; }
        public string Enkel_1_40 { get; set; }
        
        public string Enkel_2_Vol { get; set; }
        public string Enkel_2_20 { get; set; }
        public string Enkel_2_40 { get; set; }

        public string Dag_1_Vol { get; set; }
        public string Dag_1_20 { get; set; }
        public string Dag_1_40 { get; set; }

        public string Dag_2_Vol { get; set; }
        public string Dag_2_20 { get; set; }
        public string Dag_2_40 { get; set; }

        public bool IsEmpty
        {
          get
          {
            return
              string.IsNullOrEmpty(this.Enkel_1_Vol)
              && string.IsNullOrEmpty(this.Enkel_1_20)
              && string.IsNullOrEmpty(this.Enkel_1_40)

              && string.IsNullOrEmpty(this.Enkel_2_Vol)
              && string.IsNullOrEmpty(this.Enkel_2_20)
              && string.IsNullOrEmpty(this.Enkel_2_40)

              && string.IsNullOrEmpty(this.Dag_1_Vol)
              && string.IsNullOrEmpty(this.Dag_1_20)
              && string.IsNullOrEmpty(this.Dag_1_40)

              && string.IsNullOrEmpty(this.Dag_2_Vol)
              && string.IsNullOrEmpty(this.Dag_2_20)
              && string.IsNullOrEmpty(this.Dag_2_40)
              ;
          }
        }

    }
}
