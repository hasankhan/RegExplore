using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using CrackSoft.RegExplore.Registry;
using System.IO;
using System.Diagnostics;
using System.Text;
using CrackSoft.Utility;
using System.Collections.Generic;
using CrackSoft.RegExplore.Editors;
using CrackSoft.Collections;

namespace CrackSoft.RegExplore
{
    partial class MainForm : Form
    {
        RegSearcher searcher;
        DateTime searchStartTime;
        bool searchStarted;
        Properties.Settings settings;
        EventDictionary<string, string> favorites;
        
        public MainForm()
        {
            InitializeComponent();
            searcher = new RegSearcher();
            searcher.SearchComplete += new EventHandler<SearchCompleteEventArgs>(searcher_SearchComplete);
            searcher.MatchFound += new EventHandler<MatchFoundEventArgs>(searcher_MatchFound);
            favorites = new EventDictionary<string, string>();
            favorites.ItemAdded += new EventHandler<ItemEventArgs<string, string>>(favorites_ItemAdded);
            favorites.ItemRemoved += new EventHandler<ItemEventArgs<string, string>>(favorites_ItemRemoved);
        }

        void favorites_ItemRemoved(object sender, ItemEventArgs<string, string> e)
        {
            favoritesToolStripMenuItem.DropDownItems.RemoveByKey(e.Item.Key);
        }

        void favorites_ItemAdded(object sender, ItemEventArgs<string, string> e)
        {
            AddFavoriteMenuItem(e.Item.Key, e.Item.Value);
        }
        
        TreeNode CreateNode()
        {
            TreeNode node = new TreeNode();
            return node; 
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            settings = Properties.Settings.Default;
            LoadSettings();
            AddRootKeys();
            LoadFavorites();
            if (settings.LastKey != String.Empty)
                JumpToKey(settings.LastKey);

            if (!Environment.Is64BitProcess)
                qWORDValuePopupMenuItem.Visible = false;
        }

        private void LoadFavorites()
        {
            RegKey favoritesKey = RegKey.Parse(RegExplorer.RegistryFavoritePath);
            if (favoritesKey == null)
                return;

            List<RegValue> values = RegExplorer.GetValues(favoritesKey.Key);
            if (values.Count > 0)
            {                
                values.ForEach(val =>
                                {
                                    string key = val.Data.ToString();
                                    //removing "My Computer\" set by RegEdit
                                    key = key.Substring(key.IndexOf('\\') + 1);
                                    favorites[val.Name] = key;
                                });
            }
        }

        private void AddFavoriteMenuItem(string name, string key)
        {
            ToolStripItem item = new ToolStripMenuItem(name);
            item.Tag = key;
            item.Name = name;
            favoritesToolStripMenuItem.DropDownItems.Add(item);
            item.Click += new EventHandler(favoriteMenuItem_Click);
        }

        void favoriteMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            JumpToKey(item.Tag.ToString());
        }

        private void AddRootKeys()
        {
            AddRootKey(Microsoft.Win32.Registry.ClassesRoot);
            AddRootKey(Microsoft.Win32.Registry.CurrentUser);
            AddRootKey(Microsoft.Win32.Registry.LocalMachine);
            AddRootKey(Microsoft.Win32.Registry.Users);
            AddRootKey(Microsoft.Win32.Registry.CurrentConfig);
        }

        private void AdjustControls()
        {
            //bug fix: gbSearch.Width is incorrect when docking is enabled.
            int gbSearchWidth = this.Width - gbSearch.Left * 2 - tbSearch.Left * 2 - 8;
            btnFind.Left = gbSearchWidth - btnFind.Width - 6;
            txtPattern.Width = gbSearchWidth - txtPattern.Left - btnFind.Width - 12;
            txtBranch.Width = gbSearchWidth - txtBranch.Left - btnFind.Width - 12;
        }

        void AddRootKey(RegistryKey key)
        {
            TreeNode node = CreateNode(key.Name, key.Name, key);
            tvwKeys.Nodes.Add(node);
            node.Nodes.Add(CreateNode());
        }

