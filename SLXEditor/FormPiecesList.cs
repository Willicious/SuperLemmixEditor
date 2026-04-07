using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormPiecesList : Form
    {
        private Level curLevel;
        private SLXEditForm mainForm;

        private ListView activeListView;
        private bool bypassSelectionChange = false;

        private List<GadgetPiece> gadgetPieceCache = new List<GadgetPiece>();
        private List<TerrainPiece> terrainPieceCache = new List<TerrainPiece>();

        internal FormPiecesList(Level level, SLXEditForm parentForm)
        {
            curLevel = level;
            mainForm = parentForm;

            InitializeComponent();

            Location = new System.Drawing.Point(parentForm.Left + 20, parentForm.Top + 80);
        }

        private string GetType(LevelPiece piece)
        {
            string s = piece.ObjType.ToString();
            if (string.IsNullOrEmpty(s))
                return s;

            return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }

        private ListViewItem PrepareItem(int index, LevelPiece piece)
        {
            ListViewItem item =
                new ListViewItem(index.ToString()); // Column 1: Index
                item.SubItems.Add(piece.Name);      // Column 2: Piece Name
                item.SubItems.Add(GetType(piece));  // Column 3: Type
                item.SubItems.Add(piece.DrawMode);  // Column 4: Draw Mode
            item.Tag = piece;
            return item;
        }

        private void UpdateDrawModeForListView(ListView listView)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Tag is LevelPiece piece)
                    item.SubItems[3].Text = piece.DrawMode;
            }
        }

        private bool ListsMatch<T>(IList<T> listA, IList<T> listB)
        {
            if (listA.Count != listB.Count)
                return false;

            for (int i = 0; i < listA.Count; i++)
            {
                if (!ReferenceEquals(listA[i], listB[i]))
                    return false;
            }

            return true;
        }

        private void PopulateTerrainPiecesList()
        {
            if (ListsMatch(curLevel.TerrainList, terrainPieceCache))
            {
                UpdateDrawModeForListView(listViewTerrain);
                return;
            }

            listViewTerrain.Items.Clear();
            terrainPieceCache.Clear();

            int index = 0;
            foreach (var piece in curLevel.TerrainList)
            {
                ListViewItem item = PrepareItem(index, piece);
                listViewTerrain.Items.Add(item);
                terrainPieceCache.Add(piece);
                index++;
            }
        }

        private void PopulateObjectPiecesList()
        {
            if (ListsMatch(curLevel.GadgetList, gadgetPieceCache))
            {
                UpdateDrawModeForListView(listViewObjects);
                return;
            }

            listViewObjects.Items.Clear();
            gadgetPieceCache.Clear();

            int index = 0;
            foreach (var piece in curLevel.GadgetList)
            {
                ListViewItem item = PrepareItem(index, piece);
                listViewObjects.Items.Add(item);
                gadgetPieceCache.Add(piece);
                index++;
            }
        }

        private void PopulatePiecesList()
        {
            if (activeListView == listViewTerrain)
                PopulateTerrainPiecesList();
            else if (activeListView == listViewObjects)
                PopulateObjectPiecesList();
            else
                return;
        }

        private void SyncSelectionFromLevel()
        {
            bypassSelectionChange = true;

            try
            {
                activeListView = null;

                foreach (ListViewItem item in listViewTerrain.Items)
                {
                    if (item.Tag is LevelPiece piece)
                        if (item.Selected != piece.IsSelected)
                            item.Selected = piece.IsSelected;
                }

                if (listViewTerrain.SelectedItems.Count > 0)
                {
                    this.ActiveControl = listViewTerrain;
                    activeListView = listViewTerrain;
                    return;
                }

                foreach (ListViewItem item in listViewObjects.Items)
                {
                    if (item.Tag is LevelPiece piece)
                        if (item.Selected != piece.IsSelected)
                            item.Selected = piece.IsSelected;
                }

                if (listViewObjects.SelectedItems.Count > 0)
                {
                    this.ActiveControl = listViewObjects;
                    activeListView = listViewObjects;
                }
            }
            finally
            {
                bypassSelectionChange = false;
            }
        }

        private void UpdatePiecePreview()
        {
            picPiecePreview.Image = null;
            picLemming.Visible = false;

            if (activeListView == null)
                return;

            LevelPiece firstSelected = null;
            foreach (ListViewItem item in activeListView.SelectedItems)
                if (item.Tag is LevelPiece piece)
                    firstSelected = piece;

            string key = firstSelected?.Key;
            if (key != null)
            {
                if (ImageLibrary.GetObjType(key) == C.OBJ.LEMMING)
                    picLemming.Visible = true;
                else
                    ImagePreview.PreviewPiece(key, picPiecePreview, curLevel.ThemeStyle);
            }
        }

        private void UpdateTitleBar()
        {
            int terrainCount = curLevel.TerrainList.Count;
            int gadgetCount = curLevel.GadgetList.Count;

            if (terrainCount + gadgetCount <= 0)
                this.Text = "Pieces List - Add a piece to the level to populate the list";
            else
                this.Text = "Pieces List";
        }

        private void RefreshLists()
        {
            PopulateTerrainPiecesList();
            PopulateObjectPiecesList();
            SyncSelectionFromLevel();
            UpdatePiecePreview();
            UpdateTitleBar();
        }

        private void HandleListSelection(ListView listView)
        {
            if (bypassSelectionChange)
                return;

            activeListView = listView;
            ClearOtherListView();
            PushSelectionToLevelArranger();
            UpdatePiecePreview();
        }

        private void HandleMovePieceIndex(bool toFront, bool moveByOne)
        {
            if (activeListView == null)
                return;

            mainForm.MovePieceIndex(toFront, moveByOne);
            PopulatePiecesList();
            SyncSelectionFromLevel();
            UpdatePiecePreview();
        }

        private void DeleteSelectedPieces()
        {
            mainForm.DeleteSelectedPieces();
            PopulatePiecesList();
            UpdatePiecePreview();
            UpdateTitleBar();
        }

        private void ClearOtherListView()
        {
            bypassSelectionChange = true;

            if (activeListView == listViewTerrain)
                listViewObjects.SelectedItems.Clear();
            else if (activeListView == listViewObjects)
                listViewTerrain.SelectedItems.Clear();

            bypassSelectionChange = false;
        }

        private void PushSelectionToLevelArranger()
        {
            if (activeListView == null)
                return;

            curLevel.UnselectAll();

            foreach (ListViewItem item in activeListView.SelectedItems)
            {
                if (item.Tag is LevelPiece piece)
                    piece.IsSelected = true;

            }

            mainForm.RefreshLevel();
        }

        private void FormPiecesList_Load(object sender, EventArgs e)
        {
            RefreshLists();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleListSelection(sender as ListView);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDrawFirst_Click(object sender, EventArgs e)
        {
            HandleMovePieceIndex(false, false);
        }

        private void btnDrawSooner_Click(object sender, EventArgs e)
        {
            HandleMovePieceIndex(false, true);
        }

        private void btnDrawLater_Click(object sender, EventArgs e)
        {
            HandleMovePieceIndex(true, true);
        }

        private void btnDrawLast_Click(object sender, EventArgs e)
        {
            HandleMovePieceIndex(true, false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedPieces();
        }

        private void FormPiecesList_Activated(object sender, EventArgs e)
        {
            this.Text = "Pieces List";
            RefreshLists();
        }

        private void FormPiecesList_Deactivate(object sender, EventArgs e)
        {
            this.Text = "Pieces List - Click title bar to refresh lists";
        }
    }
}
