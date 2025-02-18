namespace ConsistentHashing.Models
{
    public class Data
    {
        public string data_key;
        public int hash_pos;

        public Data(string data_key, int hash_pos)
        {
            this.data_key = data_key;
            this.hash_pos = hash_pos;
        }
    }
}
