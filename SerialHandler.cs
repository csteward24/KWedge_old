using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace KWedge
{

    class SerialHandler : IDisposable
    {
        //public delegate void ClosePortEvent();
        //public ClosePortEvent closeport;

        Form1 form;
        SerialPort serialPort;
        string data = "";
        bool running = true;
        public SerialHandler(Form1 form, String port)
        {
            this.form = form;
            serialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.None;
            serialPort.WriteTimeout = 500;
            //_serialPort.ReadTimeout = 2000;
            serialPort.Open();
            //closeport = new ClosePortEvent(ClosePort);
        }



        public void Serial()
        {
            Debug.WriteLine("serial thread started");

            while (running)
            {
                //Thread.Sleep(100);
                try
                {
                    data = serialPort.ReadLine();
                }
                catch (System.IO.IOException)
                {
                    Debug.WriteLine("Could not read serial data");
                    break;
                }
                if (data.Length == 10 || data.Length == 14)
                {
                    Debug.WriteLine(String.Format("Serial Input: {0}", data));
                    if (data.Substring(0, 4).Equals("0000"))
                    {
                        Debug.WriteLine("trimming leading zeros");
                        data = data.Substring(4);
                    }
                    form.Invoke(form.datastate, data);
                }
                else
                {
                    Debug.WriteLine("Improperly formatted input");
                }
            }
        }
        public void Dispose() => serialPort.Close();

        public void ClosePort()
        {
            serialPort.Close();
            running = false;
        }
    }
}
