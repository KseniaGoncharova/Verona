namespace Verona.Model
{
    public struct AquariumData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public bool NoWorking { get; set; }
        public bool BrokenFilter { get; set; }
    }
}
