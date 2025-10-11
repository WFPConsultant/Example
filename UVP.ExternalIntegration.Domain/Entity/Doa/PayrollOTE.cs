namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class PayrollOTE
    {
        public long PaymentId { get; set; }
        public long ErpAssignmentNumber { get; set; }

        public long ErpPersonId { get; set; }

        public DateTime EffectiveStartDate { get; set; }

        public DateTime EffectiveEndDate { get; set; }

        public long Element_Type_Id { get; set; }

        public long Input_value_id { get; set; }

        public string Input_value_name { get; set; }

        public string Input_value_amount { get; set; }

        public long Currency_value_id { get; set; }

        public  string Currency_value_name { get; set; }

        public long EntrySequence { get; set; }

        public string EntryType { get; set; }

        public bool ErpIsRecurring { get; set; }


    }


    public class PayrollOTEDetails
    {

        public System.Collections.Generic.List<PayrollOTE> PayrollOTEList { get; set; }
    }


}
