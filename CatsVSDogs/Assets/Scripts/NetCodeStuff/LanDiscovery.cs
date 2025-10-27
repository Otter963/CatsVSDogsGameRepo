using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class LanDiscovery : MonoBehaviour
{
    public int broadcastPort = 47777; // LAN discovery port
    public string broadcastMessage = "NGO_HOST_HERE";

    private UdpClient udpClient;
    private Thread listenThread;
    public string detectedHostIP;

    private void Start()
    {
        detectedHostIP = null;
        StartListening();
    }

    private void OnDestroy()
    {
        listenThread?.Abort();
        udpClient?.Close();
    }

    #region Host
    public void BroadcastHost()
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            using (var client = new UdpClient())
            {
                client.EnableBroadcast = true;
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
                byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);

                while (true)
                {
                    client.Send(data, data.Length, ep);
                    Thread.Sleep(1000); // send every 1 second
                }
            }
        });
    }
    #endregion

    #region Client
    private void StartListening()
    {
        listenThread = new Thread(() =>
        {
            udpClient = new UdpClient(broadcastPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, broadcastPort);

            while (true)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);
                    if (message == broadcastMessage)
                    {
                        detectedHostIP = remoteEP.Address.ToString();
                        Debug.Log("Detected host: " + detectedHostIP);
                    }
                }
                catch { }
            }
        });
        listenThread.IsBackground = true;
        listenThread.Start();
    }
    #endregion
}
