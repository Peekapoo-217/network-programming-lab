// See https://aka.ms/new-console-template for more information

using System.Net;

getLocalHostInfor();


static void getLocalHostInfor()
{
    string hostName = Dns.GetHostName();
    Console.WriteLine("Laptop: "+ hostName);
    IPAddress[] addresses = Dns.GetHostAddresses(hostName);
    foreach(var ip in addresses)
    {
        Console.WriteLine(ip);
    }


}