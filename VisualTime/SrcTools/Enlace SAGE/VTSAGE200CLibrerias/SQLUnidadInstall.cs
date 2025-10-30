using Microsoft.WindowsAzure.Storage.Table;

namespace VTSAGE200CLibrerias
{
    public class SQLUnidadInstall : TableEntity
    {
        public SQLUnidadInstall(string azguard, string version)
        {
            this.PartitionKey = azguard;
            this.RowKey = version;
        }

        public SQLUnidadInstall() { }

        public string Numero { get; set; }
    }
}
