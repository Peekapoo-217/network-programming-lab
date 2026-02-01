// See https://aka.ms/new-console-template for more information


using System.Net;
using System.Runtime.Intrinsics.Arm;

string domainName = Console.ReadLine();
GetIpFromAnyDomain(domainName);

static void GetIpFromAnyDomain(string domainName)
{
    try
    {
        IPAddress[] ipAdress = Dns.GetHostAddresses(domainName);
        foreach (IPAddress ip in ipAdress)
        {
            Console.WriteLine($"{ip}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}