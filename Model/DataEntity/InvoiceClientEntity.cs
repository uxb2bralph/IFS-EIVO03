namespace Model.DataEntity
{
    public partial class InvoiceClientEntityDataContext
    {
        partial void OnCreated()
        {
            this.CommandTimeout = 86400;
        }
    }
}