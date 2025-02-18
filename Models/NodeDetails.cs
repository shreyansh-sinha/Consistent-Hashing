namespace ConsistentHashing.Models
{
    public class NodeDetails
    {
        public string node_name;
        public string node_ip;
        public int pos;
        public List<Data> data;

        public NodeDetails(string node_name, string node_ip, int pos)
        {
            this.node_name = node_name;
            this.node_ip = node_ip;
            this.pos = pos;
            this.data = new List<Data>();
        }
    }
}
