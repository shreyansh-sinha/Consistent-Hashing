using ConsistentHashing.Helpers;
using ConsistentHashing.Models;

namespace ConsistentHashing.Service
{
    public class ConsistentHashingService
    {
        private const int MAX_SIZE = 2 ^ 31 - 1;
        private NodeDetails[] consistentHashingRing;
        private Dictionary<int, NodeDetails> posToNodeMap;

        public ConsistentHashingService()
        {
            consistentHashingRing = new NodeDetails[MAX_SIZE];
            posToNodeMap = new Dictionary<int, NodeDetails>();
        }

        public void AddNode(string node_ip)
        {

            int node_ip_hash = Calculations.CalculateMD5(node_ip);
            int pos = node_ip_hash % MAX_SIZE;

            NodeDetails node = new NodeDetails(node_ip, pos);
            consistentHashingRing[pos] = node;

            Console.WriteLine($"Node created for {node_ip} at position {pos}");
            bool flag = false;
            if (posToNodeMap.Count > 0)
            {

                foreach (var kvp in posToNodeMap)
                {
                    NodeDetails neighbourNode = kvp.Value;
                    if (neighbourNode.data.Count > 0)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    // Find the just previous node because we need to update the next node's data
                    Console.WriteLine("Identifying any data migration");
                    int dataStartIdx = pos - 1;
                    while (dataStartIdx != pos)
                    {
                        NodeDetails nodeDetails = consistentHashingRing[dataStartIdx];
                        if (nodeDetails != null)
                        {
                            Console.WriteLine($"Identified data migration for node {node_ip}");
                            break;
                        }
                        dataStartIdx--;
                        if (dataStartIdx < 0)
                        {
                            dataStartIdx = MAX_SIZE - 1;
                        }
                    }

                    // data of the next node from the node just added which lies from index idx+1 to pos-1 should be updated to the new node

                    Console.WriteLine("Get data to migrate");
                    int idx = pos + 1 == MAX_SIZE ? 0 : pos + 1;
                    while (idx != pos)
                    {
                        NodeDetails nextNode = consistentHashingRing[idx];
                        if (nextNode != null)
                        {
                            List<Data> data = nextNode.data;
                            if (data.Count > 0)
                            {
                                Console.WriteLine($"Data to migrate found, Migration started from {nextNode.node_ip} to {node.node_ip}");
                                List<Data> newData = new List<Data>();
                                foreach (var d in data)
                                {
                                    int hash_pos = d.hash_pos;
                                    if (hash_pos <= pos && hash_pos > dataStartIdx)
                                    {
                                        Console.WriteLine($"Migrating {d.data_key} to {node.node_ip} at {node.pos}");
                                        node.data.Add(d);
                                    }
                                    else
                                    {
                                        newData.Add(d);
                                    }
                                }
                                nextNode.data = newData;
                                Console.WriteLine("Data migration done");
                            }
                            break;
                        }
                        idx++;

                        if (idx == MAX_SIZE)
                        {
                            idx = 0;
                        }
                    }
                }
            }

            posToNodeMap.Add(pos, node);
            Console.WriteLine("Node Added Successfully");
        }

        public void AssignNodeToData(string data)
        {

            int data_hash = Calculations.CalculateMD5(data);
            int data_pos = data_hash % MAX_SIZE;
            Data dataToAdd = new Data(data, data_pos);

            int idx = data_pos + 1;
            while (idx != data_pos)
            {
                if (consistentHashingRing[idx] != null)
                {
                    Console.WriteLine($"Got the nearest node for {data}");
                    NodeDetails node = consistentHashingRing[idx];
                    node.data.Add(dataToAdd);
                    Console.WriteLine($"Added the data for {data} to node {node.node_ip} at {node.pos}");
                    break;
                }
                idx++;
                if (idx == MAX_SIZE)
                {
                    idx = 0;
                }
            }
        }
    }
}
