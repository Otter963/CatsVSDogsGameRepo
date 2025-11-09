using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LanDiscovery : MonoBehaviour
{
    public int broadcastPort = 47777; // LAN discovery port
    public string broadcastMessage = "NGO_HOST_HERE";
    public string detectedHostIP { get; private set; }

    private bool broadcasting = false;
    private UdpClient udpClient;
    private Thread listenThread;
    private readonly object lockObj = new object();

    private void Start()
    {
        detectedHostIP = null;
        StartListening();
    }

    private void OnDestroy()
    {
        StopBroadcast();
        listenThread?.Abort();
        udpClient?.Close();
    }

    #region Host
    public void BroadcastHost()
    {
        broadcasting = true;

        ThreadPool.QueueUserWorkItem(_ =>
        {
            using (var client = new UdpClient())
            {
                client.EnableBroadcast = true;
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
                byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);

                while (broadcasting)
                {
                    try
                    {
                        client.Send(data, data.Length, ep);
                        Thread.Sleep(1000);
                    }
                    catch (SocketException ex)
                    {
                        Debug.LogWarning("Broadcast error: " + ex.Message);
                    }
                }
            }
        });
    }

    public void StopBroadcast()
    {
        broadcasting = false;
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
                        lock (lockObj)
                        {
                            detectedHostIP = remoteEP.Address.ToString();
                        }
                        Debug.Log("Detected host: " + detectedHostIP);
                    }
                }
                catch { }
            }
        });
        listenThread.IsBackground = true;
        listenThread.Start();
    }

    public string GetDetectedHostIP()
    {
        lock (lockObj)
        {
            return detectedHostIP;
        }
    }
    #endregion
}