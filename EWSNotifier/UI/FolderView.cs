using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EWSNotifier.Utility;
using EWSNotifier.ewswebreference;
using EWSNotifier.Model;
using EWSNotifier.Logging;

namespace EWSNotifier.UI
{
    public partial class FolderView : UserControl, ILoadingAware
    {
        public List<BaseFolderType> CheckedFolders
        {
            get
            {
                List<BaseFolderType> folderIds = new List<BaseFolderType>();
                GetCheckedNodes(null, folderIds);
                Configuration.FoldersToWatch = (from fid in folderIds select fid.FolderId.Id).ToList();
                Configuration.SaveSettings();
                return folderIds;
            }
        }

        public FolderView()
        {
            InitializeComponent();
        }

        public void LoadFolders(EWSManager ews)
        {
            OnLoadingBegin(new LoadingEventArgs());
            folderLoadBgWorker.RunWorkerAsync(ews);
        }

        private void folderLoadBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            EWSManager ews = (EWSManager)e.Argument;
            NTree<FolderType> folderTree = ews.FindFolders();
            e.Result = folderTree;
        }

        private void folderLoadBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool success = false;

            if (e.Error != null)
            {
                Logger.Log("Loading folders failed. Message: " + e.Error.Message);
                success = false;
            }
            else
            {
                success = true;
                NTree<FolderType> folderTree = (NTree<FolderType>)e.Result;
                FolderTreeNode uiNode = AddFolderToTreeView(folderTree, null);
                foreach (FolderTreeNode childNode in uiNode.Nodes)
                {
                    treeView1.Nodes.Add(childNode);
                    if (EWSNotifier.Utility.Configuration.FoldersToWatch.Contains(childNode.Folder.FolderId.Id))
                        childNode.Checked = true;
                }
                treeView1.ExpandAll();
                Logger.Log("Folders loaded");
            }

            OnLoadingEnd(new LoadingEventArgs() { LoadSuccessful = success });            
        }

        private FolderTreeNode AddFolderToTreeView(NTree<FolderType> folderTree, FolderTreeNode uiNode)
        {
            if (folderTree == null)
                return null;

            if (uiNode == null)
                uiNode = new FolderTreeNode(folderTree.Data);

            NTree<FolderType> childNode;
            int i = 0;
            while ((childNode = folderTree.getChild(i)) != null)
            {
                FolderTreeNode nextUINode = new FolderTreeNode(childNode.Data);
                if (EWSNotifier.Utility.Configuration.FoldersToWatch.Contains(nextUINode.Folder.FolderId.Id))
                    nextUINode.Checked = true;

                AddFolderToTreeView(childNode, nextUINode);
                uiNode.Nodes.Add(nextUINode);
                i++;
            }

            return uiNode;
        }

        private void GetCheckedNodes(FolderTreeNode currentNode, List<BaseFolderType> folders)
        {
            TreeNodeCollection nodes;
            if (currentNode == null)
                nodes = treeView1.Nodes;
            else
                nodes = currentNode.Nodes;

            foreach (FolderTreeNode node in nodes)
            {
                if (node.Checked)
                    folders.Add(node.Folder);
                GetCheckedNodes(node, folders);
            }
        }

        #region "ILoadingAware Members"
        public event LoadingEventHandler LoadingBegin;
        protected virtual void OnLoadingBegin(LoadingEventArgs e)
        {
            if (LoadingBegin != null)
                LoadingBegin(this, e);
        }

        public event LoadingEventHandler LoadingEnd;
        protected virtual void OnLoadingEnd(LoadingEventArgs e)
        {
            if (LoadingEnd != null)
                LoadingEnd(this, e);
        }
        #endregion  
    }
}
