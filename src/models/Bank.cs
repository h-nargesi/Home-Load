namespace Photon.HomeLoad;

struct Bank
{
    public int BankOrganizationUnitId { get; set; }
    public string Title { get; set; }

    public struct Result
    {
        public BankCollection Data { get; set; }
        public string Message { get; set; }
    }

    public struct BankCollection
    {
        public Bank[] BankData { get; set; }
        public long MaxLoanAmount { get; set; }
    }
}
