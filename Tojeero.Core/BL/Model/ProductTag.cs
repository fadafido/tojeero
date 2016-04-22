using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class ProductTag : IUniqueEntity
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Tag { get; set; }
    }
}