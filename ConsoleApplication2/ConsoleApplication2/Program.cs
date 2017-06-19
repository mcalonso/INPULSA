// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System.IO;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;

    public static void Main()
    {
        string name;
        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);

        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();

        // Allow the user to set the appropriate properties.
        _serialPort.PortName = SetPortName(_serialPort.PortName);
        _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
        _serialPort.Parity = SetPortParity(_serialPort.Parity);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

        // Set the read/write timeouts
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;

        _serialPort.Open();
        _continue = true;
        readThread.Start();

        name = "C4";

        Console.WriteLine("Type QUIT to exit");

        while (_continue)
        {
            message = Console.ReadLine();

            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {

                int counter = 0;
                string line;

                // Read the file and display it line by line.
                System.IO.StreamReader file =
                   new System.IO.StreamReader(@"C:\SerialLogs\PackageInfo.bin");
                while ((line = file.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    counter++;
                }

                file.Close();



                _serialPort.WriteLine(
                    //String.Format("<{0}>: {1}", name, message));
                    String.Format(message));
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                int message = _serialPort.ReadChar();

                try
                {
                    System.IO.Directory.CreateDirectory(@"C:\SerialLogs");
                    string pathString = System.IO.Path.Combine(@"C:\SerialLogs", "PackageInfo.bin");
                    string pathString2 = System.IO.Path.Combine(@"C:\SerialLogs", "PackageInfoBin.bin");

                    // Create the file.
                    if (!File.Exists(pathString))
                    {
                        using (FileStream fs = File.Create(pathString))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(((char)message).ToString());
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);

                        }
                    }else
                    {
                        // Open the stream and read it back.
                        using (FileStream fs = File.Open(pathString, FileMode.Open))
                        {
                            Byte[] info2 = new UTF8Encoding(true).GetBytes(((char)message).ToString());
                            // Add some information to the file.
                            fs.Seek(0, SeekOrigin.End);
                            fs.Write(info2, 0, info2.Length);
                        }
                    }



                    // Create the file.
                    if (!File.Exists(pathString2))
                    {
                        using (FileStream fs = File.Create(pathString2))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(message.ToString()+" ");
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);

                        }
                    }
                    else
                    {
                        // Open the stream and read it back.
                        using (FileStream fs = File.Open(pathString2, FileMode.Open))
                        {
                            Byte[] info2 = new UTF8Encoding(true).GetBytes(message.ToString()+" ");
                            // Add some information to the file.
                            fs.Seek(0, SeekOrigin.End);
                            fs.Write(info2, 0, info2.Length);
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            catch (TimeoutException) { }
        }
    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        //string portName= "COM4";
        string portName = Console.ReadLine();

        if (portName == "" || !(portName.ToLower()).StartsWith("com"))
        {
            portName = defaultPortName;
        }

        Console.WriteLine("\nPort:  " + portName);

        return portName;
    }

    // Display BaudRate values and prompt user to enter a value.
    public static int SetPortBaudRate(int defaultPortBaudRate)
    {
        string baudRate = "9600";

        if (baudRate == "")
        {
            baudRate = defaultPortBaudRate.ToString();
        }

        return int.Parse(baudRate);
    }

    // Display PortParity values and prompt user to enter a value.
    public static Parity SetPortParity(Parity defaultPortParity)
    {
        string parity = "None";

        if (parity == "")
        {
            parity = defaultPortParity.ToString();
        }

        return (Parity)Enum.Parse(typeof(Parity), parity, true);
    }

    // Display DataBits values and prompt user to enter a value.
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits = "8";

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits = "One";

        if (stopBits == "")
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }

    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake = "None";

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }
}