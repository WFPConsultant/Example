namespace UVP.ExternalIntegration.Business
{
    using System.ComponentModel;

    public enum UpdateErpNumberReason
    {
        [Description("1024")]
        NewEntry,

        [Description("1025")]
        GlobalTransfer,

        [Description("1026")]
        Correction,

        [Description("1027")]
        Migration,
    }
}
