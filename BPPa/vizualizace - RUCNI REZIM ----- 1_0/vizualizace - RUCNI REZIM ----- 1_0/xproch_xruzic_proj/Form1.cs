using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dldrv_ax;



namespace xproch_xruzic_proj
{

    public partial class Form1 : Form
    {
        dldrv_axClass IO = new dldrv_ax.dldrv_axClass { };
        
        enum sensors { K1 = 101, K2, K3, K4, K5};
        enum motors {Stepper_En = 205, Stepper_Dir = 204, Stepper_Speed = 301, DC_Motor_Dir = 207, DC_Motor_Speed = 300};
        enum others { Model_On = 107, Magnet = 200, M_Up = 100, M_Imp = 106, D1=400 };
        enum moving { stop =0 ,left, right, up, down };

        int citac=0;
        int posunLR=2000;
        double pomer=0.7;
        int stav = -1;
        bool sep1 = false;
        bool sep2 = false;
        int Init = 1;
        int x;

        DateTime firsttime;
        DateTime lasttime;
        TimeSpan cas;
        int pozice;
         
                
        bool max_up = false;              // +++++
        bool max_left = false;            // +++++
               
        
        
        public Form1()
        {
            InitializeComponent();
        }
      

        void IO_OnInputRead(TCommunicationState CommunicationState, int InputIndex, int ErrorCode, string Value)
        {
            if (Value == "true")
            {
                switch (InputIndex)
                {
                    case (int)others.M_Up:
                        doraz_mag.BackColor = System.Drawing.Color.Green;
                        max_up=true; // +++++
                        break;
                    case (int)others.M_Imp:
                        label7.BackColor = System.Drawing.Color.Green;
                        sep1 = true;
                        break;
                    case (int)sensors.K1:
                        K1L.BackColor = System.Drawing.Color.Green;
                        max_left=true; // +++++
                        break;
                    case (int)sensors.K2:
                        K2L.BackColor = System.Drawing.Color.Green;
                        break;
                    case (int)sensors.K3:
                        K3L.BackColor = System.Drawing.Color.Green;
                        break;
                    case (int)sensors.K4:
                        label2.Text = Convert.ToString(obr_magnet.Left);
                        K4L.BackColor = System.Drawing.Color.Green;
                        break;
                    case (int)sensors.K5:
                        K5L.BackColor = System.Drawing.Color.Green;
                        break;
                    
                    default:

                        break;
                }

            }
            else
            {
                switch (InputIndex)
                {
                    case (int)others.M_Up:
                        doraz_mag.BackColor = System.Drawing.Color.Empty;
                        max_up = false;
                        break;
                    case (int)others.M_Imp:
                        label7.BackColor = System.Drawing.Color.Empty;
                        sep1 = false;
                        break;
                    case (int)sensors.K1:
                        K1L.BackColor = System.Drawing.Color.Empty;
                        max_left = false;
                        break;
                    case (int)sensors.K2:
                        K2L.BackColor = System.Drawing.Color.Empty;
                        break;
                    case (int)sensors.K3:
                        K3L.BackColor = System.Drawing.Color.Empty;
                        break;
                    case (int)sensors.K4:
                        K4L.BackColor = System.Drawing.Color.Empty;
                        break;
                    case (int)sensors.K5:
                        K5L.BackColor = System.Drawing.Color.Empty;
                        break;
                    default:

                        break;
                }
            }
            
            if (InputIndex == 400)
            { 
            //label2.Text = Convert.ToString(pozice);


            if (stav == (int)moving.right)
            {
                if ((pozice < Convert.ToInt32(Value))&((Convert.ToInt32(Value)))<(pozice+600))
                {
                    pozice = Convert.ToInt32(Value);
                }
                else
                {
                    pozice += 40;
                }
            }
            else if (stav == (int)moving.left)
            {
                if ((pozice > Convert.ToInt32(Value)) & ((Convert.ToInt32(Value))) > (pozice - 600))
                {
                    pozice = Convert.ToInt32(Value);
                }
                else
                {
                    pozice -= 40;
                }
            }


            }

        }


