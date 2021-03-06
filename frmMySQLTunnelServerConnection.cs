﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBManager.DBASES;
using System.Net;

namespace DBManager
{
    public partial class frmMySQLTunnelServerConnection : BlueForm
    {
        private bool importSchema;
        private List<string> tbls;
        MySQLTunnel c = null;

        public frmMySQLTunnelServerConnection()
        {
            InitializeComponent();
        }

        public frmMySQLTunnelServerConnection(bool importSchema, List<string> tbls)
        {
            InitializeComponent();

            this.importSchema = importSchema;
            this.tbls = tbls;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtURL.Text.Trim().Length == 0 || txtPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please fill all infos!", General.apTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (txtPassword.Text.Contains("$"))
                {
                    MessageBox.Show("The symbol '$' forbidden!", General.apTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!txtURL.Text.ToLower().StartsWith("https"))
                {
                   if ( MessageBox.Show("Server must be https enabled, are you sure you want to continue ?", General.apTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) ==System.Windows.Forms.DialogResult.No)
                    return;
                }
            }


            if (txtURL.Text.ToLower().StartsWith("https"))
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            else
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(48 | 192);


            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            //MySqlConnection objConn = null;
            //bool errorOccured = false;
            c = new MySQLTunnel();
            c.isConnected += new MySQLTunnel.IsConnected(c_isConnected);
            try
            {
                Cursor = System.Windows.Forms.Cursors.WaitCursor;

                c.testConnection(txtURL.Text.Trim(), txtPassword.Text.Trim());

            }
            catch (Exception ex)
            {
                General.Mes(ex.Message);
              
            }
            finally
            {
                Cursor = System.Windows.Forms.Cursors.Default;
            }


        }


        delegate void c_isConnectedCallback(bool isSuccess);
        void c_isConnected(bool isSuccess)
        {
            if (txtURL.InvokeRequired)
            {
                txtURL.BeginInvoke(new c_isConnectedCallback(this.c_isConnected), new object[] { isSuccess });
                return;
            }

            Cursor = System.Windows.Forms.Cursors.Default;

            if (isSuccess)
            {
                General.Connections.Add(new dbConnection
                {
                    dbaseName = "",
                    filename = "",
                    password = txtPassword.Text,
                    port = "",
                    serverName = txtURL.Text,
                    TYPE = (int)General.dbTypes.MySQLtunnel,
                    user = ""
                });


                this.DialogResult = System.Windows.Forms.DialogResult.OK;

            }
            else
                General.Mes("Could not connect!");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("mysqli script is broken, due new method of exchange (server -> client), use v2.3.9 instead.");
            frmInformation i = new frmInformation(DBManager.Properties.Resources.tunnelPHP);
            i.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInformation i = new frmInformation(DBManager.Properties.Resources.tunnelPHP_PDO);
            i.ShowDialog();
        }
    }
}
