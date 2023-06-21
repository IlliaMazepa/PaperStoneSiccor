using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Runtime.Remoting.Messaging;

namespace ExamNPClient
{
    public partial class Form1 : Form
    {
        private bool _done = true;
        private UdpClient _udpClient;
        private IPAddress _groupAddress;
        private int _localPort;
        private int _remotePort;
        private int _ttl;
        private int res=-1;
        private int roundA;
        private int roundS;
        private List<ComboBox> list = new List<ComboBox>();

        private IPEndPoint _remoteEP;
        private UTF8Encoding _encoding;
        private string _name;
        private int ans;
        public Form1()
        {
            InitializeComponent();
            list.Add(cbFirR);
            list.Add(cbSecR);
            list.Add(cbThiR);
            list.Add(cbFouR);
            list.Add(cbFifR);
            foreach(ComboBox c in list)
            {
                c.Items.Add("Scissors");
                c.Items.Add("Stone");
                c.Items.Add("Paper");
            }
            _encoding = new UTF8Encoding();
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _groupAddress = IPAddress.Parse("234.5.6.11");
                _ttl = 128;
                roundA = 1;
                roundS = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
            Task.Run(ResWait);
        }

        private void btnStart_Checker(object sender, EventArgs e)
        {
            if (tbLocalPort.Text != "" && tbRemotePort.Text != "")
            {
                btnStart.Enabled = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            cbFirR.Enabled = true;
            tbOpFirR.Enabled = true;
            tbResFirR.Enabled = true;
            _localPort = Convert.ToInt32(tbLocalPort.Text);
            _remotePort = Convert.ToInt32(tbRemotePort.Text);

            try
            {
                _udpClient = new UdpClient(_localPort);

                _udpClient.JoinMulticastGroup(_groupAddress, _ttl);

                _remoteEP = new IPEndPoint(_groupAddress, _remotePort);

                Task.Run(Listener);

                

                btnStart.Enabled = false;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Listener()
        {
            _done = false;
            try
            {
                while (!_done)
                {
                    IPEndPoint endPoint = null;
                    res = Convert.ToInt32(_encoding.GetString(_udpClient.Receive(ref endPoint)));

                    //BeginInvoke(new Action(() =>
                    //{

                    //})
                    //);
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode != 10004) 
                {
                    MessageBox.Show(se.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            if (roundS == 1)
            {
                if(cbFirR.SelectedIndex>-1) ans = cbFirR.SelectedIndex;
            }
            else if (roundS == 2)
            {
                if (cbSecR.SelectedIndex > -1) ans = cbSecR.SelectedIndex ;
            }
            else if (roundS == 3)
            {
                if (cbSecR.SelectedIndex > -1) ans = cbThiR.SelectedIndex ;
            }
            else if (roundS == 4)
            {
                if (cbFouR.SelectedIndex > -1) ans = cbFouR.SelectedIndex ;
            }
            else if (roundS == 5)
            {
                if (cbFifR.SelectedIndex > -1) ans = cbFifR.SelectedIndex ;
            }

            try
            {
                byte[] data = _encoding.GetBytes($"{ans}");
                _udpClient.Send(data, data.Length, _remoteEP);
                roundS += 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void ResWait()
        {
            while (true)
            {
                if (roundA == 1)
                {
                    if (cbFirR.Text != "")
                    {

                        if (res == 0) tbOpFirR.Text = "Scissors";
                        if (res == 1) tbOpFirR.Text = "Stone";
                        if (res == 2) tbOpFirR.Text = "Paper";


                        if (tbOpFirR.Text != "")
                        {
                            if (cbFirR.SelectedIndex == 0 && tbOpFirR.Text == "Scissors")
                            {
                                tbResFirR.Text = "DRAW";
                                tbResFirR.BackColor = Color.Yellow;
                            }
                            else if (cbFirR.SelectedIndex == 1 && tbOpFirR.Text == "Stone")
                            {
                                tbResFirR.Text = "DRAW";
                                tbResFirR.BackColor = Color.Yellow;
                            }
                            else if (cbFirR.SelectedIndex == 2 && tbOpFirR.Text == "Paper")
                            {
                                tbResFirR.Text = "DRAW";
                                tbResFirR.BackColor = Color.Yellow;
                            }


                            if (cbFirR.SelectedIndex == 1 && tbOpFirR.Text == "Scissors")
                            {
                                tbResFirR.Text = "WIN";
                                tbResFirR.BackColor = Color.Green;
                            }
                            else if (cbFirR.SelectedIndex == 2 && tbOpFirR.Text == "Stone")
                            {
                                tbResFirR.Text = "WIN";
                                tbResFirR.BackColor = Color.Green;
                            }
                            else if (cbFirR.SelectedIndex == 0 && tbOpFirR.Text == "Paper")
                            {
                                tbResFirR.Text = "WIN";
                                tbResFirR.BackColor = Color.Green;
                            }



                            if (cbFirR.SelectedIndex == 2 && tbOpFirR.Text == "Scissors")
                            {
                                tbResFirR.Text = "LOOSE";
                                tbResFirR.BackColor = Color.Red;
                            }
                            else if (cbFirR.SelectedIndex == 0 && tbOpFirR.Text == "Stone")
                            {
                                tbResFirR.Text = "LOOSE";
                                tbResFirR.BackColor = Color.Red;
                            }
                            else if (cbFirR.SelectedIndex == 1 && tbOpFirR.Text == "Paper")
                            {
                                tbResFirR.Text = "LOOSE";
                                tbResFirR.BackColor = Color.Red;
                            }

                            //btnSend.PerformClick();
                            res = -1;
                            cbFirR.Enabled = false;
                            tbOpFirR.Enabled = false;
                            tbResFirR.Enabled = false;
                            

                            cbSecR.Enabled = true;
                            tbOpSecR.Enabled = true;
                            tbResSecR.Enabled = true;

                            roundA += 1;
                        }
                    }


                }
                else if (roundA == 2)
                {
                    if (cbSecR.Text != "")
                    {
                        if (res == 0) tbOpSecR.Text = "Scissors";
                        if (res == 1) tbOpSecR.Text = "Stone";
                        if (res == 2) tbOpSecR.Text = "Paper";

                        if (tbOpSecR.Text != "")
                        {
                            if (cbSecR.SelectedIndex == 0 && tbOpSecR.Text == "Scissors")
                            {
                                tbResSecR.Text = "DRAW";
                                tbResSecR.BackColor = Color.Yellow;
                            }
                            else if (cbSecR.SelectedIndex == 1 && tbOpSecR.Text == "Stone")
                            {
                                tbResSecR.Text = "DRAW";
                                tbResSecR.BackColor = Color.Yellow;
                            }
                            else if (cbSecR.SelectedIndex == 2 && tbOpSecR.Text == "Paper")
                            {
                                tbResSecR.Text = "DRAW";
                                tbResSecR.BackColor = Color.Yellow;
                            }


                            if (cbSecR.SelectedIndex == 1 && tbOpSecR.Text == "Scissors")
                            {
                                tbResSecR.Text = "WIN";
                                tbResSecR.BackColor = Color.Green;
                            }
                            else if (cbSecR.SelectedIndex == 2 && tbOpSecR.Text == "Stone")
                            {
                                tbResSecR.Text = "WIN";
                                tbResSecR.BackColor = Color.Green;
                            }
                            else if (cbSecR.SelectedIndex == 0 && tbOpSecR.Text == "Paper")
                            {
                                tbResSecR.Text = "WIN";
                                tbResSecR.BackColor = Color.Green;
                            }



                            if (cbSecR.SelectedIndex == 2 && tbOpSecR.Text == "Scissors")
                            {
                                tbResSecR.Text = "LOOSE";
                                tbResSecR.BackColor = Color.Red;
                            }
                            else if (cbSecR.SelectedIndex == 0 && tbOpSecR.Text == "Stone")
                            {
                                tbResSecR.Text = "LOOSE";
                                tbResSecR.BackColor = Color.Red;
                            }
                            else if (cbSecR.SelectedIndex == 1 && tbOpSecR.Text == "Paper")
                            {
                                tbResSecR.Text = "LOOSE";
                                tbResSecR.BackColor = Color.Red;
                            }
                            //btnSend.PerformClick();
                            res = -1;
                            cbSecR.Enabled = false;
                            tbOpSecR.Enabled = false;
                            tbResSecR.Enabled = false;
                            

                            cbThiR.Enabled = true;
                            tbOpThiR.Enabled = true;
                            tbResThiR.Enabled = true;

                            roundA += 1;
                        }
                    }

                }
                else if (roundA == 3)
                {
                    if (cbThiR.Text != "")
                    {
                        if (res == 0) tbOpThiR.Text = "Scissors";
                        if (res == 1) tbOpThiR.Text = "Stone";
                        if (res == 2) tbOpThiR.Text = "Paper";
                        if (tbOpThiR.Text != "")
                        {
                            if (cbThiR.SelectedIndex == 0 && tbOpThiR.Text == "Scissors")
                            {
                                tbResThiR.Text = "DRAW";
                                tbResThiR.BackColor = Color.Yellow;
                            }
                            else if (cbThiR.SelectedIndex == 1 && tbOpThiR.Text == "Stone")
                            {
                                tbResThiR.Text = "DRAW";
                                tbResThiR.BackColor = Color.Yellow;
                            }
                            else if (cbThiR.SelectedIndex == 2 && tbOpThiR.Text == "Paper")
                            {
                                tbResThiR.Text = "DRAW";
                                tbResThiR.BackColor = Color.Yellow;
                            }


                            if (cbThiR.SelectedIndex == 1 && tbOpThiR.Text == "Scissors")
                            {
                                tbResThiR.Text = "WIN";
                                tbResThiR.BackColor = Color.Green;
                            }
                            else if (cbThiR.SelectedIndex == 2 && tbOpThiR.Text == "Stone")
                            {
                                tbResThiR.Text = "WIN";
                                tbResThiR.BackColor = Color.Green;
                            }
                            else if (cbThiR.SelectedIndex == 0 && tbOpThiR.Text == "Paper")
                            {
                                tbResThiR.Text = "WIN";
                                tbResThiR.BackColor = Color.Green;
                            }



                            if (cbThiR.SelectedIndex == 2 && tbOpThiR.Text == "Scissors")
                            {
                                tbResThiR.Text = "LOOSE";
                                tbResThiR.BackColor = Color.Red;
                            }
                            else if (cbThiR.SelectedIndex == 0 && tbOpThiR.Text == "Stone")
                            {
                                tbResThiR.Text = "LOOSE";
                                tbResThiR.BackColor = Color.Red;
                            }
                            else if (cbThiR.SelectedIndex == 1 && tbOpThiR.Text == "Paper")
                            {
                                tbResThiR.Text = "LOOSE";
                                tbResThiR.BackColor = Color.Red;
                            }

                            res = -1;
                            cbThiR.Enabled = false;
                            tbOpThiR.Enabled = false;
                            tbResThiR.Enabled = false;

                            cbFouR.Enabled = true;
                            tbOpFouR.Enabled = true;
                            tbResFouR.Enabled = true;

                            roundA += 1;
                        }
                    }

                }
                else if (roundA == 4)
                {
                    if (cbFouR.Text != "")
                    {
                        if (res == 0) tbOpFouR.Text = "Scissors";
                        if (res == 1) tbOpFouR.Text = "Stone";
                        if (res == 2) tbOpFouR.Text = "Paper";

                        if (tbOpFouR.Text != "")
                        {
                            if (cbFouR.SelectedIndex == 0 && tbOpFouR.Text == "Scissors")
                            {
                                tbResFouR.Text = "DRAW";
                                tbResFouR.BackColor = Color.Yellow;
                            }
                            else if (cbFouR.SelectedIndex == 1 && tbOpFouR.Text == "Stone")
                            {
                                tbResFouR.Text = "DRAW";
                                tbResFouR.BackColor = Color.Yellow;
                            }
                            else if (cbFouR.SelectedIndex == 2 && tbOpFouR.Text == "Paper")
                            {
                                tbResFouR.Text = "DRAW";
                                tbResFouR.BackColor = Color.Yellow;
                            }


                            if (cbFouR.SelectedIndex == 1 && tbOpFouR.Text == "Scissors")
                            {
                                tbResFouR.Text = "WIN";
                                tbResFouR.BackColor = Color.Green;
                            }
                            else if (cbFouR.SelectedIndex == 2 && tbOpFouR.Text == "Stone")
                            {
                                tbResFouR.Text = "WIN";
                                tbResFouR.BackColor = Color.Green;
                            }
                            else if (cbFouR.SelectedIndex == 0 && tbOpFouR.Text == "Paper")
                            {
                                tbResFouR.Text = "WIN";
                                tbResFouR.BackColor = Color.Green;
                            }



                            if (cbFouR.SelectedIndex == 2 && tbOpFouR.Text == "Scissors")
                            {
                                tbResFouR.Text = "LOOSE";
                                tbResFouR.BackColor = Color.Red;
                            }
                            else if (cbFouR.SelectedIndex == 0 && tbOpFouR.Text == "Stone")
                            {
                                tbResFouR.Text = "LOOSE";
                                tbResFouR.BackColor = Color.Red;
                            }
                            else if (cbFouR.SelectedIndex == 1 && tbOpFouR.Text == "Paper")
                            {
                                tbResFouR.Text = "LOOSE";
                                tbResFouR.BackColor = Color.Red;
                            }

                            res = -1;
                            cbFouR.Enabled = false;
                            tbOpFouR.Enabled = false;
                            tbResFouR.Enabled = false;

                            cbFifR.Enabled = true;
                            tbOpFifR.Enabled = true;
                            tbResFifR.Enabled = true;

                            roundA += 1;
                        }
                    }

                }
                else if (roundA == 5)
                {
                    if (cbFifR.Text != "")
                    {
                        if (res == 0) tbOpFifR.Text = "Scissors";
                        if (res == 1) tbOpFifR.Text = "Stone";
                        if (res == 2) tbOpFifR.Text = "Paper";

                        if (tbOpFifR.Text != "")
                        {
                            if (cbFifR.SelectedIndex == 0 && tbOpFifR.Text == "Scissors")
                            {
                                tbResFifR.Text = "DRAW";
                                tbResFifR.BackColor = Color.Yellow;
                            }
                            else if (cbFifR.SelectedIndex == 1 && tbOpFifR.Text == "Stone")
                            {
                                tbResFifR.Text = "DRAW";
                                tbResFifR.BackColor = Color.Yellow;
                            }
                            else if (cbFifR.SelectedIndex == 2 && tbOpFifR.Text == "Paper")
                            {
                                tbResFifR.Text = "DRAW";
                                tbResFifR.BackColor = Color.Yellow;
                            }


                            if (cbFifR.SelectedIndex == 1 && tbOpFifR.Text == "Scissors")
                            {
                                tbResFifR.Text = "WIN";
                                tbResFifR.BackColor = Color.Green;
                            }
                            else if (cbFifR.SelectedIndex == 2 && tbOpFifR.Text == "Stone")
                            {
                                tbResFifR.Text = "WIN";
                                tbResFifR.BackColor = Color.Green;
                            }
                            else if (cbFifR.SelectedIndex == 0 && tbOpFifR.Text == "Paper")
                            {
                                tbResFifR.Text = "WIN";
                                tbResFifR.BackColor = Color.Green;
                            }



                            if (cbFifR.SelectedIndex == 2 && tbOpFifR.Text == "Scissors")
                            {
                                tbResFifR.Text = "LOOSE";
                                tbResFifR.BackColor = Color.Red;
                            }
                            else if (cbFifR.SelectedIndex == 0 && tbOpFifR.Text == "Stone")
                            {
                                tbResFifR.Text = "LOOSE";
                                tbResFifR.BackColor = Color.Red;
                            }
                            else if (cbFifR.SelectedIndex == 1 && tbOpFifR.Text == "Paper")
                            {
                                tbResFifR.Text = "LOOSE";
                                tbResFifR.BackColor = Color.Red;
                            }

                            res = -1;
                            cbFifR.Enabled = false;
                            tbOpFifR.Enabled = false;
                            tbResFifR.Enabled = false;


                            roundA += 1;

                            MessageBox.Show("The game is over!!!");
                            StopListener();
                            Close();
                        }
                    }
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(roundA.ToString()+"         "+roundS.ToString()+"      "+res.ToString());
        }

        private void StopListener()
        {
            byte[] data = _encoding.GetBytes($"{_name} left the chat");

            try
            {
                _udpClient.Send(data, data.Length, _remoteEP);
            }
            catch { }
            finally
            {
                _udpClient.DropMulticastGroup(_groupAddress);
                _udpClient.Close();

                _done = true;

               
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_done)
            {
                StopListener();
            }
        }
    }
}
