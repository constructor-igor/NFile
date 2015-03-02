using System.Collections.Generic;

namespace NFile.Tests.NCsvFileTests
{
    public class DataItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double Growth { get; set; }
        public override bool Equals(object obj)
        {
            DataItem other = (DataItem) obj;
            return Name == other.Name && Id == other.Id && Growth == other.Growth;
        }

        protected bool Equals(DataItem other)
        {
            return string.Equals(Name, other.Name) && Id == other.Id && Growth.Equals(other.Growth);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ Growth.GetHashCode();
                return hashCode;
            }
        }
    }

    public class DataObject
    {
        public string VersionId { get; set; }
        public List<DataItem> Items { get; set; }
        public override bool Equals(object obj)
        {
            DataObject other = (DataObject) obj;
            return VersionId == other.VersionId && Items.Equals(other.Items);
        }

        protected bool Equals(DataObject other)
        {
            return string.Equals(VersionId, other.VersionId) && Equals(Items, other.Items);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((VersionId != null ? VersionId.GetHashCode() : 0) * 397) ^ (Items != null ? Items.GetHashCode() : 0);
            }
        }
    }
}