        private TreeNode CreateNode(string key, string text, object tag)
        {
            TreeNode node = CreateNode();
            node.Text = text;
            node.Name = key;
            node.Tag = tag;
            return node;
        }

        void AddKeyToTree(TreeNode parent, RegKey subKey)
        {
            RegistryKey key = subKey.Key;
            TreeNode newNode = CreateNode(key.Name, subKey.Name, key);
            parent.Nodes.Add(newNode);
            if (key.SubKeyCount > 0)
                newNode.Nodes.Add(CreateNode());
        }

        private void tvwKeys_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode parentNode = e.Node;
            if (parentNode.FirstNode.Tag == null)
            {
                using (new BusyCursor(this))
                {
                    LoadSubKeys(parentNode);
                }
            }
        }

        private void LoadSubKeys(TreeNode parentNode)
        {
            tvwKeys.SuspendLayout();

            parentNode.Nodes.Clear();
            RegistryKey key = (RegistryKey) parentNode.Tag;
            var subKeys = RegExplorer.GetSubKeys(key);
            subKeys.OrderBy<RegKey, string>(subKey => subKey.Name);
            foreach (RegKey subKey in subKeys)
                AddKeyToTree(parentNode, subKey);

            tvwKeys.ResumeLayout();
        }

        private void tvwKeys_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RegistryKey key = e.Node.Tag as RegistryKey;
            LoadValues(key);
        }

        private void LoadValues(RegistryKey key)
        {            
            toolStripStatusLabel1.Text = key.Name;
            lstValues.Items.Clear();
            List<RegValue> values = RegExplorer.GetValues(key);
            if (values != null)
            {
                if (values.Count == 0)
                    AddValueToList(key, CreateDefaultValue());
                else
                {
                    lstValues.SuspendLayout();

                    RegValue defaultValue = CreateDefaultValue();
                    if (values.SingleOrDefault((val) =>  val.Name == defaultValue.Name) == null)
                        AddValueToList(key, defaultValue);
                    
                    foreach (RegValue value in values)
                        AddValueToList(key, value);                                           

                    lstValues.ResumeLayout();
                }
            }                
        }

        private static RegValue CreateDefaultValue()
        {
            return new RegValue(String.Empty, RegistryValueKind.String, "(value not set)");
        }

        private ListViewItem AddValueToList(RegistryKey key, RegValue value)
        {
            ListViewItem item = lstValues.Items.Add(value.Name);
            item.ImageKey = GetValueTypeIcon(value.Kind);
            item.Name = value.Name;
            item.Tag = key;
            item.SubItems.Add(value.Kind.ToDataType());
            ListViewItem.ListViewSubItem subItem = item.SubItems.Add(value.ToString());
            subItem.Tag = value;
            return item;
        }

        private string GetValueTypeIcon(RegistryValueKind registryValueKind)
        {
            if (registryValueKind == RegistryValueKind.ExpandString ||
                registryValueKind == RegistryValueKind.MultiString ||
                registryValueKind == RegistryValueKind.String)
                return "ascii";
            else
                return "binary";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            AdjustControls();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (btnFind.Text == "F&ind")
            {
                RegistryKey[] keys;

                if (cmbSearch.Text == "All Hives")
                {
                    keys = new RegistryKey[cmbSearch.Items.Count];
                    for (int i = 0; i < cmbSearch.Items.Count; i++)
                        keys[i] = RegUtility.ParseRootKey(cmbSearch.Items[i].ToString());
                }
                else
                    keys = new RegistryKey[] { RegUtility.ParseRootKey(cmbSearch.Text) };

                if (txtBranch.Text != String.Empty)
                {
                    keys[0] = keys[0].OpenSubKey(txtBranch.Text);
                    if (keys[0] == null)
                    {
                        UIUtility.DisplayError(this, Properties.Resources.Error_InvalidKey, txtBranch);
                        return;
                    }
                }

                RegSearchArgs searchArgs = GetSearchArgs(keys);
                StartSearch();
                try
                {                    
                    searcher.Start(searchArgs);
                }
                catch (ArgumentException ex)
                {
                    toolStripStatusLabel1.Text = "Ready.";
                    UIUtility.DisplayError(this, ex.Message, txtPattern);
                    EnableSearch();
                    return;
                }
                searchStarted = true;                
            }
            else
            {
                btnFind.Enabled = false;
                searcher.Stop();
            }
        }

        private void StartSearch()
        {
            DisableSearch();
            lstResults.Items.Clear();
            toolStripStatusLabel1.Text = "Searching...";
            searchStartTime = DateTime.Now;
        }

        private RegSearchArgs GetSearchArgs(RegistryKey[] keys)
        {
            RegSearchLookAt lookAt = GetSearchTarget();
            RegSearchArgs searchArgs = new RegSearchArgs(keys, txtPattern.Text, chkMatchCase.Checked, lookAt, chkUseRegex.Checked);
            return searchArgs;
        }

        private RegSearchLookAt GetSearchTarget()
        {
            RegSearchLookAt lookAt = 0;
            if (chkLookAtData.Checked)
                lookAt |= RegSearchLookAt.Data;
            if (chkLookAtValues.Checked)
                lookAt |= RegSearchLookAt.Values;
            if (chkLookAtKeys.Checked)
                lookAt |= RegSearchLookAt.Keys;
            return lookAt;
        }               

        void searcher_MatchFound(object sender, MatchFoundEventArgs e)
        {
            AddResultToListView(e.Match);
        }

        private void DisableSearch()
        {            
            btnFind.Text = "&Cancel";
            txtPattern.Enabled = chkLookAtKeys.Enabled = txtBranch.Enabled =
                chkLookAtValues.Enabled = chkLookAtData.Enabled = cmbSearch.Enabled = 
                chkMatchCase.Enabled = chkUseRegex.Enabled = false;
        }

        void searcher_SearchComplete(object sender, SearchCompleteEventArgs e)
        {
            double seconds = DateTime.Now.Subtract(searchStartTime).TotalSeconds;
            int matches = lstResults.Items.Count;
            btnFind.Enabled = true;
            EnableSearch();
            if (tabControl1.SelectedIndex == 1)
            {
                toolStripStatusLabel1.Text = String.Format("Found {0} matches in {1} seconds.", matches, seconds);
                searchStarted = false;
            }
        }

        private void AddResultToListView(RegSearchMatch result)
        {
            var item = lstResults.Items.Add(result.Key);
            item.Tag = result;
            item.SubItems.Add(RegUtility.GetRegValueName(result.Value));
            item.SubItems.Add(result.Data);
        }

        private void EnableSearch()
        {
            btnFind.Text = "F&ind";
            txtPattern.Enabled = chkLookAtKeys.Enabled = txtBranch.Enabled =
                chkLookAtValues.Enabled = chkLookAtData.Enabled = cmbSearch.Enabled =
                chkMatchCase.Enabled = chkUseRegex.Enabled = true;
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = (chkLookAtKeys.Checked || chkLookAtValues.Checked || chkLookAtData.Checked) &&
                txtPattern.Text != String.Empty;
        }

        private void txtPattern_TextChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = (chkLookAtKeys.Checked || chkLookAtValues.Checked || chkLookAtData.Checked) &&
                txtPattern.Text != String.Empty;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (searchStarted && searcher.IsBusy)
                    toolStripStatusLabel1.Text = "Searching...";
                else if (searchStarted)
                {
                    toolStripStatusLabel1.Text = "Search complete.";
                    searchStarted = false;
                }
                else
                    toolStripStatusLabel1.Text = "Ready.";
            }
            else
                toolStripStatusLabel1.Text = String.Empty;
        }

        private void aboutRegExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog(this);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnFindAction();            
        }

        private void OnFindAction()
        {
            string key = GetSelectedKey();
            if (key != String.Empty)
            {
                string hive;
                string branch;
                RegUtility.SplitKey(key, out hive, out branch);

                if (!searcher.IsBusy)
                {
                    cmbSearch.SelectedItem = hive;
                    txtBranch.Text = branch;
                }
            }
            tabControl1.SelectedTab = tbSearch;
            txtPattern.Focus();
        }

        void CreatePopupMenu()
        {
            newPopupMenuItem.Visible =
                popupMenuSeparatorNew.Visible = GetNewMenuState();

            modifyPopupMenuItem.Visible =
                popupMenuSeperatorModify.Visible = GetModifyMenuState();

            if (ActiveControl == tvwKeys)
            {
                expandPopupMenuItem.Visible = true;
                expandPopupMenuItem.Enabled = tvwKeys.SelectedNode.Nodes.Count > 0;
                expandPopupMenuItem.Text = tvwKeys.SelectedNode.IsExpanded ? "Collapse" : "Expand";
            }
            else
                expandPopupMenuItem.Visible = false;
            
            refreshPopupMenuItem.Enabled = GetRefreshMenuState();
            deletePopupMenuItem.Enabled = GetDeleteMenuState();
            
            exportPopupMenuItem.Visible = 
                popupMenuSeperatorExport.Visible = (ActiveControl != lstValues);
            exportPopupMenuItem.Enabled = (ActiveControl != lstResults) || lstResults.SelectedItems.Count == 1;

            copyKeyNamePopupMenuItem.Visible =                 
                popupMenuSeperatorCopyKeyName.Visible = GetCopyMenuState();
        }

        private bool GetNewMenuState()
        {
            if ((ActiveControl == tvwKeys || (ActiveControl == lstValues && lstValues.Items.Count>0)) 
                && tvwKeys.SelectedNode != null)
                return true;
            return false;
        }

        private bool GetModifyMenuState()
        {
            if (ActiveControl == lstValues && lstValues.SelectedItems.Count == 1)
                return true;
            return false;
        }

        private void lstResults_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && lstResults.SelectedItems.Count > 0)
                DisplayPopupMenu(lstResults, e);
        }

        private void lstValues_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                DisplayPopupMenu(lstValues, e);
        }

        private void DisplayPopupMenu(Control source, MouseEventArgs e)
        {
            CreatePopupMenu();
            contextMenuStrip1.Show(source, e.X, e.Y);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                if (ActiveControl == lstValues)
                    foreach (ListViewItem item in lstValues.Items)
                        item.Selected = true;
                else if (ActiveControl == lstResults)
                    foreach (ListViewItem item in lstResults.Items)
                        item.Selected = true;
            }
        }

        private void tvwKeys_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tvwKeys.SelectedNode != null)
                DisplayPopupMenu(tvwKeys, e);
        }

        private string GetSelectedKey()
        {
            if (ActiveControl == tvwKeys && tvwKeys.SelectedNode != null)
                return tvwKeys.SelectedNode.Name;
            else if (ActiveControl == lstValues && lstValues.SelectedItems.Count > 0)
                return lstValues.SelectedItems[0].Tag.ToString();
            else if (ActiveControl == lstResults && lstResults.SelectedItems.Count > 0)
                return lstResults.SelectedItems[0].Text;
            else
                return String.Empty;
        }

        private void copyKeyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnCopyKeyNameAction();
        }

        private void OnCopyKeyNameAction()
        {
            string key = GetSelectedKey();
            if (key != String.Empty)
                Clipboard.SetText(key);
        }

        private void tvwKeys_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Bug fix: treeview doesnt change the selection on right click
            if (e.Button == MouseButtons.Right)
                tvwKeys.SelectedNode = e.Node;
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {            
            CreateEditMenu();
        }

        private void CreateEditMenu()
        {
            newToolStripMenuItem.Visible =
                toolStripMenuSeperatorNew.Visible = GetNewMenuState();
            modifyToolStripMenuItem.Visible = 
                toolStripMenuSeperatorModify.Visible = GetModifyMenuState();
            refreshToolStripMenuItem.Enabled = GetRefreshMenuState();
            copyKeyNameToolStripMenuItem.Visible = 
                toolStripMenuSeperatorCopyKeyName.Visible = GetCopyMenuState();
            deleteToolStripMenuItem.Enabled = GetDeleteMenuState();            
        }

        private bool GetDeleteMenuState()
        {
            if (ActiveControl == tvwKeys && tvwKeys.SelectedNode != null)
                return (tvwKeys.SelectedNode.Level != 0);
            else if (ActiveControl is ListView && ((ListView)ActiveControl).SelectedItems.Count > 0)
                return true;
            return false;
        }

        private bool GetRefreshMenuState()
        {
            if ((ActiveControl == tvwKeys || (ActiveControl == lstValues && lstValues.Items.Count>0)) 
                && tvwKeys.SelectedNode != null)
                return true;
            else
                return false;
        }

        private bool GetCopyMenuState()
        {
            if ((ActiveControl == tvwKeys && tvwKeys.SelectedNode != null) ||
                (ActiveControl == lstResults && lstResults.SelectedItems.Count == 1))
                return true;
            return false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnDeleteAction();
        }

        private void OnRefreshAction()
        {
            if (tvwKeys.SelectedNode == null)
                return;

            using (new BusyCursor(this))
            {
                if (ActiveControl == tvwKeys || ActiveControl == lstValues)
                {
                    string lastSelectedKey = tvwKeys.SelectedNode.FullPath;
                    string lastSelectedValue = GetSelectedValue();

                    RefreshTreeView();

                    TreeNode[] matches = tvwKeys.Nodes.Find(lastSelectedKey, true);
                    if (matches.Length > 0)
                    {
                        tvwKeys.SelectedNode = matches[0];
                        SetSelectedValue(lastSelectedValue);
                    }
                }
            }
        }

        private void RefreshValues()
        {
            string key = GetSelectedValue();
            RegistryKey regKey = tvwKeys.SelectedNode.Tag as RegistryKey;
            if (regKey != null)
                LoadValues(regKey);
            SetSelectedValue(key);
        }

        private void SetSelectedValue(string key)
        {
            ListViewItem item = lstValues.Items[key];
            if (item != null)
                item.Selected = true;
        }

        private string GetSelectedValue()
        {
            if (lstValues.SelectedItems.Count == 1)
                return lstValues.SelectedItems[0].Name;
            else
                return String.Empty;
        }

        private void RefreshTreeView()
        {
            TreeNode targetNode;
            if (tvwKeys.SelectedNode == null)
                return;

            if (tvwKeys.SelectedNode.IsExpanded)
                targetNode = tvwKeys.SelectedNode;
            else if (tvwKeys.SelectedNode.Level == 0)
                return;
            else
                targetNode = tvwKeys.SelectedNode.Parent;

            bool error = false;
            do
                try
                {
                    LoadSubKeys(targetNode);
                }
                catch (IOException)
                {
                    error = true;
                    targetNode = targetNode.Parent;
                }
            while (error && targetNode.Level > 0);
        }

        private void OnDeleteAction()
        {
            if (ActiveControl == tvwKeys)
                DeleteTreeKey();
            else if (ActiveControl == lstValues)
                DeleteListValue();
            else if (ActiveControl == lstResults)
                DeleteListEntry();
        }

        private void DeleteListEntry()
        {
            if (UIUtility.ConfirmAction(this, Properties.Resources.Confirm_DeleteEntries, "Entry Delete", true))
                if (!DeleteEntries())
                    UIUtility.WarnUser(this, Properties.Resources.Error_DeleteEntriesFail);
        }

        private void DeleteListValue()
        {
            if (UIUtility.ConfirmAction(this, Properties.Resources.Confirm_DeleteValue, "Value Delete", true))
                if (!DeleteValues())
                    UIUtility.WarnUser(this, Properties.Resources.Error_DeleteValueFail);
        }

        private void DeleteTreeKey()
        {
            if (UIUtility.ConfirmAction(this, Properties.Resources.Confirm_DeleteKey, "Key Delete", true))
                if (RegUtility.DeleteKey(tvwKeys.SelectedNode.Tag.ToString()))
                    tvwKeys.SelectedNode.Remove();
                else
                    UIUtility.WarnUser(this, Properties.Resources.Error_DeleteKeyFail);
        }

        private bool DeleteEntries()
        {
            bool success = true;
            foreach (ListViewItem item in lstResults.SelectedItems)
            {
                RegSearchMatch match = (RegSearchMatch) item.Tag;
                // if value is not specified
                if (match.Value == "-")
                {
                    if (RegUtility.DeleteKey(match.Key))
                        item.Remove();
                    else
                        success = false;
                }
                else
                    if (RegUtility.DeleteValue(match.Key, match.Value))
                        item.Remove();
                    else
                        success = false;
            }
            return success;
        }

        private bool DeleteValues()
        {
            bool success = true;
            foreach (ListViewItem item in lstValues.SelectedItems)
            {
                if (RegUtility.DeleteValue(item.Tag.ToString(), item.Text))
                    item.Remove();
                else
                    success = false;
            }
            return success;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searcher != null)
                searcher.Stop();
            SaveSettings();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnRefreshAction();
        }

        private void tvwKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && GetDeleteMenuState())
                DeleteTreeKey();
        }

        private void expandPopupMenuItem_Click(object sender, EventArgs e)
        {
            if (tvwKeys.SelectedNode.IsExpanded)
                tvwKeys.SelectedNode.Collapse();
            else
                tvwKeys.SelectedNode.Expand();
        }

        private void LoadSettings()
        {
            if (settings.Location.X != -1)
                this.Location = settings.Location;
            this.Size = settings.Size;
            
            chkLookAtKeys.Checked = settings.LookAtKeys;
            chkLookAtValues.Checked = settings.LookAtValues;
            chkLookAtData.Checked = settings.LookAtData;
            chkMatchCase.Checked = settings.MatchCase;
            chkUseRegex.Checked = settings.UseRegEx;
            cmbSearch.SelectedItem = settings.SearchHive.Clone();
            lstValues.Columns[0].Width = settings.ValWidth1;
            lstValues.Columns[1].Width = settings.ValWidth2;
            lstValues.Columns[2].Width = settings.ValWidth3;
            lstResults.Columns[0].Width = settings.ResWidth1;
            lstResults.Columns[1].Width = settings.ResWidth2;
            lstResults.Columns[2].Width = settings.ResWidth3;

            if (settings.Maximized)
                this.WindowState = FormWindowState.Maximized;
        }

        private void SaveSettings()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                settings.Maximized = false;
                settings.Size = this.Size;
                settings.Location = this.Location;
            }
            else
                settings.Maximized = true;

            settings.LookAtKeys = chkLookAtKeys.Checked;
            settings.LookAtValues = chkLookAtValues.Checked;
            settings.LookAtData = chkLookAtData.Checked;
            settings.MatchCase = chkMatchCase.Checked;
            settings.UseRegEx = chkUseRegex.Checked;
            settings.SearchHive = cmbSearch.SelectedItem.ToString();
            settings.ValWidth1 = lstValues.Columns[0].Width;
            settings.ValWidth2 = lstValues.Columns[1].Width;
            settings.ValWidth3 = lstValues.Columns[2].Width;
            settings.ResWidth1 = lstResults.Columns[0].Width;
            settings.ResWidth2 = lstResults.Columns[1].Width;
            settings.ResWidth3 = lstResults.Columns[2].Width;
            if (tvwKeys.SelectedNode != null)
                settings.LastKey = tvwKeys.SelectedNode.Name;
            settings.Save();
        }

        private void lstResults_DoubleClick(object sender, EventArgs e)
        {
            if (lstResults.SelectedItems.Count == 1)
            {
                RegSearchMatch match = lstResults.SelectedItems[0].Tag as RegSearchMatch;
                if (JumpToKey(match.Key))
                {
                    if (match.Value != "-")
                    {
                        string valueName = RegUtility.GetRegValueName(match.Value);
                        ListViewItem item = lstValues.Items[valueName];
                        if (item != null)
                        {
                            item.Selected = true;
                            lstValues.Focus();
                        }
                    }
                }
            }
        }

        private bool JumpToKey(string key)
        {
            tabControl1.SelectedTab = tbExplorer;
            string[] tokens = key.Split('\\');
            TreeNode node = tvwKeys.Nodes[tokens[0]];
            if (node == null) 
                return false;
            SelectAndExpand(node);
            StringBuilder path = new StringBuilder(node.Name);
            for (int i = 1; i < tokens.Length; i++)
            {
                path.Append('\\');
                path.Append(tokens[i]);
                node = node.Nodes[path.ToString()];
                if (node == null)
                    return false;
                SelectAndExpand(node);
            }
            return true;
        }

        private void SelectAndExpand(TreeNode node)
        {
            node.EnsureVisible();
            tvwKeys.SelectedNode = node;
            node.Expand();
        }

        private void txtPattern_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                btnFind.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void lstValues_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && GetDeleteMenuState())
                DeleteListValue();
            else if (e.KeyCode == Keys.Enter)
                OnValueEditAction();
        }

        private void OnValueEditAction()
        {
            if (lstValues.SelectedItems.Count == 1)
            {
                RegValue value = (RegValue)lstValues.SelectedItems[0].SubItems[2].Tag;
                if (value.ParentKey != null)
                {
                    ValueEditor editor = null;

                    switch (value.Kind)
                    {
                        case RegistryValueKind.Binary:
                            editor = new BinaryEditor(value);
                            break;                        
                        case RegistryValueKind.MultiString:
                            editor = new MultiStringEditor(value);
                            break;
                        case RegistryValueKind.DWord:
                        case RegistryValueKind.QWord:
                            editor = new DWordEditor(value);
                            break;
                        case RegistryValueKind.String:
                        case RegistryValueKind.ExpandString:
                            editor = new StringEditor(value);
                            break;
                        case RegistryValueKind.Unknown:
                        default:
                            break;
                    }

                    if (editor != null)
                        if (editor.ShowDialog(this) == DialogResult.OK)
                            RefreshValues();
                }
            }
        }

        private void lstResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && GetDeleteMenuState())
                DeleteListEntry();
        }

        private void jumpToKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JumpToKeyDialog dialog = new JumpToKeyDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                if (!JumpToKey(dialog.txtKeyPath.Text))
                    UIUtility.DisplayError(this, Properties.Resources.Error_InvalidKey);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnExportAction();
        }

        private void OnExportAction()
        {
            string key = GetSelectedKey();
            ExportDialog dialog = new ExportDialog();
            dialog.cmbBranch.Text = key;
            dialog.ShowDialog(this);
        }

        private void crackSoftWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellUtility.OpenWebPage("http://www.cracksoft.net");
        }

        private void lstValues_DoubleClick(object sender, EventArgs e)
        {
            OnValueEditAction();
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnValueEditAction();
        }

        private void keyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewKeyAction();
        }

        private void OnNewKeyAction()
        {
            if (tvwKeys.SelectedNode == null)
                return;
            if (tvwKeys.HasChildren && !tvwKeys.SelectedNode.IsExpanded)
                tvwKeys.SelectedNode.Expand();
            RegistryKey key = (RegistryKey) tvwKeys.SelectedNode.Tag;
            string name = RegUtility.GetNewKeyName(key);
            string path = key.Name + "\\" + name;
            // adding new object() as tag to prevent this key from being deleted on expanding.
            TreeNode node = CreateNode(path, name, new object());
            tvwKeys.SelectedNode.Nodes.Add(node);
            node.EnsureVisible();
            tvwKeys.LabelEdit = true;            
            node.BeginEdit();
        }

        private void tvwKeys_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            tvwKeys.LabelEdit = false;
            string keyName = e.Label == null ? e.Node.Text : e.Label;
            try
            {                
                RegistryKey readOnlyParent = (RegistryKey)e.Node.Parent.Tag;
                RegKey parent = RegKey.Parse(readOnlyParent.Name, true);
                parent.Key.CreateSubKey(keyName);
                e.Node.Name = parent.Key.Name + "\\" + keyName;
                e.Node.Tag = RegKey.Parse(e.Node.Name).Key;
            }
            catch
            {
                e.Node.Remove();
                UIUtility.DisplayError(this, Properties.Resources.Error_CreateKeyFail);
            }
        }

        private void stringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.String);
        }

        private void OnNewValueAction(RegistryValueKind valueKind)
        {
            if (tvwKeys.SelectedNode == null)
                return;
            RegistryKey key = (RegistryKey) tvwKeys.SelectedNode.Tag;
            string name = RegUtility.GetNewValueName(key);
            ListViewItem item = AddValueToList(key, new RegValue(name, valueKind, valueKind.GetDefaultData()));
            lstValues.LabelEdit = true;
            item.BeginEdit();
        }

        private void binaryValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.Binary);
        }

        private void dWORDValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.DWord);
        }

        private void multiStringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.MultiString);
        }

        private void expandableStringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.ExpandString);
        }

        private void qWORDValuePopupMenuItem_Click(object sender, EventArgs e)
        {
            OnNewValueAction(RegistryValueKind.QWord);
        }

        private void lstValues_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            lstValues.LabelEdit = false;
            ListViewItem item = lstValues.Items[e.Item];
            string valName = e.Label == null ? item.Text : e.Label;
            try
            {                
                RegistryKey readOnlyKey = (RegistryKey)item.Tag;
                RegistryKey key = RegKey.Parse(readOnlyKey.Name, true).Key;
                RegValue value = (RegValue)item.SubItems[2].Tag;
                key.SetValue(valName, value.Data, value.Kind);
                item.Name = valName;
                item.SubItems[2].Tag = new RegValue(readOnlyKey, valName);
            }
            catch
            {
                item.Remove();
                UIUtility.DisplayError(this, Properties.Resources.Error_CreateValueFail);
            }
        }

        private void favoritesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            removeFavoriteToolStripMenuItem.Enabled = (favorites.Count > 0);
            addToFavoritesToolStripMenuItem.Enabled = (tvwKeys.SelectedNode != null);
            toolStripMenuSeperatorFavorites.Visible = (favorites.Count > 0);
        }

        private void addToFavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToFavoritesDialog dialog = new AddToFavoritesDialog();
            RegistryKey key = (RegistryKey)tvwKeys.SelectedNode.Tag;
            int i = key.Name.LastIndexOf('\\');
            dialog.txtName.SelectedText = i >= 0 ? key.Name.Substring(i + 1) : key.Name;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (favorites.ContainsKey(dialog.txtName.Text))
                    UIUtility.DisplayError(this, Properties.Resources.Error_AlreadyFavorite);
                else
                {
                    string path = "My Computer\\" + key.Name;
                    Microsoft.Win32.Registry.SetValue(RegExplorer.RegistryFavoritePath, dialog.txtName.Text, path);
                    favorites[dialog.txtName.Text] = key.Name;
                }
            }
        }

        private void removeFavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveFavoritesDialog dialog = new RemoveFavoritesDialog(favorites);
            dialog.ShowDialog(this);
        }

        private void cmbSearch_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbSearch.Text == "All Hives")
            {
                txtBranch.Text = String.Empty;
                txtBranch.Enabled = false;
            }
            else
                txtBranch.Enabled = true;
        }        
    }
}