        private void init_magnet()
        {
            IO.MarkOutput((int)motors.Stepper_Speed, "1000");
            IO.MarkOutput((int)motors.Stepper_Dir, "false");
            IO.MarkOutput((int)motors.Stepper_En, "true");
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            string a = "stringa";
            IO.LoadPARFile("dldrv.par", out a);
            IO.OnInputRead += new IDrvAx_Event_OnInputReadEventHandler(IO_OnInputRead);
            IO.MarkOutput(200, "false");
            IO.WriteOutputs();
            
            init_magnet();

        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            IO.WriteOutputs();
            IO.MarkInput((int)others.M_Up);
            IO.MarkInput((int)others.M_Imp);
            IO.MarkInput((int)sensors.K1);
            IO.MarkInput((int)sensors.K2);
            IO.MarkInput((int)sensors.K3);
            IO.MarkInput((int)sensors.K4);
            IO.MarkInput((int)sensors.K5);
            // ++++++++++
            
            IO.MarkInput((int)others.D1);
            
            // ++++++++++
            IO.ReadInputs();

            if (Init>0)
            {
                if (max_up)
                {
                    Init = 2;

                    IO.MarkOutput((int)motors.Stepper_Speed, "1000");
                    IO.MarkOutput((int)motors.Stepper_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_En, "true");
                    IO.WriteOutputs();
                }

                if (Init == 2)
                {
                    x++;
                }

                if (x > 300)
                {
                    IO.MarkOutput((int)motors.Stepper_Speed, "1000");
                    IO.MarkOutput((int)motors.Stepper_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_En, "false");
                    IO.WriteOutputs();
                    Init = 3;
                }

                if ((Init == 3) & (!max_left))
                {
                    IO.MarkOutput((int)motors.DC_Motor_Speed, Convert.ToString(2000));
                    IO.MarkOutput((int)motors.DC_Motor_Dir, "false");
                    IO.MarkOutput((int)motors.Stepper_Speed, Convert.ToString(0.7 * 2000));
                    IO.MarkOutput((int)motors.Stepper_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_En, "true");
                    IO.WriteOutputs();
                }

                if ((max_left) & (Init == 3))
                {
                    stav = 0;
                    Init = 0;
                    up.Enabled = true;
                    stop.Enabled = true;
                    down.Enabled = true;
                    left.Enabled = true;
                    right.Enabled = true;
                    mag.Enabled = true;
                    rychlost.Enabled = true;
                    obr_magnet.Visible = true;
                    pozice = 5200;
                }
            }

            switch(stav)
            {
                case (int)moving.stop:
                    IO.MarkOutput((int)motors.DC_Motor_Speed, "0");
                    IO.MarkOutput((int)motors.DC_Motor_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_Speed, "0");
                    IO.MarkOutput((int)motors.Stepper_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_En, "false");
                    LDC_left.BackColor = System.Drawing.Color.Empty;
                    LDC_right.BackColor = System.Drawing.Color.Empty;
                    Lstepper_up.BackColor = System.Drawing.Color.Empty;
                    Lstepper_down.BackColor = System.Drawing.Color.Empty;
                    firsttime = System.DateTime.Now;
                    //pozice = obr_magnet.Left;
                    break;
                case (int)moving.left:
                    if (K1L.BackColor != System.Drawing.Color.Green)
                    {
                    IO.MarkOutput((int)motors.DC_Motor_Speed, Convert.ToString(posunLR));
                    IO.MarkOutput((int)motors.DC_Motor_Dir, "false");
                    IO.MarkOutput((int)motors.Stepper_Speed, Convert.ToString(pomer * posunLR));
                    IO.MarkOutput((int)motors.Stepper_Dir, "true");
                    IO.MarkOutput((int)motors.Stepper_En, "true");
                    LDC_left.BackColor = System.Drawing.Color.Green;
                    LDC_right.BackColor = System.Drawing.Color.Empty;
                    Lstepper_up.BackColor = System.Drawing.Color.Empty;
                    Lstepper_down.BackColor = System.Drawing.Color.Green;

                    obr_magnet.Left = (pozice - 5200) / 19;
                    }
                    else
                    {
                        obr_magnet.Left = 0;
                        stav = (int)moving.stop;
                    }
                    break;
                case (int)moving.right:
                    if (K5L.BackColor != System.Drawing.Color.Green)
                    {
                        IO.MarkOutput((int)motors.DC_Motor_Speed, Convert.ToString(posunLR));
                        IO.MarkOutput((int)motors.DC_Motor_Dir, "true");
                        IO.MarkOutput((int)motors.Stepper_Speed, Convert.ToString(pomer * posunLR));
                        IO.MarkOutput((int)motors.Stepper_Dir, "false");
                        IO.MarkOutput((int)motors.Stepper_En, "true");
                        LDC_left.BackColor = System.Drawing.Color.Empty;
                        LDC_right.BackColor = System.Drawing.Color.Green;
                        Lstepper_up.BackColor = System.Drawing.Color.Green;
                        Lstepper_down.BackColor = System.Drawing.Color.Empty;
                        
                        obr_magnet.Left = (pozice-5200)/19;   
                    }
                    else
                    {
                        obr_magnet.Left = 770;
                        stav = (int)moving.stop;
                    }
                    break;
                case (int)moving.up:
                    if (doraz_mag.BackColor != System.Drawing.Color.Green)
                    {
                    IO.MarkOutput((int)motors.Stepper_Speed, "1000");
                    IO.MarkOutput((int)motors.Stepper_Dir, "false");
                    IO.MarkOutput((int)motors.Stepper_En, "true");
                    LDC_left.BackColor = System.Drawing.Color.Empty;
                    LDC_right.BackColor = System.Drawing.Color.Empty;
                    Lstepper_up.BackColor = System.Drawing.Color.Green;
                    Lstepper_down.BackColor = System.Drawing.Color.Empty;
                    }
                    else
                    {
                    stav = (int)moving.stop;
                    }
                        break;
                case (int)moving.down:
                        if (citac < 18)
                        {
                            IO.MarkOutput((int)motors.Stepper_Speed, "2000");
                            IO.MarkOutput((int)motors.Stepper_Dir, "true");
                            IO.MarkOutput((int)motors.Stepper_En, "true");
                            LDC_left.BackColor = System.Drawing.Color.Empty;
                            LDC_right.BackColor = System.Drawing.Color.Empty;
                            Lstepper_up.BackColor = System.Drawing.Color.Empty;
                            Lstepper_down.BackColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            stav = (int)moving.stop;
                            timer2.Enabled = false;
                        }
                        break;

                default: break;
            }

        }
      

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IO.MarkOutput((int)others.Magnet, "false");
            IO.WriteOutputs();
        }

 

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (sep1 == sep2)
            {
                citac++;
            }
            else 
            {
                citac = 0;
                sep2 = sep1;
            }
        }

        private void left_Click(object sender, EventArgs e)
        {
            stav = (int)moving.left;  
        }

        private void right_Click(object sender, EventArgs e)
        {
            stav = (int)moving.right;
        }

        private void up_Click(object sender, EventArgs e)
        {
            stav = (int)moving.up;
            citac = 0;
        }

        private void down_Click(object sender, EventArgs e)
        {
            stav = (int)moving.down;
            timer2.Enabled = true;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            stav = (int)moving.stop;
        }

        private void mag_Click(object sender, EventArgs e)
        {
            if (mag.Tag == "0")
            {
                IO.MarkOutput(200, "true");
                mag.Tag = "1";
            }
            else
            {
                IO.MarkOutput(200, "false");
                mag.Tag = "0";
            }
            
        }

        private void rychlost_SelectedIndexChanged_2(object sender, EventArgs e)
        {
            
           
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            obr_L.Left = 217;
            obr_M.Left = 202;
            obr_B.Left = 187;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            obr_L.Left = 427;
            obr_M.Left = 412;
            obr_B.Left = 397;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            obr_L.Left = 602;
            obr_M.Left = 587;
            obr_B.Left = 572;
        }

        private void rychlost_SelectedValueChanged(object sender, EventArgs e)
        {
            if (rychlost.Text == "slow")
            {
                posunLR = 1000;
                pomer = 0.47;
            }

            if (rychlost.Text == "medium")
            {
                posunLR = 2000;
                pomer = 0.7;
                
            }

            if (rychlost.Text == "high")
            {
                posunLR = 3000;
                pomer = 0.81;
               
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel3.Visible = true;
            groupBox1.Enabled = false;
            rychlost.Enabled = false;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = false;
            groupBox1.Enabled = true;
            rychlost.Enabled = true;
        }

               
        
    }
}
