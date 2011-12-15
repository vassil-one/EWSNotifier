using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EWSNotifier.ewswebreference;

namespace EWSNotifier.UI
{
    public class FolderTreeNode : TreeNode
    {
        public FolderType Folder { get; private set; }

        public FolderTreeNode(FolderType folder)
        {
            this.Folder = folder;
            this.Text = folder.DisplayName;
        }
    }
}
