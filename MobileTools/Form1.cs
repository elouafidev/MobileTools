using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ADBCommandSDK;
using System.IO;
using System.Collections;
namespace MobileTools
{
    public partial class Form1 : Form
    {
        ADBCommand adbCommand = new ADBCommand();
        
        List<int> ListSearch = new List<int>();
        enum BandingMoveSearch{ MoveNext, MovePrevious, MoveLast , MoveFirst ,}



        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        private void infoNewLine(params object[] Args)
        {
            bool colorChanged=false;

            for (int i = 0; i < Args.Length; i++)
                if (Args[i] is Color)
                    colorChanged = true;
                else if (Args[i] is string)
                {
                    if (colorChanged)
                        infoRichTextBox.SelectionColor = ((Color)Args[i - 1]);
                    else
                        infoRichTextBox.SelectionColor = SystemColors.ControlText;

                    infoRichTextBox.AppendText(Args[i].ToString());
                    infoRichTextBox.SelectionStart = infoRichTextBox.Text.Length;
                    infoRichTextBox.ScrollToCaret();
                    infoRichTextBox.Refresh();
                    colorChanged = false;
                }

        }

        private void Writingdata(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    consoleRichTextBox.Text += "\n" + data;
                    consoleRichTextBox.SelectionStart = consoleRichTextBox.Text.Length;
                    consoleRichTextBox.ScrollToCaret();
                    consoleRichTextBox.Refresh();
                }));
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            RebootGroupBox.DataBindings.Add("Visible",RebootRadioButton,"Checked");
            InstallApkGroupBox.DataBindings.Add("Visible", InstallApkRadioButton, "Checked");
            // stateProcessRadioButton.DataBindings.Add("Checked",dv,"StateProcess");
            ConsoleCommand.WritingData += Writingdata;

        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            List<Device> ListDevices = adbCommand.ScanDevices();
            ScanComboBox.Items.Clear();
            if (ListDevices.Count != 0)
            {
                for (int i = 0; i < ListDevices.Count; i++) ScanComboBox.Items.Add(ListDevices[i]);
                ScanComboBox.SelectedIndex = ScanComboBox.Items.Count - 1;
            }
            else ScanComboBox.Text = "Connect Dervices.";

        }

        private void ScanComboBox_TextChanged(object sender, EventArgs e)
        {
            adbCommand.DeviceSelected = ((Device)ScanComboBox.SelectedItem);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Process ="";
           foreach(Control c in allTabControl.SelectedTab.Controls)
                if (c is TabControl)
                {
                    foreach (Control cc in ((TabControl)c).SelectedTab.Controls)
                        if (cc is RadioButton)
                            if (((RadioButton)cc).Checked)
                            {
                                Process = ((RadioButton)cc).Text;
                                break;
                            }
                    
                }
            infoNewLine("\n################### {" + Process + "} ###################\n");
           ServiceStart(Process);

        }



        private void ServiceStart(string nameservice)
        {
            if (adbCommand.DeviceSelected != null)
            {
                switch(nameservice)
                {
                    case "Read Info": ReadInfo();
                        break;

                    case "Check Root": ServiceRoot();
                        break;

                    case "Read Imei":
                        adbCommand.RunArgument("shell getprop ro.modem.w.count");
                        new Thread(adbCommand.BeginReadLine) { IsBackground = true }.Start();
                        break;

                    case "Hardware Info":
                        ServiceHardwareInfo();
                        break;

                    case "Software Info":
                        infoNewLine(Color.Blue, adbCommand.GetFingerPrint());
                        break;
                    case "Reset PinLock":
                        ResetPinLock();
                        break;
                    case "Reboot":
                        infoNewLine("\nReboot :", Color.Red, adbCommand.Reboot(ReBootComboBox.Text).ToString());
                        break;
                    case "Install APK":
                        InstallAPk();
                        break;
                    case "Reset GestureLock":
                        Gesturelock();
                        break;
                    case "Wipe/Factory Reset":
                        wipedata();
                        break;
                    case "Reset Gmail":
                        wipedata();
                        break;
                    case "FB Wipe/Factory Reset":

                        break;
                }
            }
            else MessageBox.Show("Select your device..");

        }

        private void ReadInfo()
        {

            infoNewLine(
                "\nBuild Number :",
                Color.Blue, adbCommand.GetBuildNumber(),
                "\nDevice product :",
                Color.Blue, adbCommand.GetDeviceName(),
                "\nBrand :",
                Color.Blue, adbCommand.GetBrand(),
                "\nBoard Name :",
                Color.Blue, adbCommand.GetBoard(),
                "\nAndroid Version :",
                Color.Blue, adbCommand.GetAndroidVersion(),
                "\nCPU :",
                Color.Blue, adbCommand.GetCPU()
                );
            adbCommand.RunArgument("shell getprop");
            adbCommand.BeginReadLine(false);
        }
        private void ResetPinLock()
        {
            infoNewLine(Color.Red, "Reset Pin Lock : " + adbCommand.ShellSuOrNot("rm /data/system/password.key"));
        }

        private void ServiceRoot()
        {
            infoNewLine(
                System.Environment.NewLine + adbCommand.DeviceSelected.Module,
                Color.Red, adbCommand.DeviceSelected.Su.ToString());

        }
        private void ServiceHardwareInfo()
        {
            //
            adbCommand.RunArgument( "shell lsmod");
            new Thread(adbCommand.BeginReadLine) { IsBackground = true }.Start();

        }

        private void InstallAPk()
        {
                if (apkOpenFileDialog.FileNames.LongLength > 1)
                {

                    infoNewLine("\n################### {Install Files Apk} ###################\n");
                    for (int i = 0; i < apkOpenFileDialog.FileNames.LongLength; i++)
                    {
                        infoNewLine(Color.Blue, "\n[" + (i + 1) + "] :" + (new FileInfo(apkOpenFileDialog.FileNames[i])).Name, "\nInstall SD Card Wait...");
                        string tempStr = adbCommand.InstallAPK(apkOpenFileDialog.FileNames[i], StateInstallApk.R, StateInstallApk.S);
                        if (tempStr.Contains("Success"))
                        {
                            infoNewLine(Color.Blue, "\n" + tempStr);
                        }
                        else
                        {
                            infoNewLine("\n Install internal Memory Wait ...");
                            infoNewLine(Color.Blue, adbCommand.InstallAPK(apkOpenFileDialog.FileNames[i], StateInstallApk.R));
                        }

                    }
                }
                else
                {
                    infoNewLine("\n################### {Install File Apk} ###################\n");
                    infoNewLine(Color.Blue, "\n " + (new FileInfo(apkOpenFileDialog.FileName)).Name, "\nInstall SD Card Wait...");
                    string tempStr = adbCommand.InstallAPK(apkOpenFileDialog.FileName, StateInstallApk.R, StateInstallApk.S);
                    if (tempStr.Contains("Success"))
                    {
                        infoNewLine(Color.Blue, "\n" + tempStr);
                    }
                    else
                    {
                        infoNewLine("\n Install internal Memory Wait ...");
                        infoNewLine(Color.Blue, adbCommand.InstallAPK(apkOpenFileDialog.FileName, StateInstallApk.R));
                    }

                }
        }
        private void Gesturelock()
        {
            infoNewLine(Color.Red, "Gesture Lock: " + adbCommand.ShellSuOrNot("rm /data/system/gesture.key"));
        }
        
        private void wipedata()
        {
            infoNewLine(Color.Red, "Wipe/Factory Reset: " + adbCommand.ShellSuOrNot("wipe data"));
        }

        private void ResetGmail()
        {
            infoNewLine(Color.Red, "Reset Gmail : ");
            infoNewLine(Color.Red, "Method 1: " + adbCommand.ShellSuOrNot("rm /data/data/com.android.providers.settings/databases/settings.db"));
            infoNewLine(Color.Red, "Method 2: " + adbCommand.ShellSuOrNot("rm /data/data/com.android.providers.settings/databases/settings.db-shm"));
            infoNewLine(Color.Red, "Method 3: " + adbCommand.ShellSuOrNot("rm /data/data/com.android.providers.settings/databases/settings.db-wal"));
        }

        private void FBWipedata()
        {
        }
        private void consoleRichTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 6)
            {
                SearchPanel.Visible = true;
                searchTextBox.Focus();
            }
            else if (e.KeyChar == 20)
                if (adbCommand.DeviceSelected != null)
                {
                    adbCommand.RunArgument("shell");
                    new Thread(adbCommand.BeginReadLine) { IsBackground = true }.Start();
                    Writingdata("Treminal is runnig ... \n#:");
                    panelTerminal.Visible = true;
                    comboBox2.Focus();
                }else MessageBox.Show("Select your device..");


        }


        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ListSearch.Clear();
                int tempPointStartIndex = 0;
                countSearchlabel.Text = 0.ToString();
                PinSearchlabel.Text = "0";
                consoleRichTextBox.SelectAll();
                consoleRichTextBox.SelectionBackColor = System.Drawing.SystemColors.WindowText;

                for (int i = 0; i < consoleRichTextBox.TextLength; i++)
                {
                    if ((tempPointStartIndex = consoleRichTextBox.Text.IndexOf(searchTextBox.Text, tempPointStartIndex)) != -1)
                    {
                        ListSearch.Add(tempPointStartIndex);

                        // consoleRichTextBox.SelectionStart = 0;
                        countSearchlabel.Text = (int.Parse(countSearchlabel.Text) + 1).ToString();
                        consoleRichTextBox.SelectionStart = tempPointStartIndex;
                        consoleRichTextBox.SelectionLength = searchTextBox.Text.Length;
                        consoleRichTextBox.SelectionBackColor = Color.Yellow;
                        tempPointStartIndex += searchTextBox.Text.Length;
                    }
                    else break;
                    countSearchlabel.Text = ListSearch.Count.ToString();
                }
                e.Handled = true;
                BandingSearch(BandingMoveSearch.MoveNext);
            }
            

        }
         private void BandingSearch(BandingMoveSearch Move)
        {
            if (ListSearch.Count > 0)
            {
                switch (Move)
                {
                    case BandingMoveSearch.MoveNext:
                        if (int.Parse(PinSearchlabel.Text) != (ListSearch.Count - 1)) PinSearchlabel.Text = (int.Parse(PinSearchlabel.Text) + 1).ToString();
                        break;
                    case BandingMoveSearch.MovePrevious:
                        if (PinSearchlabel.Text != "1") PinSearchlabel.Text = (int.Parse(PinSearchlabel.Text) - 1).ToString();
                        break;
                    case BandingMoveSearch.MoveLast:
                        PinSearchlabel.Text = (ListSearch.Count - 1).ToString();
                        break;
                    case BandingMoveSearch.MoveFirst:
                        PinSearchlabel.Text = (1).ToString();
                        break;
                }
                consoleRichTextBox.SelectionStart = ListSearch[int.Parse(PinSearchlabel.Text)];
                consoleRichTextBox.ScrollToCaret();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            ListSearch.Clear();
            PinSearchlabel.Text = 0.ToString();
            countSearchlabel.Text = 0.ToString();
            consoleRichTextBox.SelectAll();
            consoleRichTextBox.SelectionBackColor = System.Drawing.SystemColors.WindowText;
            SearchPanel.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BandingSearch(BandingMoveSearch.MoveLast);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            BandingSearch(BandingMoveSearch.MoveNext);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BandingSearch(BandingMoveSearch.MovePrevious);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BandingSearch(BandingMoveSearch.MoveFirst);
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                adbCommand.WriteCommandLine = comboBox2.Text;
                e.Handled = true;
                //-------------------------
                if (!comboBox2.Items.Contains(comboBox2.Text)) comboBox2.Items.Add(comboBox2.Text);
                comboBox2.Text = "";

            }
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SExitTerminal:
            if (adbCommand.StateProcess)
            {
                adbCommand.WriteCommandLine = "exit";
                goto SExitTerminal;
            }
            panelTerminal.Visible = false;
            comboBox2.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(apkOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (apkOpenFileDialog.FileNames.LongLength > 1)
                {
                    LineApkTextBox.Text = (new FileInfo(apkOpenFileDialog.FileName)).DirectoryName;
                    infoNewLine("\n################### {Files Apk} ###################\n");
                    for (int i = 0; i < apkOpenFileDialog.FileNames.LongLength; i++)
                    {
                        infoNewLine(Color.Blue, "\n["+(i+1)+"] :" + (new FileInfo(apkOpenFileDialog.FileNames[i])).Name);
                    }

                }
                else
                {
                    infoNewLine("\n################### {File Apk} ###################\n");
                    infoNewLine(Color.Blue, "\n " + (new FileInfo(apkOpenFileDialog.FileName)).Name);
                    LineApkTextBox.Text = apkOpenFileDialog.FileName;
                }
            }
        }


    }
    
}
