using System;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace UdpClients
{
    class Program
    {
        static void Main()
        {
            UdpClient udpClient = new UdpClient(8080);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

            try
            {
                Console.WriteLine("Warte auf Video...");

                FileStream outputVideoFileStream = File.OpenWrite("received_video.mp4");

                byte[] buffer = new byte[1024];
                int expectedSequenceNumber = 0;

                while (true)
                {
                    // Empfange Frame mit Sequenznummer
                    byte[] receivedData = udpClient.Receive(ref serverEndPoint);

                    // Überprüfe die Sequenznummer
                    int sequenceNumber = BitConverter.ToInt32(receivedData, 0);
                    if (sequenceNumber == expectedSequenceNumber)
                    {
                        // Schreibe den Frame in die Ausgabedatei
                        outputVideoFileStream.Write(receivedData, 4, receivedData.Length - 4);

                        // Erhöhe die erwartete Sequenznummer für das nächste Frame
                        expectedSequenceNumber++;
                    }
                    else
                    {
                        Console.WriteLine($"Falsche Sequenznummer erwartet: {expectedSequenceNumber}, erhalten: {sequenceNumber}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
