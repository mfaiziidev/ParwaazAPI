using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ParwaazAPI
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class DecimalFormatAttribute : Attribute
    {
        public int Precision { get; set; }
        public int Scale { get; set; }

        public DecimalFormatAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
    public class clsParwaaz
    {
        public int id { get; set; }

        [Required]
        [StringLength(30)]
        public string AccountNo { get; set; }

        [Required(ErrorMessage ="Please Enter Debit Amount Between the Decimal(12,2)")]
        [DecimalFormat(12,2)]
        public decimal CreditAmount { get; set; }

        [Required(ErrorMessage = "Please Enter Credit Amount Between the Decimal(12,2)")]
        [DecimalFormat(12, 2)]
        public decimal DebitAmount { get; set; }

        [Required]
        [StringLength(15)]
        public string EntryID{ get; set; }

        [Required(ErrorMessage="Please Enter a Valid Date format(mm/dd/yyyy)")]
        [DataType(DataType.DateTime)]
        public DateTime PostingDate {get;set;}

        [Required]
        [StringLength(50)]
        public string DocumentType {get;set;}
        
        [Required]
        [StringLength(20)]
        public string DocumentNo { get; set; }
        
        [Required]
        [StringLength(30)]
        public string AccountType {get;set;}
        public string Comment { get; set; }
        public string ShortcutDimension1Code { get; set; }
        public string ShortcutDimension2Code { get; set; }
        public string ShortcutDimension3Code { get; set; }
        //public string ExternalDocumentNo { get; set; }
        
    }

    public class clsResponse
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
    }
    public class clsBank
    {
        public string BankNo { get; set; }
        public string BankName { get; set; }
    }

    public class clsError
    {
        public string EntryID { get; set; }
        //public string ErrorMsg { get; set; }
        public string DocumentNo { get; set; }

    }

    public class clsChartOfAccount
    {
        public string No { get; set; }
        public string Name { get; set; }
    }
}