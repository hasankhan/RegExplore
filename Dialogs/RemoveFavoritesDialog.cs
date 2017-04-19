using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrackSoft.RegExplore.Registry;
using CrackSoft.Collections;

namespace CrackSoft.RegExplore
{
    partial class RemoveFavoritesDialog : Form
    {
        EventDictionary<string, string> favorites;

        public RemoveFavoritesDialog(EventDictionary<string, string> favorites)
        {
            InitializeComponent();
            this.favorites = favorites;
        }

        private void RemoveFavoritesDialog_Load(object sender, EventArgs e)
        {
            foreach (var item in favorites.Keys)
                lstKeys.Items.Add(item);
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            RegKey regKey = RegKey.Parse(RegExplorer.RegistryFavoritePath, true);
            foreach (var item in lstKeys.SelectedItems)
            {
                string key = item.ToString();
                regKey.Key.DeleteValue(key);
                favorites.Remove(key);
            }
        }
    }
}
