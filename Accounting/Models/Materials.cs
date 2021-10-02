//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Accounting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Materials
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Materials()
        {
            this.MaterialStocks = new HashSet<MaterialStocks>();
            this.ProductMaterials = new HashSet<ProductMaterials>();
            this.RecipeMaterials = new HashSet<RecipeMaterials>();
        }
    
        public int materialID { get; set; }
        public string materialName { get; set; }
        public double materialUnitPrice { get; set; }
        public int materialType { get; set; }
        public int materialCurrencyType { get; set; }
        public int materialOperationType { get; set; }
        public int unitID { get; set; }
        public string description { get; set; }
    
        public virtual Units Units { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialStocks> MaterialStocks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMaterials> ProductMaterials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecipeMaterials> RecipeMaterials { get; set; }
    }
}
