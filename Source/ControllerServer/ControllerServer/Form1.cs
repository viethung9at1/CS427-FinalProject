using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ControllerServer
{
    enum Behaviour {
        NEUTRAL,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public partial class Form1 : Form
    {
        private static Behaviour tmpBehaviour = Behaviour.NEUTRAL;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RecieveAndControl));
            thread.Start();
            thread.Join();
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private async void RecieveAndControl()
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("100.114.185.38");
            IPAddress iPAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(iPAddress, 7000);
            using Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipEndPoint);
            listener.Listen();
            var handler = await listener.AcceptAsync();
            while (true)
            {
                var buffer = new byte[1024];
                var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, recieved);
                Behaviour key = (Behaviour)int.Parse(response.Last().ToString());
                IntPtr calcWindow = FindWindow(null, "Runner Wheel 3D");
                if (calcWindow != IntPtr.Zero && key != tmpBehaviour)
                {
                    switch (key)
                    {
                        case Behaviour.LEFT:
                            if (SetForegroundWindow(calcWindow))
                                SendKeys.SendWait("{LEFT}");
                            break;
                        case Behaviour.RIGHT:
                            if (SetForegroundWindow(calcWindow))
                                SendKeys.SendWait("{RIGHT}");
                            break;
                        case Behaviour.UP:
                            if (SetForegroundWindow(calcWindow))
                                SendKeys.SendWait("{UP}");
                            break;
                        case Behaviour.DOWN:
                            if (SetForegroundWindow(calcWindow))
                                SendKeys.SendWait("{DOWN}");
                            break;
                    }
                    tmpBehaviour = key;
                }
            }
        }
    }
}