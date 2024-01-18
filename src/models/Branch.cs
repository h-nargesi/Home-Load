namespace Photon.HomeLoad;

struct Branch
{
    public int OrganizationUnitId { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }

    public struct Result
    {
        public Branch[] Data { get; set; }
        public string Message { get; set; }
    }
}
