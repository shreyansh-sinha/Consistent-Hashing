using System;
using ConsistentHashing.Helpers;
using ConsistentHashing.Service;

public class Program
{
    public static void Main(String[] args)
    {
        ConsistentHashingService consistentHashing = new ConsistentHashingService();

        string nodea_key = Calculations.GenerateUniqueRandomString();
        string nodeb_key = Calculations.GenerateUniqueRandomString();
        string nodec_key = Calculations.GenerateUniqueRandomString();
        string noded_key = Calculations.GenerateUniqueRandomString();

        consistentHashing.AddNode(nodea_key, "NODE_A");
        consistentHashing.AddNode(nodeb_key, "NODE_B");

        string dataa = "Hyderabad";
        string datab = "Bengaluru";
        string datac = "Chennai";

        consistentHashing.AssignNodeToData(dataa);
        consistentHashing.AssignNodeToData(datab);
        consistentHashing.AssignNodeToData(datac);

        consistentHashing.AddNode(nodec_key, "NODE_C");
        consistentHashing.AddNode(noded_key, "NODE_D");

        consistentHashing.DeleteNode(nodeb_key);
    }
}