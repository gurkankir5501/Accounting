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
    
    public partial class RecipeExpenses
    {
        public int recipeExpID { get; set; }
        public int recipeID { get; set; }
        public string expenseName { get; set; }
        public double spendingAmount { get; set; }
        public double advanceUnitPrice { get; set; }
        public double expenseTotalPrice { get; set; }
    
        public virtual Recipes Recipes { get; set; }
    }
}
