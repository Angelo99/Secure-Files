using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
 
 namespace AESFileEncryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Crypto.Crypto CryptoFunctions = new Crypto.Crypto();
        private void btnen_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                int ii = 0;
                
                try
                {
                    if (listView1.Items[i].SubItems[1].Text == ".enc") //check if file type is alredy encrypted,if true popup error and skip that file
                    {
                        
                        listView1.Items[i].SubItems[3].Text = "Error";
                        listView1.Items[i].BackColor = Color.OrangeRed;
                    }
                    else
                    {

                        if(txtPasswrd.Text!="" && txtPasswordConfirm.Text != "")//check if password is not empty
                        {
                            if (txtPasswrd.Text == txtPasswordConfirm.Text)//check both passwords are equels
                            {

                                File.Copy(listView1.Items[i].SubItems[0].Text, listView1.Items[i].SubItems[0].Text + ".enc"); //get copy of original file and save it to save location with .enc extention

                                byte[] bytesToBeEncrypted = File.ReadAllBytes(listView1.Items[i].SubItems[0].Text + ".enc");//read all bytes of file and save it to byte array
                                byte[] passwordBytes = Encoding.UTF8.GetBytes(txtPasswrd.Text);//get password from txtbox

                                // Hash the password with SHA256
                                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                                byte[] bytesEncrypted = CryptoFunctions.AES_Encrypt(bytesToBeEncrypted, passwordBytes); //encrypt bytes with password

                                string fileEncrypted = listView1.Items[i].SubItems[0].Text + ".enc";

                                File.WriteAllBytes(fileEncrypted, bytesEncrypted); //write encrypted byte array to disk
                                listView1.Items[i].SubItems[3].Text = "Encrypted";
                                listView1.Items[i].BackColor = Color.LimeGreen;
                            }
                            else
                                MessageBox.Show("Password does not match the confirm password");
                        }
                        else
                            MessageBox.Show("Enter Password");


                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
              
            }


            txtPasswordConfirm.Clear(); txtPasswrd.Clear();
        }

        private void btnde_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                int ii = 0;
                if (listView1.Items[i].SubItems[1].Text == ".enc") //check the file is ends with .enc extetion decrypt.if not skip that file
                {
                    try
                    {
                        if (txtPasswrd.Text != "" && txtPasswordConfirm.Text != "") //check if password is not empty
                        {
                            if (txtPasswrd.Text == txtPasswordConfirm.Text)//check both passwords are equels
                            {
                                byte[] bytesToBeDecrypted = File.ReadAllBytes(listView1.Items[i].SubItems[0].Text);//read all bytes of file and save it to byte array
                                byte[] passwordBytes = Encoding.UTF8.GetBytes(txtPasswrd.Text);
                                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                                byte[] bytesDecrypted = CryptoFunctions.AES_Decrypt(bytesToBeDecrypted, passwordBytes);//decrypt encrypted bytearray

                               
                                string file = listView1.Items[i].SubItems[0].Text.Remove(listView1.Items[i].SubItems[0].Text.Length - 3);

                                File.WriteAllBytes(file, bytesDecrypted);//save decrypted file into disk

                                listView1.Items[i].SubItems[3].Text = "Decrypted";
                                listView1.Items[i].BackColor = Color.GreenYellow;
                            }
                            else
                                MessageBox.Show("Password does not match the confirm password");
                        }
                        else
                        {
                            MessageBox.Show("Enter Password to Decrypt");
                        }


                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                }

                ii++;
            }
            txtPasswordConfirm.Clear();txtPasswrd.Clear();
      
        }

        private void btnselect_Click(object sender, EventArgs e)
        {
          
            DialogResult result = openFileDia.ShowDialog();
            if (result == DialogResult.OK)
            {
                FileInfo FI = new FileInfo(openFileDia.FileName);
                 float size = (FI.Length / 1024f) / 1024f; //get size of file
                ListViewItem item1 = new ListViewItem(openFileDia.FileName);
                txtpath.Text = openFileDia.FileName;
                item1.SubItems.Add(FI.Extension); //get extetion of a file
                item1.SubItems.Add( size.ToString());
                item1.SubItems.Add("idle");
                listView1.Items.Add(item1);

                btnen.Enabled = true;
                btnde.Enabled = true;
            }
                

         
          
        }

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Convert Password textbox to Normal txtbox
            txtPasswrd.PasswordChar = '\0';
            txtPasswordConfirm.PasswordChar = '\0';

            txtPasswrd.Text = GetRandomString();
            txtPasswordConfirm.Text = txtPasswrd.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string foldername = folderBrowserDialog1.SelectedPath;
                foreach (string f in Directory.GetFiles(foldername))
                {
                    FileInfo FI = new FileInfo(f);
                    
                    
                    float size = (FI.Length / 1024f) / 1024f;
                    ListViewItem item1 = new ListViewItem(f);
                   
                    item1.SubItems.Add(FI.Extension);
                    item1.SubItems.Add(size.ToString());
                    item1.SubItems.Add("idle");
                    listView1.Items.Add(item1);
                    btnen.Enabled = true;
                    btnde.Enabled = true;
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //remove selected item from context menu
            DialogResult Ds;
            Ds=MessageBox.Show("Do you want to remove "+ listView1.SelectedItems[0].Text +" ?","remove alert",MessageBoxButtons.YesNo);
            if(Ds==DialogResult.Yes)
                listView1.SelectedItems[0].Remove();
            //MessageBox.Show(listView1.SelectedItems[0].Text); 
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://h3llwings.wordpress.com/");
            MessageBox.Show("Angelo Ruwantha");
        }
    }
}
