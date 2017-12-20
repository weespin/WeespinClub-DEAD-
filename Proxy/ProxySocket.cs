using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Proxy
{
    public IPAddress ProxyIP { get; set; }
    public ushort ProxyPort { get; set; }
    public Proxy(string proxyIp, ushort proxyPort)
    {
        this.ProxyIP = IPAddress.Parse(proxyIp);
        this.ProxyPort = proxyPort;
    }

    public Socket GetConnection(string destIp, ushort desPort)
    {
        var client = new TcpClient();
        client.Connect(ProxyIP, ProxyPort);

        SendCommand(client.GetStream(), 0x01, destIp, desPort);
        return client.Client;
    }

    private void SendCommand(NetworkStream proxy, byte command, string destinationHost, ushort destinationPort)
    {
        byte[] destIp = IPAddress.Parse(destinationHost).GetAddressBytes();
        byte[] destPort = BitConverter.GetBytes(destinationPort).Reverse().ToArray();
        byte[] userIdBytes = { };
        byte[] request = new byte[9 + userIdBytes.Length];

        request[0] = 0x04;
        request[1] = command;
        destPort.CopyTo(request, 2);
        destIp.CopyTo(request, 4);
        userIdBytes.CopyTo(request, 8);
        request[8 + userIdBytes.Length] = 0x00;

        proxy.Write(request, 0, request.Length);

        Wait(proxy, 10000);

        byte[] response = new byte[8];

        proxy.Read(response, 0, 8);
        if (response[1] != 0x5A)
            throw new Exception("Connection error");
    }

    private void Wait(NetworkStream stream, int timeout)
    {
        int sleepTime = 0;
        while (!stream.DataAvailable)
        {
            Thread.Sleep(50);
            sleepTime += 50;
            if (sleepTime > timeout)
                throw new TimeoutException();
        }
    }
}