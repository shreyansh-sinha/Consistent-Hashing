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

        public void AddNode(string node_ip, string node_name)
        {

            int node_ip_hash = Calculations.CalculateMD5(node_ip);
            int pos = node_ip_hash % MAX_SIZE;

            NodeDetails node = new NodeDetails(node_name, node_ip, pos);
            consistentHashingRing[pos] = node;

            Console.WriteLine($"Node {node_name} created for {node_ip} at position {pos}");
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
                    int dataStartIdx = pos - 1 <= 0 ? MAX_SIZE - 1 : pos - 1;
                    while (dataStartIdx != pos)
                    {
                        NodeDetails nodeDetails = consistentHashingRing[dataStartIdx];
                        if (nodeDetails != null)
                        {
                            Console.WriteLine($"Identified data migration start index for node name {node_name} with {node_ip} at {dataStartIdx + 1}");
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
                                Console.WriteLine($"Some Data to migrate found, Checking data to find if any data has to be Migrated from {nextNode.node_name} with {nextNode.node_ip} to {node.node_ip}");
                                List<Data> newData = new List<Data>();
                                bool isDataMigrated = false;

                                foreach (var d in data)
                                {
                                    int hash_pos = d.hash_pos;
                                    if (hash_pos <= pos && hash_pos > dataStartIdx)
                                    {
                                        isDataMigrated = true;
                                        Console.WriteLine($"Migrating {d.data_key} to {node.node_name} with {node.node_ip} at {node.pos}");
                                        node.data.Add(d);
                                    }
                                    else
                                    {
                                        newData.Add(d);
                                    }
                                }

                                nextNode.data = newData;
                                if(isDataMigrated)
                                {
                                    Console.WriteLine("Data migration done");
                                }
                                else
                                {
                                    Console.WriteLine("No data to migrate found");
                                }
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

            Console.WriteLine($"Assigning {data} hashed at pos {data_pos} started");
            Data dataToAdd = new Data(data, data_pos);

            int idx = data_pos + 1 == MAX_SIZE ? 0 : data_pos + 1;
            while (idx != data_pos)
            {
                NodeDetails currentNode = consistentHashingRing[idx];
                if (currentNode != null)
                {
                    Console.WriteLine($"Got the nearest node for {data} => {currentNode.node_name} with {currentNode.node_ip}");
                    currentNode.data.Add(dataToAdd);
                    Console.WriteLine($"Added the data for {data} to node {currentNode.node_name} with {currentNode.node_ip} at {currentNode.pos}");
                    break;
                }
                idx++;
                if (idx == MAX_SIZE)
                {
                    idx = 0;
                }
            }
        }

        public void DeleteNode(string node_ip)
        {
            Console.WriteLine($"Deletion started for {node_ip}");
            foreach(var kvp in posToNodeMap)
            {
                NodeDetails node = kvp.Value;
                List<Data> dataToMove = node.data;
                if (node.node_ip == node_ip)
                {
                    Console.WriteLine($"Node {node.node_name} with {node.node_ip} found at position {node.pos}");
                    posToNodeMap.Remove(node.pos);
                    consistentHashingRing[node.pos] = null;

                    int currentIdx = node.pos + 1;
                    while (currentIdx != node.pos)
                    {
                        NodeDetails currentNode = consistentHashingRing[currentIdx];
                        if (currentNode != null)
                        {
                            Console.WriteLine($"Data migration started from {node.node_name} with {node.node_ip} at index {node.pos} to {currentNode.node_name} with {currentNode.node_ip} at {currentNode.pos}");
                            foreach(var d in dataToMove)
                            {
                                Console.WriteLine($"Adding data {d.data_key} to the new node {currentNode.node_name} with {currentNode.node_ip} at pos {currentNode.pos}");
                                currentNode.data.Add(d);
                            }
                            Console.WriteLine($"Data added to new node {currentNode.node_name} successfully");
                            break;
                        }
                        currentIdx++;

                        if (currentIdx == MAX_SIZE)
                        {
                            currentIdx = 0;
                        }
                    }
                    Console.WriteLine($"Node {node.node_name} with {node.node_ip} deleted successfully");
                    break;
                }
            }
        }
    }
}
