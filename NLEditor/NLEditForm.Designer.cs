namespace NLEditor
{
    partial class NLEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NLEditForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPhysicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terrainRenderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectRenderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerAreasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deprecatedPiecesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validateLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combo_PieceStyle = new System.Windows.Forms.ComboBox();
            this.picPiece0 = new System.Windows.Forms.PictureBox();
            this.picPiece1 = new System.Windows.Forms.PictureBox();
            this.picPiece2 = new System.Windows.Forms.PictureBox();
            this.picPiece3 = new System.Windows.Forms.PictureBox();
            this.picPiece4 = new System.Windows.Forms.PictureBox();
            this.picPiece5 = new System.Windows.Forms.PictureBox();
            this.picPiece6 = new System.Windows.Forms.PictureBox();
            this.picPiece7 = new System.Windows.Forms.PictureBox();
            this.but_PieceTerrObj = new System.Windows.Forms.Button();
            this.pic_Level = new System.Windows.Forms.PictureBox();
            this.tabLvlProperties = new System.Windows.Forms.TabControl();
            this.tabGlobalInfo = new System.Windows.Forms.TabPage();
            this.combo_Background = new System.Windows.Forms.ComboBox();
            this.lbl_Global_Background = new System.Windows.Forms.Label();
            this.check_Lvl_InfTime = new System.Windows.Forms.CheckBox();
            this.combo_Music = new System.Windows.Forms.ComboBox();
            this.num_Lvl_TimeSec = new NLEditor.NumUpDownOverwrite();
            this.num_Lvl_TimeMin = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_TimeLimit = new System.Windows.Forms.Label();
            this.check_Lvl_LockSR = new System.Windows.Forms.CheckBox();
            this.num_Lvl_SR = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_SR = new System.Windows.Forms.Label();
            this.num_Lvl_Rescue = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_Rescue = new System.Windows.Forms.Label();
            this.num_Lvl_Lems = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_Lemmings = new System.Windows.Forms.Label();
            this.num_Lvl_StartY = new NLEditor.NumUpDownOverwrite();
            this.num_Lvl_StartX = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_StartPos = new System.Windows.Forms.Label();
            this.num_Lvl_SizeY = new NLEditor.NumUpDownOverwrite();
            this.num_Lvl_SizeX = new NLEditor.NumUpDownOverwrite();
            this.lbl_Global_Size = new System.Windows.Forms.Label();
            this.combo_MainStyle = new System.Windows.Forms.ComboBox();
            this.lbl_Global_Style = new System.Windows.Forms.Label();
            this.lbl_Global_Music = new System.Windows.Forms.Label();
            this.txt_LevelAuthor = new System.Windows.Forms.TextBox();
            this.lbl_Global_Author = new System.Windows.Forms.Label();
            this.txt_LevelTitle = new System.Windows.Forms.TextBox();
            this.lbl_Global_Title = new System.Windows.Forms.Label();
            this.tabPieces = new System.Windows.Forms.TabPage();
            this.check_Piece_Zombie = new System.Windows.Forms.CheckBox();
            this.check_Piece_Neutral = new System.Windows.Forms.CheckBox();
            this.check_Piece_Cloner = new System.Windows.Forms.CheckBox();
            this.check_Piece_Digger = new System.Windows.Forms.CheckBox();
            this.check_Piece_Fencer = new System.Windows.Forms.CheckBox();
            this.check_Piece_Miner = new System.Windows.Forms.CheckBox();
            this.check_Piece_Basher = new System.Windows.Forms.CheckBox();
            this.check_Piece_Builder = new System.Windows.Forms.CheckBox();
            this.check_Piece_Stacker = new System.Windows.Forms.CheckBox();
            this.check_Piece_Platformer = new System.Windows.Forms.CheckBox();
            this.check_Piece_Stoner = new System.Windows.Forms.CheckBox();
            this.check_Piece_Blocker = new System.Windows.Forms.CheckBox();
            this.check_Piece_Disarmer = new System.Windows.Forms.CheckBox();
            this.check_Piece_Bomber = new System.Windows.Forms.CheckBox();
            this.check_Piece_Glider = new System.Windows.Forms.CheckBox();
            this.check_Piece_Floater = new System.Windows.Forms.CheckBox();
            this.check_Piece_Swimmer = new System.Windows.Forms.CheckBox();
            this.check_Piece_Climber = new System.Windows.Forms.CheckBox();
            this.num_PickupSkillCount = new NLEditor.NumUpDownOverwrite();
            this.lbl_PickupSkillCount = new System.Windows.Forms.Label();
            this.num_LemmingLimit = new NLEditor.NumUpDownOverwrite();
            this.lbl_LemmingLimit = new System.Windows.Forms.Label();
            this.but_UngroupSelection = new System.Windows.Forms.Button();
            this.but_GroupSelection = new System.Windows.Forms.Button();
            this.but_PairTeleporter = new System.Windows.Forms.Button();
            this.lbl_Resize_Height = new System.Windows.Forms.Label();
            this.lbl_Resize_Width = new System.Windows.Forms.Label();
            this.check_Piece_Shimmier = new System.Windows.Forms.CheckBox();
            this.check_Piece_Walker = new System.Windows.Forms.CheckBox();
            this.check_Pieces_OneWay = new System.Windows.Forms.CheckBox();
            this.check_Pieces_OnlyOnTerrain = new System.Windows.Forms.CheckBox();
            this.check_Pieces_NoOv = new System.Windows.Forms.CheckBox();
            this.check_Pieces_Erase = new System.Windows.Forms.CheckBox();
            this.num_Resize_Height = new NLEditor.NumUpDownOverwrite();
            this.num_Resize_Width = new NLEditor.NumUpDownOverwrite();
            this.but_MoveBackOne = new NLEditor.RepeatButton();
            this.but_MoveFrontOne = new NLEditor.RepeatButton();
            this.but_MoveBack = new NLEditor.NoPaddingButton();
            this.but_MoveFront = new NLEditor.NoPaddingButton();
            this.but_FlipPieces = new NLEditor.RepeatButton();
            this.but_InvertPieces = new NLEditor.RepeatButton();
            this.but_RotatePieces = new NLEditor.RepeatButton();
            this.tabSkills = new System.Windows.Forms.TabPage();
            this.lbl_Skill_Shimmier = new System.Windows.Forms.Label();
            this.num_Ski_Shimmier = new NLEditor.NumUpDownOverwrite();
            this.lbl_Skill_Fencer = new System.Windows.Forms.Label();
            this.lbl_Skill_Cloner = new System.Windows.Forms.Label();
            this.lbl_Skill_Stacker = new System.Windows.Forms.Label();
            this.lbl_Skill_Platformer = new System.Windows.Forms.Label();
            this.lbl_Skill_Stoner = new System.Windows.Forms.Label();
            this.lbl_Skill_Disarmer = new System.Windows.Forms.Label();
            this.lbl_Skill_Glider = new System.Windows.Forms.Label();
            this.lbl_Skill_Swimmer = new System.Windows.Forms.Label();
            this.lbl_Skill_Walker = new System.Windows.Forms.Label();
            this.lbl_Skill_Digger = new System.Windows.Forms.Label();
            this.lbl_Skill_Miner = new System.Windows.Forms.Label();
            this.lbl_Skill_Basher = new System.Windows.Forms.Label();
            this.lbl_Skill_Builder = new System.Windows.Forms.Label();
            this.lbl_Skill_Bomber = new System.Windows.Forms.Label();
            this.lbl_Skill_Blocker = new System.Windows.Forms.Label();
            this.lbl_Skill_Floater = new System.Windows.Forms.Label();
            this.lbl_Skill_Climber = new System.Windows.Forms.Label();
            this.num_Ski_Fencer = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Cloner = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Stacker = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Platformer = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Stoner = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Disarmer = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Glider = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Swimmer = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Walker = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Digger = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Miner = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Basher = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Builder = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Bomber = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Blocker = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Floater = new NLEditor.NumUpDownOverwrite();
            this.num_Ski_Climber = new NLEditor.NumUpDownOverwrite();
            this.tabMisc = new System.Windows.Forms.TabPage();
            this.lblTalismans = new System.Windows.Forms.Label();
            this.lbTalismans = new System.Windows.Forms.ListBox();
            this.txt_Focus = new NLEditor.FocusTextBox();
            this.toolTipPieces = new System.Windows.Forms.ToolTip(this.components);
            this.tabLvlPieces = new System.Windows.Forms.TabControl();
            this.tabLvlSkills = new System.Windows.Forms.TabControl();
            this.scrollPicLevelHoriz = new System.Windows.Forms.HScrollBar();
            this.scrollPicLevelVert = new System.Windows.Forms.VScrollBar();
            this.pic_DragNewPiece = new System.Windows.Forms.PictureBox();
            this.toolTipButton = new System.Windows.Forms.ToolTip(this.components);
            this.but_PieceRight = new NLEditor.RepeatButton();
            this.but_PieceLeft = new NLEditor.RepeatButton();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Level)).BeginInit();
            this.tabLvlProperties.SuspendLayout();
            this.tabGlobalInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_TimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_TimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_Rescue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_Lems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_StartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_StartX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SizeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SizeX)).BeginInit();
            this.tabPieces.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PickupSkillCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_LemmingLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Resize_Height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Resize_Width)).BeginInit();
            this.tabSkills.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Shimmier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Fencer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Cloner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Stacker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Platformer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Stoner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Disarmer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Glider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Swimmer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Walker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Digger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Miner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Basher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Builder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Bomber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Blocker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Floater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Climber)).BeginInit();
            this.tabMisc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_DragNewPiece)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(792, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.newToolStripMenuItem.Text = "New (Ctrl+N)";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.loadToolStripMenuItem.Text = "Open (Ctrl+O)";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveToolStripMenuItem.Text = "Save (Ctrl+S)";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveAsToolStripMenuItem.Text = "Save as (Ctrl+Shift+S)";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exitToolStripMenuItem.Text = "Exit (Alt+F4)";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.duplicateCToolStripMenuItem,
            this.groupToolStripMenuItem,
            this.ungroupToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.undoToolStripMenuItem.Text = "Undo (Ctrl+Z)";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.redoToolStripMenuItem.Text = "Redo (Ctrl+Y)";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.deleteToolStripMenuItem.Text = "Cut (Ctrl+X)";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.insertToolStripMenuItem.Text = "Paste (Ctrl+V)";
            this.insertToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.copyToolStripMenuItem.Text = "Copy (Ctrl+C)";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // duplicateCToolStripMenuItem
            // 
            this.duplicateCToolStripMenuItem.Name = "duplicateCToolStripMenuItem";
            this.duplicateCToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.duplicateCToolStripMenuItem.Text = "Duplicate (C)";
            this.duplicateCToolStripMenuItem.Click += new System.EventHandler(this.duplicateCToolStripMenuItem_Click);
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            this.groupToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.groupToolStripMenuItem.Text = "Group (G)";
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.groupToolStripMenuItem_Click);
            // 
            // ungroupToolStripMenuItem
            // 
            this.ungroupToolStripMenuItem.Name = "ungroupToolStripMenuItem";
            this.ungroupToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.ungroupToolStripMenuItem.Text = "Ungroup (H)";
            this.ungroupToolStripMenuItem.Click += new System.EventHandler(this.ungroupToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPhysicsToolStripMenuItem,
            this.terrainRenderingToolStripMenuItem,
            this.objectRenderingToolStripMenuItem,
            this.triggerAreasToolStripMenuItem,
            this.screenStartToolStripMenuItem,
            this.backgroundToolStripMenuItem,
            this.deprecatedPiecesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // clearPhysicsToolStripMenuItem
            // 
            this.clearPhysicsToolStripMenuItem.CheckOnClick = true;
            this.clearPhysicsToolStripMenuItem.Name = "clearPhysicsToolStripMenuItem";
            this.clearPhysicsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.clearPhysicsToolStripMenuItem.Text = "Clear Physics (F1)";
            this.clearPhysicsToolStripMenuItem.Click += new System.EventHandler(this.clearPhysicsToolStripMenuItem_Click);
            // 
            // terrainRenderingToolStripMenuItem
            // 
            this.terrainRenderingToolStripMenuItem.Checked = true;
            this.terrainRenderingToolStripMenuItem.CheckOnClick = true;
            this.terrainRenderingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.terrainRenderingToolStripMenuItem.Name = "terrainRenderingToolStripMenuItem";
            this.terrainRenderingToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.terrainRenderingToolStripMenuItem.Text = "Terrain rendering (F2)";
            this.terrainRenderingToolStripMenuItem.Click += new System.EventHandler(this.terrainRenderingToolStripMenuItem_Click);
            // 
            // objectRenderingToolStripMenuItem
            // 
            this.objectRenderingToolStripMenuItem.Checked = true;
            this.objectRenderingToolStripMenuItem.CheckOnClick = true;
            this.objectRenderingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectRenderingToolStripMenuItem.Name = "objectRenderingToolStripMenuItem";
            this.objectRenderingToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.objectRenderingToolStripMenuItem.Text = "Object rendering (F3)";
            this.objectRenderingToolStripMenuItem.Click += new System.EventHandler(this.objectRenderingToolStripMenuItem_Click);
            // 
            // triggerAreasToolStripMenuItem
            // 
            this.triggerAreasToolStripMenuItem.CheckOnClick = true;
            this.triggerAreasToolStripMenuItem.Name = "triggerAreasToolStripMenuItem";
            this.triggerAreasToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.triggerAreasToolStripMenuItem.Text = "Trigger areas (F4)";
            this.triggerAreasToolStripMenuItem.Click += new System.EventHandler(this.triggerAreasToolStripMenuItem_Click);
            // 
            // screenStartToolStripMenuItem
            // 
            this.screenStartToolStripMenuItem.CheckOnClick = true;
            this.screenStartToolStripMenuItem.Name = "screenStartToolStripMenuItem";
            this.screenStartToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.screenStartToolStripMenuItem.Text = "Screen start (F5)";
            this.screenStartToolStripMenuItem.Click += new System.EventHandler(this.screenStartToolStripMenuItem_Click);
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.CheckOnClick = true;
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.backgroundToolStripMenuItem.Text = "Background image (F6)";
            this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
            // 
            // deprecatedPiecesToolStripMenuItem
            // 
            this.deprecatedPiecesToolStripMenuItem.CheckOnClick = true;
            this.deprecatedPiecesToolStripMenuItem.Name = "deprecatedPiecesToolStripMenuItem";
            this.deprecatedPiecesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.deprecatedPiecesToolStripMenuItem.Text = "Deprecated pieces (F7)";
            this.deprecatedPiecesToolStripMenuItem.Click += new System.EventHandler(this.deprecatedPiecesToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playLevelToolStripMenuItem,
            this.validateLevelToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // playLevelToolStripMenuItem
            // 
            this.playLevelToolStripMenuItem.Name = "playLevelToolStripMenuItem";
            this.playLevelToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.playLevelToolStripMenuItem.Text = "Play Level (F12)";
            this.playLevelToolStripMenuItem.Click += new System.EventHandler(this.playLevelToolStripMenuItem_Click);
            // 
            // validateLevelToolStripMenuItem
            // 
            this.validateLevelToolStripMenuItem.Name = "validateLevelToolStripMenuItem";
            this.validateLevelToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.validateLevelToolStripMenuItem.Text = "Validate Level";
            this.validateLevelToolStripMenuItem.Click += new System.EventHandler(this.validateLevelToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.hotkeysToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.helpToolStripMenuItem.Text = "Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.settingsToolStripMenuItem.Text = "Settings (F10)";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // hotkeysToolStripMenuItem
            // 
            this.hotkeysToolStripMenuItem.Name = "hotkeysToolStripMenuItem";
            this.hotkeysToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.hotkeysToolStripMenuItem.Text = "Hotkeys (F11)";
            this.hotkeysToolStripMenuItem.Click += new System.EventHandler(this.hotkeysToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // combo_PieceStyle
            // 
            this.combo_PieceStyle.FormattingEnabled = true;
            this.combo_PieceStyle.Location = new System.Drawing.Point(0, 451);
            this.combo_PieceStyle.Name = "combo_PieceStyle";
            this.combo_PieceStyle.Size = new System.Drawing.Size(97, 21);
            this.combo_PieceStyle.TabIndex = 24;
            this.combo_PieceStyle.TextChanged += new System.EventHandler(this.combo_PieceStyle_TextChanged);
            this.combo_PieceStyle.Leave += new System.EventHandler(this.combo_PieceStyle_Leave);
            // 
            // picPiece0
            // 
            this.picPiece0.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece0.Location = new System.Drawing.Point(36, 478);
            this.picPiece0.Name = "picPiece0";
            this.picPiece0.Size = new System.Drawing.Size(84, 84);
            this.picPiece0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece0.TabIndex = 25;
            this.picPiece0.TabStop = false;
            this.picPiece0.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece1
            // 
            this.picPiece1.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece1.Location = new System.Drawing.Point(126, 478);
            this.picPiece1.Name = "picPiece1";
            this.picPiece1.Size = new System.Drawing.Size(84, 84);
            this.picPiece1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece1.TabIndex = 27;
            this.picPiece1.TabStop = false;
            this.picPiece1.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece2
            // 
            this.picPiece2.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece2.Location = new System.Drawing.Point(216, 478);
            this.picPiece2.Name = "picPiece2";
            this.picPiece2.Size = new System.Drawing.Size(84, 84);
            this.picPiece2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece2.TabIndex = 28;
            this.picPiece2.TabStop = false;
            this.picPiece2.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece3
            // 
            this.picPiece3.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece3.Location = new System.Drawing.Point(306, 478);
            this.picPiece3.Name = "picPiece3";
            this.picPiece3.Size = new System.Drawing.Size(84, 84);
            this.picPiece3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece3.TabIndex = 30;
            this.picPiece3.TabStop = false;
            this.picPiece3.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece4
            // 
            this.picPiece4.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece4.Location = new System.Drawing.Point(396, 478);
            this.picPiece4.Name = "picPiece4";
            this.picPiece4.Size = new System.Drawing.Size(84, 84);
            this.picPiece4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece4.TabIndex = 31;
            this.picPiece4.TabStop = false;
            this.picPiece4.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece5
            // 
            this.picPiece5.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece5.Location = new System.Drawing.Point(486, 478);
            this.picPiece5.Name = "picPiece5";
            this.picPiece5.Size = new System.Drawing.Size(84, 84);
            this.picPiece5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece5.TabIndex = 32;
            this.picPiece5.TabStop = false;
            this.picPiece5.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece6
            // 
            this.picPiece6.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece6.Location = new System.Drawing.Point(576, 478);
            this.picPiece6.Name = "picPiece6";
            this.picPiece6.Size = new System.Drawing.Size(84, 84);
            this.picPiece6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece6.TabIndex = 33;
            this.picPiece6.TabStop = false;
            this.picPiece6.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // picPiece7
            // 
            this.picPiece7.BackColor = System.Drawing.SystemColors.Control;
            this.picPiece7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPiece7.Location = new System.Drawing.Point(666, 478);
            this.picPiece7.Name = "picPiece7";
            this.picPiece7.Size = new System.Drawing.Size(84, 84);
            this.picPiece7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPiece7.TabIndex = 34;
            this.picPiece7.TabStop = false;
            this.picPiece7.Click += new System.EventHandler(this.picPieces_Click);
            this.picPiece7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPieces_MouseDown);
            this.picPiece7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // but_PieceTerrObj
            // 
            this.but_PieceTerrObj.Location = new System.Drawing.Point(103, 451);
            this.but_PieceTerrObj.Name = "but_PieceTerrObj";
            this.but_PieceTerrObj.Size = new System.Drawing.Size(79, 21);
            this.but_PieceTerrObj.TabIndex = 35;
            this.but_PieceTerrObj.Text = "Get Objects";
            this.but_PieceTerrObj.UseVisualStyleBackColor = true;
            this.but_PieceTerrObj.Click += new System.EventHandler(this.but_PieceTerrObj_Click);
            // 
            // pic_Level
            // 
            this.pic_Level.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pic_Level.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_Level.Location = new System.Drawing.Point(188, 27);
            this.pic_Level.Name = "pic_Level";
            this.pic_Level.Size = new System.Drawing.Size(600, 445);
            this.pic_Level.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pic_Level.TabIndex = 36;
            this.pic_Level.TabStop = false;
            this.pic_Level.DoubleClick += new System.EventHandler(this.pic_Level_DoubleClick);
            this.pic_Level.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseDown);
            this.pic_Level.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseMove);
            this.pic_Level.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Level_MouseUp);
            // 
            // tabLvlProperties
            // 
            this.tabLvlProperties.Controls.Add(this.tabGlobalInfo);
            this.tabLvlProperties.Controls.Add(this.tabPieces);
            this.tabLvlProperties.Controls.Add(this.tabSkills);
            this.tabLvlProperties.Controls.Add(this.tabMisc);
            this.tabLvlProperties.Location = new System.Drawing.Point(0, 27);
            this.tabLvlProperties.Name = "tabLvlProperties";
            this.tabLvlProperties.SelectedIndex = 0;
            this.tabLvlProperties.Size = new System.Drawing.Size(182, 422);
            this.tabLvlProperties.TabIndex = 1;
            this.tabLvlProperties.TabStop = false;
            this.tabLvlProperties.Click += new System.EventHandler(this.tabLvlProperties_Click);
            // 
            // tabGlobalInfo
            // 
            this.tabGlobalInfo.Controls.Add(this.combo_Background);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Background);
            this.tabGlobalInfo.Controls.Add(this.check_Lvl_InfTime);
            this.tabGlobalInfo.Controls.Add(this.combo_Music);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_TimeSec);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_TimeMin);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_TimeLimit);
            this.tabGlobalInfo.Controls.Add(this.check_Lvl_LockSR);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_SR);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_SR);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_Rescue);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Rescue);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_Lems);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Lemmings);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_StartY);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_StartX);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_StartPos);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_SizeY);
            this.tabGlobalInfo.Controls.Add(this.num_Lvl_SizeX);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Size);
            this.tabGlobalInfo.Controls.Add(this.combo_MainStyle);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Style);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Music);
            this.tabGlobalInfo.Controls.Add(this.txt_LevelAuthor);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Author);
            this.tabGlobalInfo.Controls.Add(this.txt_LevelTitle);
            this.tabGlobalInfo.Controls.Add(this.lbl_Global_Title);
            this.tabGlobalInfo.Location = new System.Drawing.Point(4, 22);
            this.tabGlobalInfo.Name = "tabGlobalInfo";
            this.tabGlobalInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGlobalInfo.Size = new System.Drawing.Size(174, 396);
            this.tabGlobalInfo.TabIndex = 0;
            this.tabGlobalInfo.Text = "Globals";
            this.tabGlobalInfo.UseVisualStyleBackColor = true;
            // 
            // combo_Background
            // 
            this.combo_Background.FormattingEnabled = true;
            this.combo_Background.Items.AddRange(new object[] {
            "--none--"});
            this.combo_Background.Location = new System.Drawing.Point(70, 312);
            this.combo_Background.Name = "combo_Background";
            this.combo_Background.Size = new System.Drawing.Size(98, 21);
            this.combo_Background.TabIndex = 26;
            this.combo_Background.Text = "--none--";
            this.combo_Background.TextChanged += new System.EventHandler(this.combo_Background_TextChanged);
            this.combo_Background.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Background
            // 
            this.lbl_Global_Background.Location = new System.Drawing.Point(6, 315);
            this.lbl_Global_Background.Name = "lbl_Global_Background";
            this.lbl_Global_Background.Size = new System.Drawing.Size(86, 15);
            this.lbl_Global_Background.TabIndex = 25;
            this.lbl_Global_Background.Text = "Background";
            // 
            // check_Lvl_InfTime
            // 
            this.check_Lvl_InfTime.AutoSize = true;
            this.check_Lvl_InfTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Lvl_InfTime.Checked = true;
            this.check_Lvl_InfTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.check_Lvl_InfTime.Location = new System.Drawing.Point(59, 289);
            this.check_Lvl_InfTime.Name = "check_Lvl_InfTime";
            this.check_Lvl_InfTime.Size = new System.Drawing.Size(83, 17);
            this.check_Lvl_InfTime.TabIndex = 24;
            this.check_Lvl_InfTime.Text = "Infinite Time";
            this.check_Lvl_InfTime.UseVisualStyleBackColor = true;
            this.check_Lvl_InfTime.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // combo_Music
            // 
            this.combo_Music.FormattingEnabled = true;
            this.combo_Music.Location = new System.Drawing.Point(45, 57);
            this.combo_Music.Name = "combo_Music";
            this.combo_Music.Size = new System.Drawing.Size(123, 21);
            this.combo_Music.TabIndex = 5;
            this.combo_Music.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Lvl_TimeSec
            // 
            this.num_Lvl_TimeSec.Location = new System.Drawing.Point(121, 263);
            this.num_Lvl_TimeSec.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.num_Lvl_TimeSec.Name = "num_Lvl_TimeSec";
            this.num_Lvl_TimeSec.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_TimeSec.TabIndex = 23;
            this.num_Lvl_TimeSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_TimeSec.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Lvl_TimeMin
            // 
            this.num_Lvl_TimeMin.Location = new System.Drawing.Point(70, 263);
            this.num_Lvl_TimeMin.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_Lvl_TimeMin.Name = "num_Lvl_TimeMin";
            this.num_Lvl_TimeMin.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_TimeMin.TabIndex = 22;
            this.num_Lvl_TimeMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_TimeMin.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_TimeLimit
            // 
            this.lbl_Global_TimeLimit.Location = new System.Drawing.Point(6, 265);
            this.lbl_Global_TimeLimit.Name = "lbl_Global_TimeLimit";
            this.lbl_Global_TimeLimit.Size = new System.Drawing.Size(56, 15);
            this.lbl_Global_TimeLimit.TabIndex = 21;
            this.lbl_Global_TimeLimit.Text = "Time Limit";
            // 
            // check_Lvl_LockSR
            // 
            this.check_Lvl_LockSR.AutoSize = true;
            this.check_Lvl_LockSR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Lvl_LockSR.Location = new System.Drawing.Point(24, 240);
            this.check_Lvl_LockSR.Name = "check_Lvl_LockSR";
            this.check_Lvl_LockSR.Size = new System.Drawing.Size(112, 17);
            this.check_Lvl_LockSR.TabIndex = 20;
            this.check_Lvl_LockSR.Text = "Lock Spawn Rate";
            this.check_Lvl_LockSR.UseVisualStyleBackColor = true;
            this.check_Lvl_LockSR.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Lvl_SR
            // 
            this.num_Lvl_SR.Location = new System.Drawing.Point(95, 214);
            this.num_Lvl_SR.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_Lvl_SR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Lvl_SR.Name = "num_Lvl_SR";
            this.num_Lvl_SR.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_SR.TabIndex = 19;
            this.num_Lvl_SR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_SR.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_Lvl_SR.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_SR
            // 
            this.lbl_Global_SR.Location = new System.Drawing.Point(6, 216);
            this.lbl_Global_SR.Name = "lbl_Global_SR";
            this.lbl_Global_SR.Size = new System.Drawing.Size(81, 15);
            this.lbl_Global_SR.TabIndex = 18;
            this.lbl_Global_SR.Text = "Spawn Rate";
            // 
            // num_Lvl_Rescue
            // 
            this.num_Lvl_Rescue.Location = new System.Drawing.Point(95, 188);
            this.num_Lvl_Rescue.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.num_Lvl_Rescue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Lvl_Rescue.Name = "num_Lvl_Rescue";
            this.num_Lvl_Rescue.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_Rescue.TabIndex = 17;
            this.num_Lvl_Rescue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_Rescue.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_Lvl_Rescue.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Rescue
            // 
            this.lbl_Global_Rescue.Location = new System.Drawing.Point(6, 190);
            this.lbl_Global_Rescue.Name = "lbl_Global_Rescue";
            this.lbl_Global_Rescue.Size = new System.Drawing.Size(56, 15);
            this.lbl_Global_Rescue.TabIndex = 16;
            this.lbl_Global_Rescue.Text = "Rescue";
            // 
            // num_Lvl_Lems
            // 
            this.num_Lvl_Lems.Location = new System.Drawing.Point(95, 162);
            this.num_Lvl_Lems.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.num_Lvl_Lems.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Lvl_Lems.Name = "num_Lvl_Lems";
            this.num_Lvl_Lems.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_Lems.TabIndex = 15;
            this.num_Lvl_Lems.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_Lems.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.num_Lvl_Lems.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Lemmings
            // 
            this.lbl_Global_Lemmings.Location = new System.Drawing.Point(6, 164);
            this.lbl_Global_Lemmings.Name = "lbl_Global_Lemmings";
            this.lbl_Global_Lemmings.Size = new System.Drawing.Size(56, 15);
            this.lbl_Global_Lemmings.TabIndex = 14;
            this.lbl_Global_Lemmings.Text = "Lemmings";
            // 
            // num_Lvl_StartY
            // 
            this.num_Lvl_StartY.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Lvl_StartY.Location = new System.Drawing.Point(121, 136);
            this.num_Lvl_StartY.Maximum = new decimal(new int[] {
            159,
            0,
            0,
            0});
            this.num_Lvl_StartY.Name = "num_Lvl_StartY";
            this.num_Lvl_StartY.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_StartY.TabIndex = 13;
            this.num_Lvl_StartY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_StartY.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.num_Lvl_StartY.ValueChanged += new System.EventHandler(this.num_Lvl_StartY_ValueChanged);
            this.num_Lvl_StartY.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Lvl_StartX
            // 
            this.num_Lvl_StartX.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Lvl_StartX.Location = new System.Drawing.Point(70, 136);
            this.num_Lvl_StartX.Maximum = new decimal(new int[] {
            319,
            0,
            0,
            0});
            this.num_Lvl_StartX.Name = "num_Lvl_StartX";
            this.num_Lvl_StartX.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_StartX.TabIndex = 12;
            this.num_Lvl_StartX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_StartX.Value = new decimal(new int[] {
            160,
            0,
            0,
            0});
            this.num_Lvl_StartX.ValueChanged += new System.EventHandler(this.num_Lvl_StartX_ValueChanged);
            this.num_Lvl_StartX.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_StartPos
            // 
            this.lbl_Global_StartPos.Location = new System.Drawing.Point(6, 138);
            this.lbl_Global_StartPos.Name = "lbl_Global_StartPos";
            this.lbl_Global_StartPos.Size = new System.Drawing.Size(46, 15);
            this.lbl_Global_StartPos.TabIndex = 11;
            this.lbl_Global_StartPos.Text = "Start";
            // 
            // num_Lvl_SizeY
            // 
            this.num_Lvl_SizeY.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Lvl_SizeY.Location = new System.Drawing.Point(121, 110);
            this.num_Lvl_SizeY.Maximum = new decimal(new int[] {
            2400,
            0,
            0,
            0});
            this.num_Lvl_SizeY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Lvl_SizeY.Name = "num_Lvl_SizeY";
            this.num_Lvl_SizeY.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_SizeY.TabIndex = 10;
            this.num_Lvl_SizeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_SizeY.Value = new decimal(new int[] {
            160,
            0,
            0,
            0});
            this.num_Lvl_SizeY.ValueChanged += new System.EventHandler(this.num_Lvl_SizeY_ValueChanged);
            this.num_Lvl_SizeY.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Lvl_SizeX
            // 
            this.num_Lvl_SizeX.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Lvl_SizeX.Location = new System.Drawing.Point(70, 110);
            this.num_Lvl_SizeX.Maximum = new decimal(new int[] {
            2400,
            0,
            0,
            0});
            this.num_Lvl_SizeX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Lvl_SizeX.Name = "num_Lvl_SizeX";
            this.num_Lvl_SizeX.Size = new System.Drawing.Size(47, 20);
            this.num_Lvl_SizeX.TabIndex = 9;
            this.num_Lvl_SizeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Lvl_SizeX.Value = new decimal(new int[] {
            320,
            0,
            0,
            0});
            this.num_Lvl_SizeX.ValueChanged += new System.EventHandler(this.num_Lvl_SizeX_ValueChanged);
            this.num_Lvl_SizeX.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Size
            // 
            this.lbl_Global_Size.Location = new System.Drawing.Point(6, 115);
            this.lbl_Global_Size.Name = "lbl_Global_Size";
            this.lbl_Global_Size.Size = new System.Drawing.Size(46, 15);
            this.lbl_Global_Size.TabIndex = 8;
            this.lbl_Global_Size.Text = "Size";
            // 
            // combo_MainStyle
            // 
            this.combo_MainStyle.FormattingEnabled = true;
            this.combo_MainStyle.Location = new System.Drawing.Point(45, 84);
            this.combo_MainStyle.Name = "combo_MainStyle";
            this.combo_MainStyle.Size = new System.Drawing.Size(123, 21);
            this.combo_MainStyle.TabIndex = 7;
            this.combo_MainStyle.TextChanged += new System.EventHandler(this.combo_MainStyle_TextChanged);
            this.combo_MainStyle.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Style
            // 
            this.lbl_Global_Style.Location = new System.Drawing.Point(6, 89);
            this.lbl_Global_Style.Name = "lbl_Global_Style";
            this.lbl_Global_Style.Size = new System.Drawing.Size(60, 15);
            this.lbl_Global_Style.TabIndex = 6;
            this.lbl_Global_Style.Text = "Theme";
            // 
            // lbl_Global_Music
            // 
            this.lbl_Global_Music.Location = new System.Drawing.Point(6, 63);
            this.lbl_Global_Music.Name = "lbl_Global_Music";
            this.lbl_Global_Music.Size = new System.Drawing.Size(46, 15);
            this.lbl_Global_Music.TabIndex = 4;
            this.lbl_Global_Music.Text = "Music";
            // 
            // txt_LevelAuthor
            // 
            this.txt_LevelAuthor.Location = new System.Drawing.Point(45, 32);
            this.txt_LevelAuthor.MaxLength = 32;
            this.txt_LevelAuthor.Name = "txt_LevelAuthor";
            this.txt_LevelAuthor.Size = new System.Drawing.Size(123, 20);
            this.txt_LevelAuthor.TabIndex = 3;
            this.txt_LevelAuthor.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Author
            // 
            this.lbl_Global_Author.Location = new System.Drawing.Point(6, 37);
            this.lbl_Global_Author.Name = "lbl_Global_Author";
            this.lbl_Global_Author.Size = new System.Drawing.Size(46, 15);
            this.lbl_Global_Author.TabIndex = 2;
            this.lbl_Global_Author.Text = "Author";
            // 
            // txt_LevelTitle
            // 
            this.txt_LevelTitle.Location = new System.Drawing.Point(45, 6);
            this.txt_LevelTitle.MaxLength = 40;
            this.txt_LevelTitle.Name = "txt_LevelTitle";
            this.txt_LevelTitle.Size = new System.Drawing.Size(123, 20);
            this.txt_LevelTitle.TabIndex = 1;
            this.txt_LevelTitle.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // lbl_Global_Title
            // 
            this.lbl_Global_Title.Location = new System.Drawing.Point(6, 11);
            this.lbl_Global_Title.Name = "lbl_Global_Title";
            this.lbl_Global_Title.Size = new System.Drawing.Size(46, 15);
            this.lbl_Global_Title.TabIndex = 0;
            this.lbl_Global_Title.Text = "Title";
            // 
            // tabPieces
            // 
            this.tabPieces.Controls.Add(this.check_Piece_Zombie);
            this.tabPieces.Controls.Add(this.check_Piece_Neutral);
            this.tabPieces.Controls.Add(this.check_Piece_Cloner);
            this.tabPieces.Controls.Add(this.check_Piece_Digger);
            this.tabPieces.Controls.Add(this.check_Piece_Fencer);
            this.tabPieces.Controls.Add(this.check_Piece_Miner);
            this.tabPieces.Controls.Add(this.check_Piece_Basher);
            this.tabPieces.Controls.Add(this.check_Piece_Builder);
            this.tabPieces.Controls.Add(this.check_Piece_Stacker);
            this.tabPieces.Controls.Add(this.check_Piece_Platformer);
            this.tabPieces.Controls.Add(this.check_Piece_Stoner);
            this.tabPieces.Controls.Add(this.check_Piece_Blocker);
            this.tabPieces.Controls.Add(this.check_Piece_Disarmer);
            this.tabPieces.Controls.Add(this.check_Piece_Bomber);
            this.tabPieces.Controls.Add(this.check_Piece_Glider);
            this.tabPieces.Controls.Add(this.check_Piece_Floater);
            this.tabPieces.Controls.Add(this.check_Piece_Swimmer);
            this.tabPieces.Controls.Add(this.check_Piece_Climber);
            this.tabPieces.Controls.Add(this.num_PickupSkillCount);
            this.tabPieces.Controls.Add(this.lbl_PickupSkillCount);
            this.tabPieces.Controls.Add(this.num_LemmingLimit);
            this.tabPieces.Controls.Add(this.lbl_LemmingLimit);
            this.tabPieces.Controls.Add(this.but_UngroupSelection);
            this.tabPieces.Controls.Add(this.but_GroupSelection);
            this.tabPieces.Controls.Add(this.but_PairTeleporter);
            this.tabPieces.Controls.Add(this.lbl_Resize_Height);
            this.tabPieces.Controls.Add(this.lbl_Resize_Width);
            this.tabPieces.Controls.Add(this.check_Piece_Shimmier);
            this.tabPieces.Controls.Add(this.check_Piece_Walker);
            this.tabPieces.Controls.Add(this.check_Pieces_OneWay);
            this.tabPieces.Controls.Add(this.check_Pieces_OnlyOnTerrain);
            this.tabPieces.Controls.Add(this.check_Pieces_NoOv);
            this.tabPieces.Controls.Add(this.check_Pieces_Erase);
            this.tabPieces.Controls.Add(this.num_Resize_Height);
            this.tabPieces.Controls.Add(this.num_Resize_Width);
            this.tabPieces.Controls.Add(this.but_MoveBackOne);
            this.tabPieces.Controls.Add(this.but_MoveFrontOne);
            this.tabPieces.Controls.Add(this.but_MoveBack);
            this.tabPieces.Controls.Add(this.but_MoveFront);
            this.tabPieces.Controls.Add(this.but_FlipPieces);
            this.tabPieces.Controls.Add(this.but_InvertPieces);
            this.tabPieces.Controls.Add(this.but_RotatePieces);
            this.tabPieces.Location = new System.Drawing.Point(4, 22);
            this.tabPieces.Name = "tabPieces";
            this.tabPieces.Padding = new System.Windows.Forms.Padding(3);
            this.tabPieces.Size = new System.Drawing.Size(174, 396);
            this.tabPieces.TabIndex = 1;
            this.tabPieces.Text = "Pieces";
            this.tabPieces.UseVisualStyleBackColor = true;
            // 
            // check_Piece_Zombie
            // 
            this.check_Piece_Zombie.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Zombie.Location = new System.Drawing.Point(8, 325);
            this.check_Piece_Zombie.Name = "check_Piece_Zombie";
            this.check_Piece_Zombie.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Zombie.TabIndex = 31;
            this.check_Piece_Zombie.Text = "Zombie";
            this.check_Piece_Zombie.UseVisualStyleBackColor = true;
            this.check_Piece_Zombie.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Neutral
            // 
            this.check_Piece_Neutral.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Neutral.Location = new System.Drawing.Point(92, 325);
            this.check_Piece_Neutral.Name = "check_Piece_Neutral";
            this.check_Piece_Neutral.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Neutral.TabIndex = 31;
            this.check_Piece_Neutral.Text = "Neutral";
            this.check_Piece_Neutral.UseVisualStyleBackColor = true;
            this.check_Piece_Neutral.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Cloner
            // 
            this.check_Piece_Cloner.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Cloner.Location = new System.Drawing.Point(92, 310);
            this.check_Piece_Cloner.Name = "check_Piece_Cloner";
            this.check_Piece_Cloner.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Cloner.TabIndex = 29;
            this.check_Piece_Cloner.Text = "Cloner";
            this.check_Piece_Cloner.UseVisualStyleBackColor = true;
            this.check_Piece_Cloner.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Digger
            // 
            this.check_Piece_Digger.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Digger.Location = new System.Drawing.Point(8, 310);
            this.check_Piece_Digger.Name = "check_Piece_Digger";
            this.check_Piece_Digger.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Digger.TabIndex = 20;
            this.check_Piece_Digger.Text = "Digger";
            this.check_Piece_Digger.UseVisualStyleBackColor = true;
            this.check_Piece_Digger.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Fencer
            // 
            this.check_Piece_Fencer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Fencer.Location = new System.Drawing.Point(8, 295);
            this.check_Piece_Fencer.Name = "check_Piece_Fencer";
            this.check_Piece_Fencer.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Fencer.TabIndex = 21;
            this.check_Piece_Fencer.Text = "Fencer";
            this.check_Piece_Fencer.UseVisualStyleBackColor = true;
            this.check_Piece_Fencer.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Miner
            // 
            this.check_Piece_Miner.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Miner.Location = new System.Drawing.Point(92, 295);
            this.check_Piece_Miner.Name = "check_Piece_Miner";
            this.check_Piece_Miner.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Miner.TabIndex = 19;
            this.check_Piece_Miner.Text = "Miner";
            this.check_Piece_Miner.UseVisualStyleBackColor = true;
            this.check_Piece_Miner.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Basher
            // 
            this.check_Piece_Basher.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Basher.Location = new System.Drawing.Point(92, 280);
            this.check_Piece_Basher.Name = "check_Piece_Basher";
            this.check_Piece_Basher.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Basher.TabIndex = 18;
            this.check_Piece_Basher.Text = "Basher";
            this.check_Piece_Basher.UseVisualStyleBackColor = true;
            this.check_Piece_Basher.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Builder
            // 
            this.check_Piece_Builder.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Builder.Location = new System.Drawing.Point(92, 265);
            this.check_Piece_Builder.Name = "check_Piece_Builder";
            this.check_Piece_Builder.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Builder.TabIndex = 17;
            this.check_Piece_Builder.Text = "Builder";
            this.check_Piece_Builder.UseVisualStyleBackColor = true;
            this.check_Piece_Builder.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Stacker
            // 
            this.check_Piece_Stacker.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Stacker.Location = new System.Drawing.Point(8, 280);
            this.check_Piece_Stacker.Name = "check_Piece_Stacker";
            this.check_Piece_Stacker.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Stacker.TabIndex = 28;
            this.check_Piece_Stacker.Text = "Stacker";
            this.check_Piece_Stacker.UseVisualStyleBackColor = true;
            this.check_Piece_Stacker.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Platformer
            // 
            this.check_Piece_Platformer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Platformer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check_Piece_Platformer.Location = new System.Drawing.Point(8, 265);
            this.check_Piece_Platformer.Name = "check_Piece_Platformer";
            this.check_Piece_Platformer.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Platformer.TabIndex = 27;
            this.check_Piece_Platformer.Text = "Platformer";
            this.check_Piece_Platformer.UseVisualStyleBackColor = true;
            this.check_Piece_Platformer.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Stoner
            // 
            this.check_Piece_Stoner.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Stoner.Location = new System.Drawing.Point(8, 250);
            this.check_Piece_Stoner.Name = "check_Piece_Stoner";
            this.check_Piece_Stoner.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Stoner.TabIndex = 26;
            this.check_Piece_Stoner.Text = "Stoner";
            this.check_Piece_Stoner.UseVisualStyleBackColor = true;
            this.check_Piece_Stoner.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Blocker
            // 
            this.check_Piece_Blocker.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Blocker.Location = new System.Drawing.Point(92, 250);
            this.check_Piece_Blocker.Name = "check_Piece_Blocker";
            this.check_Piece_Blocker.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Blocker.TabIndex = 16;
            this.check_Piece_Blocker.Text = "Blocker";
            this.check_Piece_Blocker.UseVisualStyleBackColor = true;
            this.check_Piece_Blocker.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Disarmer
            // 
            this.check_Piece_Disarmer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Disarmer.Location = new System.Drawing.Point(8, 235);
            this.check_Piece_Disarmer.Name = "check_Piece_Disarmer";
            this.check_Piece_Disarmer.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Disarmer.TabIndex = 25;
            this.check_Piece_Disarmer.Text = "Disarmer";
            this.check_Piece_Disarmer.UseVisualStyleBackColor = true;
            this.check_Piece_Disarmer.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Bomber
            // 
            this.check_Piece_Bomber.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Bomber.Location = new System.Drawing.Point(92, 235);
            this.check_Piece_Bomber.Name = "check_Piece_Bomber";
            this.check_Piece_Bomber.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Bomber.TabIndex = 15;
            this.check_Piece_Bomber.Text = "Bomber";
            this.check_Piece_Bomber.UseVisualStyleBackColor = true;
            this.check_Piece_Bomber.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Glider
            // 
            this.check_Piece_Glider.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Glider.Location = new System.Drawing.Point(92, 220);
            this.check_Piece_Glider.Name = "check_Piece_Glider";
            this.check_Piece_Glider.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Glider.TabIndex = 24;
            this.check_Piece_Glider.Text = "Glider";
            this.check_Piece_Glider.UseVisualStyleBackColor = true;
            this.check_Piece_Glider.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Floater
            // 
            this.check_Piece_Floater.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Floater.Location = new System.Drawing.Point(8, 220);
            this.check_Piece_Floater.Name = "check_Piece_Floater";
            this.check_Piece_Floater.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Floater.TabIndex = 14;
            this.check_Piece_Floater.Text = "Floater";
            this.check_Piece_Floater.UseVisualStyleBackColor = true;
            this.check_Piece_Floater.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Swimmer
            // 
            this.check_Piece_Swimmer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Swimmer.Location = new System.Drawing.Point(92, 205);
            this.check_Piece_Swimmer.Name = "check_Piece_Swimmer";
            this.check_Piece_Swimmer.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Swimmer.TabIndex = 23;
            this.check_Piece_Swimmer.Text = "Swimmer";
            this.check_Piece_Swimmer.UseVisualStyleBackColor = true;
            this.check_Piece_Swimmer.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Climber
            // 
            this.check_Piece_Climber.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Climber.Location = new System.Drawing.Point(8, 205);
            this.check_Piece_Climber.Name = "check_Piece_Climber";
            this.check_Piece_Climber.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Climber.TabIndex = 13;
            this.check_Piece_Climber.Text = "Climber";
            this.check_Piece_Climber.UseVisualStyleBackColor = true;
            this.check_Piece_Climber.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // num_PickupSkillCount
            // 
            this.num_PickupSkillCount.Location = new System.Drawing.Point(69, 350);
            this.num_PickupSkillCount.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_PickupSkillCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_PickupSkillCount.Name = "num_PickupSkillCount";
            this.num_PickupSkillCount.Size = new System.Drawing.Size(47, 20);
            this.num_PickupSkillCount.TabIndex = 35;
            this.num_PickupSkillCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_PickupSkillCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_PickupSkillCount.Visible = false;
            this.num_PickupSkillCount.ValueChanged += new System.EventHandler(this.num_PickupSkillCount_ValueChanged);
            // 
            // lbl_PickupSkillCount
            // 
            this.lbl_PickupSkillCount.Location = new System.Drawing.Point(9, 352);
            this.lbl_PickupSkillCount.Name = "lbl_PickupSkillCount";
            this.lbl_PickupSkillCount.Size = new System.Drawing.Size(60, 15);
            this.lbl_PickupSkillCount.TabIndex = 34;
            this.lbl_PickupSkillCount.Text = "Skill Count";
            this.lbl_PickupSkillCount.Visible = false;
            // 
            // num_LemmingLimit
            // 
            this.num_LemmingLimit.Location = new System.Drawing.Point(69, 350);
            this.num_LemmingLimit.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_LemmingLimit.Name = "num_LemmingLimit";
            this.num_LemmingLimit.Size = new System.Drawing.Size(47, 20);
            this.num_LemmingLimit.TabIndex = 35;
            this.num_LemmingLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_LemmingLimit.Visible = false;
            this.num_LemmingLimit.ValueChanged += new System.EventHandler(this.num_LemmingLimit_ValueChanged);
            // 
            // lbl_LemmingLimit
            // 
            this.lbl_LemmingLimit.Location = new System.Drawing.Point(9, 352);
            this.lbl_LemmingLimit.Name = "lbl_LemmingLimit";
            this.lbl_LemmingLimit.Size = new System.Drawing.Size(60, 15);
            this.lbl_LemmingLimit.TabIndex = 34;
            this.lbl_LemmingLimit.Text = "Lemming Limit";
            this.lbl_LemmingLimit.Visible = false;
            // 
            // but_UngroupSelection
            // 
            this.but_UngroupSelection.Location = new System.Drawing.Point(90, 84);
            this.but_UngroupSelection.Name = "but_UngroupSelection";
            this.but_UngroupSelection.Size = new System.Drawing.Size(80, 34);
            this.but_UngroupSelection.TabIndex = 8;
            this.but_UngroupSelection.Text = "Ungroup";
            this.but_UngroupSelection.UseVisualStyleBackColor = true;
            this.but_UngroupSelection.Click += new System.EventHandler(this.but_UngroupSelection_Click);
            // 
            // but_GroupSelection
            // 
            this.but_GroupSelection.Location = new System.Drawing.Point(4, 84);
            this.but_GroupSelection.Name = "but_GroupSelection";
            this.but_GroupSelection.Size = new System.Drawing.Size(80, 34);
            this.but_GroupSelection.TabIndex = 7;
            this.but_GroupSelection.Text = "Group";
            this.but_GroupSelection.UseVisualStyleBackColor = true;
            this.but_GroupSelection.Click += new System.EventHandler(this.but_GroupSelection_Click);
            // 
            // but_PairTeleporter
            // 
            this.but_PairTeleporter.Location = new System.Drawing.Point(50, 350);
            this.but_PairTeleporter.Name = "but_PairTeleporter";
            this.but_PairTeleporter.Size = new System.Drawing.Size(76, 36);
            this.but_PairTeleporter.TabIndex = 33;
            this.but_PairTeleporter.Text = "Pair Teleporter";
            this.but_PairTeleporter.UseVisualStyleBackColor = true;
            this.but_PairTeleporter.Visible = false;
            this.but_PairTeleporter.Click += new System.EventHandler(this.but_PairTeleporter_Click);
            // 
            // lbl_Resize_Height
            // 
            this.lbl_Resize_Height.Location = new System.Drawing.Point(9, 375);
            this.lbl_Resize_Height.Name = "lbl_Resize_Height";
            this.lbl_Resize_Height.Size = new System.Drawing.Size(46, 15);
            this.lbl_Resize_Height.TabIndex = 32;
            this.lbl_Resize_Height.Text = "Height";
            this.lbl_Resize_Height.Visible = false;
            // 
            // lbl_Resize_Width
            // 
            this.lbl_Resize_Width.Location = new System.Drawing.Point(9, 353);
            this.lbl_Resize_Width.Name = "lbl_Resize_Width";
            this.lbl_Resize_Width.Size = new System.Drawing.Size(46, 15);
            this.lbl_Resize_Width.TabIndex = 31;
            this.lbl_Resize_Width.Text = "Width";
            this.lbl_Resize_Width.Visible = false;
            // 
            // check_Piece_Shimmier
            // 
            this.check_Piece_Shimmier.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Shimmier.Location = new System.Drawing.Point(92, 190);
            this.check_Piece_Shimmier.Name = "check_Piece_Shimmier";
            this.check_Piece_Shimmier.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Shimmier.TabIndex = 30;
            this.check_Piece_Shimmier.Text = "Shimmier";
            this.check_Piece_Shimmier.UseVisualStyleBackColor = true;
            this.check_Piece_Shimmier.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Piece_Walker
            // 
            this.check_Piece_Walker.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Piece_Walker.Location = new System.Drawing.Point(8, 190);
            this.check_Piece_Walker.Name = "check_Piece_Walker";
            this.check_Piece_Walker.Size = new System.Drawing.Size(76, 17);
            this.check_Piece_Walker.TabIndex = 22;
            this.check_Piece_Walker.Text = "Walker";
            this.check_Piece_Walker.UseVisualStyleBackColor = true;
            this.check_Piece_Walker.CheckedChanged += new System.EventHandler(this.check_Piece_Skill_CheckedChanged);
            // 
            // check_Pieces_OneWay
            // 
            this.check_Pieces_OneWay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Pieces_OneWay.Location = new System.Drawing.Point(32, 165);
            this.check_Pieces_OneWay.Name = "check_Pieces_OneWay";
            this.check_Pieces_OneWay.Size = new System.Drawing.Size(103, 17);
            this.check_Pieces_OneWay.TabIndex = 12;
            this.check_Pieces_OneWay.Text = "Allow One-Way";
            this.check_Pieces_OneWay.UseVisualStyleBackColor = true;
            this.check_Pieces_OneWay.CheckedChanged += new System.EventHandler(this.check_Pieces_OneWay_CheckedChanged);
            // 
            // check_Pieces_OnlyOnTerrain
            // 
            this.check_Pieces_OnlyOnTerrain.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Pieces_OnlyOnTerrain.Location = new System.Drawing.Point(32, 150);
            this.check_Pieces_OnlyOnTerrain.Name = "check_Pieces_OnlyOnTerrain";
            this.check_Pieces_OnlyOnTerrain.Size = new System.Drawing.Size(103, 17);
            this.check_Pieces_OnlyOnTerrain.TabIndex = 11;
            this.check_Pieces_OnlyOnTerrain.Text = "Only On Terrain";
            this.check_Pieces_OnlyOnTerrain.UseVisualStyleBackColor = true;
            this.check_Pieces_OnlyOnTerrain.CheckedChanged += new System.EventHandler(this.check_Pieces_OnlyOnTerrain_CheckedChanged);
            // 
            // check_Pieces_NoOv
            // 
            this.check_Pieces_NoOv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Pieces_NoOv.Location = new System.Drawing.Point(32, 135);
            this.check_Pieces_NoOv.Name = "check_Pieces_NoOv";
            this.check_Pieces_NoOv.Size = new System.Drawing.Size(103, 17);
            this.check_Pieces_NoOv.TabIndex = 10;
            this.check_Pieces_NoOv.Text = "No Overwrite";
            this.check_Pieces_NoOv.UseVisualStyleBackColor = true;
            this.check_Pieces_NoOv.CheckedChanged += new System.EventHandler(this.check_Pieces_NoOv_CheckedChanged);
            // 
            // check_Pieces_Erase
            // 
            this.check_Pieces_Erase.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_Pieces_Erase.Location = new System.Drawing.Point(32, 120);
            this.check_Pieces_Erase.Name = "check_Pieces_Erase";
            this.check_Pieces_Erase.Size = new System.Drawing.Size(103, 17);
            this.check_Pieces_Erase.TabIndex = 9;
            this.check_Pieces_Erase.Text = "Erase Terrain";
            this.check_Pieces_Erase.UseVisualStyleBackColor = true;
            this.check_Pieces_Erase.CheckedChanged += new System.EventHandler(this.check_Pieces_Erase_CheckedChanged);
            // 
            // num_Resize_Height
            // 
            this.num_Resize_Height.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Resize_Height.Location = new System.Drawing.Point(69, 373);
            this.num_Resize_Height.Maximum = new decimal(new int[] {
            160,
            0,
            0,
            0});
            this.num_Resize_Height.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Resize_Height.Name = "num_Resize_Height";
            this.num_Resize_Height.Size = new System.Drawing.Size(47, 20);
            this.num_Resize_Height.TabIndex = 30;
            this.num_Resize_Height.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Resize_Height.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Resize_Height.Visible = false;
            this.num_Resize_Height.ValueChanged += new System.EventHandler(this.num_Resize_Height_ValueChanged);
            // 
            // num_Resize_Width
            // 
            this.num_Resize_Width.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Resize_Width.Location = new System.Drawing.Point(69, 351);
            this.num_Resize_Width.Maximum = new decimal(new int[] {
            320,
            0,
            0,
            0});
            this.num_Resize_Width.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Resize_Width.Name = "num_Resize_Width";
            this.num_Resize_Width.Size = new System.Drawing.Size(47, 20);
            this.num_Resize_Width.TabIndex = 29;
            this.num_Resize_Width.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Resize_Width.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Resize_Width.Visible = false;
            this.num_Resize_Width.ValueChanged += new System.EventHandler(this.num_Resize_Width_ValueChanged);
            // 
            // but_MoveBackOne
            // 
            this.but_MoveBackOne.Location = new System.Drawing.Point(88, 46);
            this.but_MoveBackOne.Name = "but_MoveBackOne";
            this.but_MoveBackOne.NoPaddingText = "Back";
            this.but_MoveBackOne.Size = new System.Drawing.Size(46, 34);
            this.but_MoveBackOne.TabIndex = 5;
            this.but_MoveBackOne.UseVisualStyleBackColor = true;
            this.but_MoveBackOne.Click += new System.EventHandler(this.but_MoveBackOne_Click);
            this.but_MoveBackOne.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_MoveBackOne_MouseUp);
            // 
            // but_MoveFrontOne
            // 
            this.but_MoveFrontOne.Location = new System.Drawing.Point(40, 46);
            this.but_MoveFrontOne.Name = "but_MoveFrontOne";
            this.but_MoveFrontOne.NoPaddingText = "Forward";
            this.but_MoveFrontOne.Size = new System.Drawing.Size(46, 34);
            this.but_MoveFrontOne.TabIndex = 4;
            this.but_MoveFrontOne.UseVisualStyleBackColor = true;
            this.but_MoveFrontOne.Click += new System.EventHandler(this.but_MoveFrontOne_Click);
            this.but_MoveFrontOne.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_MoveFrontOne_MouseUp);
            // 
            // but_MoveBack
            // 
            this.but_MoveBack.Location = new System.Drawing.Point(136, 46);
            this.but_MoveBack.Name = "but_MoveBack";
            this.but_MoveBack.NoPaddingText = "To Back";
            this.but_MoveBack.Size = new System.Drawing.Size(34, 34);
            this.but_MoveBack.TabIndex = 6;
            this.but_MoveBack.UseVisualStyleBackColor = true;
            this.but_MoveBack.Click += new System.EventHandler(this.but_MoveBack_Click);
            // 
            // but_MoveFront
            // 
            this.but_MoveFront.Location = new System.Drawing.Point(4, 46);
            this.but_MoveFront.Name = "but_MoveFront";
            this.but_MoveFront.NoPaddingText = "To Front";
            this.but_MoveFront.Size = new System.Drawing.Size(34, 34);
            this.but_MoveFront.TabIndex = 3;
            this.but_MoveFront.UseVisualStyleBackColor = true;
            this.but_MoveFront.Click += new System.EventHandler(this.but_MoveFront_Click);
            // 
            // but_FlipPieces
            // 
            this.but_FlipPieces.Location = new System.Drawing.Point(118, 8);
            this.but_FlipPieces.Name = "but_FlipPieces";
            this.but_FlipPieces.NoPaddingText = null;
            this.but_FlipPieces.Size = new System.Drawing.Size(52, 34);
            this.but_FlipPieces.TabIndex = 2;
            this.but_FlipPieces.Text = "Flip";
            this.but_FlipPieces.UseVisualStyleBackColor = true;
            this.but_FlipPieces.Click += new System.EventHandler(this.but_FlipPieces_Click);
            this.but_FlipPieces.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_FlipPieces_MouseUp);
            // 
            // but_InvertPieces
            // 
            this.but_InvertPieces.Location = new System.Drawing.Point(60, 8);
            this.but_InvertPieces.Name = "but_InvertPieces";
            this.but_InvertPieces.NoPaddingText = null;
            this.but_InvertPieces.Size = new System.Drawing.Size(53, 34);
            this.but_InvertPieces.TabIndex = 1;
            this.but_InvertPieces.Text = "Invert";
            this.but_InvertPieces.UseVisualStyleBackColor = true;
            this.but_InvertPieces.Click += new System.EventHandler(this.but_InvertPieces_Click);
            this.but_InvertPieces.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_InvertPieces_MouseUp);
            // 
            // but_RotatePieces
            // 
            this.but_RotatePieces.Location = new System.Drawing.Point(4, 8);
            this.but_RotatePieces.Name = "but_RotatePieces";
            this.but_RotatePieces.NoPaddingText = null;
            this.but_RotatePieces.Size = new System.Drawing.Size(52, 34);
            this.but_RotatePieces.TabIndex = 0;
            this.but_RotatePieces.Text = "Rotate";
            this.but_RotatePieces.UseVisualStyleBackColor = true;
            this.but_RotatePieces.Click += new System.EventHandler(this.but_RotatePieces_Click);
            this.but_RotatePieces.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_RotatePieces_MouseUp);
            // 
            // tabSkills
            // 
            this.tabSkills.Controls.Add(this.lbl_Skill_Shimmier);
            this.tabSkills.Controls.Add(this.num_Ski_Shimmier);
            this.tabSkills.Controls.Add(this.lbl_Skill_Fencer);
            this.tabSkills.Controls.Add(this.lbl_Skill_Cloner);
            this.tabSkills.Controls.Add(this.lbl_Skill_Stacker);
            this.tabSkills.Controls.Add(this.lbl_Skill_Platformer);
            this.tabSkills.Controls.Add(this.lbl_Skill_Stoner);
            this.tabSkills.Controls.Add(this.lbl_Skill_Disarmer);
            this.tabSkills.Controls.Add(this.lbl_Skill_Glider);
            this.tabSkills.Controls.Add(this.lbl_Skill_Swimmer);
            this.tabSkills.Controls.Add(this.lbl_Skill_Walker);
            this.tabSkills.Controls.Add(this.lbl_Skill_Digger);
            this.tabSkills.Controls.Add(this.lbl_Skill_Miner);
            this.tabSkills.Controls.Add(this.lbl_Skill_Basher);
            this.tabSkills.Controls.Add(this.lbl_Skill_Builder);
            this.tabSkills.Controls.Add(this.lbl_Skill_Bomber);
            this.tabSkills.Controls.Add(this.lbl_Skill_Blocker);
            this.tabSkills.Controls.Add(this.lbl_Skill_Floater);
            this.tabSkills.Controls.Add(this.lbl_Skill_Climber);
            this.tabSkills.Controls.Add(this.num_Ski_Fencer);
            this.tabSkills.Controls.Add(this.num_Ski_Cloner);
            this.tabSkills.Controls.Add(this.num_Ski_Stacker);
            this.tabSkills.Controls.Add(this.num_Ski_Platformer);
            this.tabSkills.Controls.Add(this.num_Ski_Stoner);
            this.tabSkills.Controls.Add(this.num_Ski_Disarmer);
            this.tabSkills.Controls.Add(this.num_Ski_Glider);
            this.tabSkills.Controls.Add(this.num_Ski_Swimmer);
            this.tabSkills.Controls.Add(this.num_Ski_Walker);
            this.tabSkills.Controls.Add(this.num_Ski_Digger);
            this.tabSkills.Controls.Add(this.num_Ski_Miner);
            this.tabSkills.Controls.Add(this.num_Ski_Basher);
            this.tabSkills.Controls.Add(this.num_Ski_Builder);
            this.tabSkills.Controls.Add(this.num_Ski_Bomber);
            this.tabSkills.Controls.Add(this.num_Ski_Blocker);
            this.tabSkills.Controls.Add(this.num_Ski_Floater);
            this.tabSkills.Controls.Add(this.num_Ski_Climber);
            this.tabSkills.Location = new System.Drawing.Point(4, 22);
            this.tabSkills.Name = "tabSkills";
            this.tabSkills.Size = new System.Drawing.Size(174, 396);
            this.tabSkills.TabIndex = 2;
            this.tabSkills.Text = "Skills";
            this.tabSkills.UseVisualStyleBackColor = true;
            // 
            // lbl_Skill_Shimmier
            // 
            this.lbl_Skill_Shimmier.Location = new System.Drawing.Point(8, 27);
            this.lbl_Skill_Shimmier.Name = "lbl_Skill_Shimmier";
            this.lbl_Skill_Shimmier.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Shimmier.TabIndex = 34;
            this.lbl_Skill_Shimmier.Text = "Shimmier";
            // 
            // num_Ski_Shimmier
            // 
            this.num_Ski_Shimmier.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Shimmier.Location = new System.Drawing.Point(84, 25);
            this.num_Ski_Shimmier.Name = "num_Ski_Shimmier";
            this.num_Ski_Shimmier.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Shimmier.TabIndex = 35;
            this.num_Ski_Shimmier.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Shimmier.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            // 
            // lbl_Skill_Fencer
            // 
            this.lbl_Skill_Fencer.Location = new System.Drawing.Point(8, 301);
            this.lbl_Skill_Fencer.Name = "lbl_Skill_Fencer";
            this.lbl_Skill_Fencer.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Fencer.TabIndex = 33;
            this.lbl_Skill_Fencer.Text = "Fencer";
            // 
            // lbl_Skill_Cloner
            // 
            this.lbl_Skill_Cloner.Location = new System.Drawing.Point(8, 364);
            this.lbl_Skill_Cloner.Name = "lbl_Skill_Cloner";
            this.lbl_Skill_Cloner.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Cloner.TabIndex = 15;
            this.lbl_Skill_Cloner.Text = "Cloner";
            // 
            // lbl_Skill_Stacker
            // 
            this.lbl_Skill_Stacker.Location = new System.Drawing.Point(8, 259);
            this.lbl_Skill_Stacker.Name = "lbl_Skill_Stacker";
            this.lbl_Skill_Stacker.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Stacker.TabIndex = 14;
            this.lbl_Skill_Stacker.Text = "Stacker";
            // 
            // lbl_Skill_Platformer
            // 
            this.lbl_Skill_Platformer.Location = new System.Drawing.Point(8, 217);
            this.lbl_Skill_Platformer.Name = "lbl_Skill_Platformer";
            this.lbl_Skill_Platformer.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Platformer.TabIndex = 13;
            this.lbl_Skill_Platformer.Text = "Platformer";
            // 
            // lbl_Skill_Stoner
            // 
            this.lbl_Skill_Stoner.Location = new System.Drawing.Point(8, 175);
            this.lbl_Skill_Stoner.Name = "lbl_Skill_Stoner";
            this.lbl_Skill_Stoner.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Stoner.TabIndex = 12;
            this.lbl_Skill_Stoner.Text = "Stoner";
            // 
            // lbl_Skill_Disarmer
            // 
            this.lbl_Skill_Disarmer.Location = new System.Drawing.Point(8, 132);
            this.lbl_Skill_Disarmer.Name = "lbl_Skill_Disarmer";
            this.lbl_Skill_Disarmer.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Disarmer.TabIndex = 11;
            this.lbl_Skill_Disarmer.Text = "Disarmer";
            // 
            // lbl_Skill_Glider
            // 
            this.lbl_Skill_Glider.Location = new System.Drawing.Point(8, 111);
            this.lbl_Skill_Glider.Name = "lbl_Skill_Glider";
            this.lbl_Skill_Glider.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Glider.TabIndex = 10;
            this.lbl_Skill_Glider.Text = "Glider";
            // 
            // lbl_Skill_Swimmer
            // 
            this.lbl_Skill_Swimmer.Location = new System.Drawing.Point(8, 69);
            this.lbl_Skill_Swimmer.Name = "lbl_Skill_Swimmer";
            this.lbl_Skill_Swimmer.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Swimmer.TabIndex = 9;
            this.lbl_Skill_Swimmer.Text = "Swimmer";
            // 
            // lbl_Skill_Walker
            // 
            this.lbl_Skill_Walker.Location = new System.Drawing.Point(8, 6);
            this.lbl_Skill_Walker.Name = "lbl_Skill_Walker";
            this.lbl_Skill_Walker.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Walker.TabIndex = 8;
            this.lbl_Skill_Walker.Text = "Walker";
            // 
            // lbl_Skill_Digger
            // 
            this.lbl_Skill_Digger.Location = new System.Drawing.Point(8, 343);
            this.lbl_Skill_Digger.Name = "lbl_Skill_Digger";
            this.lbl_Skill_Digger.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Digger.TabIndex = 7;
            this.lbl_Skill_Digger.Text = "Digger";
            // 
            // lbl_Skill_Miner
            // 
            this.lbl_Skill_Miner.Location = new System.Drawing.Point(8, 322);
            this.lbl_Skill_Miner.Name = "lbl_Skill_Miner";
            this.lbl_Skill_Miner.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Miner.TabIndex = 6;
            this.lbl_Skill_Miner.Text = "Miner";
            // 
            // lbl_Skill_Basher
            // 
            this.lbl_Skill_Basher.Location = new System.Drawing.Point(8, 280);
            this.lbl_Skill_Basher.Name = "lbl_Skill_Basher";
            this.lbl_Skill_Basher.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Basher.TabIndex = 5;
            this.lbl_Skill_Basher.Text = "Basher";
            // 
            // lbl_Skill_Builder
            // 
            this.lbl_Skill_Builder.Location = new System.Drawing.Point(8, 238);
            this.lbl_Skill_Builder.Name = "lbl_Skill_Builder";
            this.lbl_Skill_Builder.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Builder.TabIndex = 4;
            this.lbl_Skill_Builder.Text = "Builder";
            // 
            // lbl_Skill_Bomber
            // 
            this.lbl_Skill_Bomber.Location = new System.Drawing.Point(8, 154);
            this.lbl_Skill_Bomber.Name = "lbl_Skill_Bomber";
            this.lbl_Skill_Bomber.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Bomber.TabIndex = 3;
            this.lbl_Skill_Bomber.Text = "Bomber";
            // 
            // lbl_Skill_Blocker
            // 
            this.lbl_Skill_Blocker.Location = new System.Drawing.Point(8, 196);
            this.lbl_Skill_Blocker.Name = "lbl_Skill_Blocker";
            this.lbl_Skill_Blocker.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Blocker.TabIndex = 2;
            this.lbl_Skill_Blocker.Text = "Blocker";
            // 
            // lbl_Skill_Floater
            // 
            this.lbl_Skill_Floater.Location = new System.Drawing.Point(8, 90);
            this.lbl_Skill_Floater.Name = "lbl_Skill_Floater";
            this.lbl_Skill_Floater.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Floater.TabIndex = 1;
            this.lbl_Skill_Floater.Text = "Floater";
            // 
            // lbl_Skill_Climber
            // 
            this.lbl_Skill_Climber.Location = new System.Drawing.Point(8, 48);
            this.lbl_Skill_Climber.Name = "lbl_Skill_Climber";
            this.lbl_Skill_Climber.Size = new System.Drawing.Size(70, 15);
            this.lbl_Skill_Climber.TabIndex = 0;
            this.lbl_Skill_Climber.Text = "Climber";
            // 
            // num_Ski_Fencer
            // 
            this.num_Ski_Fencer.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Fencer.Location = new System.Drawing.Point(84, 299);
            this.num_Ski_Fencer.Name = "num_Ski_Fencer";
            this.num_Ski_Fencer.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Fencer.TabIndex = 24;
            this.num_Ski_Fencer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Fencer.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Fencer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Fencer.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Cloner
            // 
            this.num_Ski_Cloner.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Cloner.Location = new System.Drawing.Point(84, 362);
            this.num_Ski_Cloner.Name = "num_Ski_Cloner";
            this.num_Ski_Cloner.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Cloner.TabIndex = 32;
            this.num_Ski_Cloner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Cloner.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Cloner.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Cloner.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Stacker
            // 
            this.num_Ski_Stacker.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Stacker.Location = new System.Drawing.Point(84, 257);
            this.num_Ski_Stacker.Name = "num_Ski_Stacker";
            this.num_Ski_Stacker.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Stacker.TabIndex = 31;
            this.num_Ski_Stacker.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Stacker.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Stacker.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Stacker.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Platformer
            // 
            this.num_Ski_Platformer.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Platformer.Location = new System.Drawing.Point(84, 215);
            this.num_Ski_Platformer.Name = "num_Ski_Platformer";
            this.num_Ski_Platformer.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Platformer.TabIndex = 30;
            this.num_Ski_Platformer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Platformer.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Platformer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Platformer.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Stoner
            // 
            this.num_Ski_Stoner.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Stoner.Location = new System.Drawing.Point(84, 173);
            this.num_Ski_Stoner.Name = "num_Ski_Stoner";
            this.num_Ski_Stoner.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Stoner.TabIndex = 29;
            this.num_Ski_Stoner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Stoner.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Stoner.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Stoner.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Disarmer
            // 
            this.num_Ski_Disarmer.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Disarmer.Location = new System.Drawing.Point(84, 130);
            this.num_Ski_Disarmer.Name = "num_Ski_Disarmer";
            this.num_Ski_Disarmer.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Disarmer.TabIndex = 28;
            this.num_Ski_Disarmer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Disarmer.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Disarmer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Disarmer.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Glider
            // 
            this.num_Ski_Glider.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Glider.Location = new System.Drawing.Point(84, 109);
            this.num_Ski_Glider.Name = "num_Ski_Glider";
            this.num_Ski_Glider.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Glider.TabIndex = 27;
            this.num_Ski_Glider.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Glider.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Glider.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Glider.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Swimmer
            // 
            this.num_Ski_Swimmer.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Swimmer.Location = new System.Drawing.Point(84, 67);
            this.num_Ski_Swimmer.Name = "num_Ski_Swimmer";
            this.num_Ski_Swimmer.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Swimmer.TabIndex = 26;
            this.num_Ski_Swimmer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Swimmer.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Swimmer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Swimmer.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Walker
            // 
            this.num_Ski_Walker.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Walker.Location = new System.Drawing.Point(84, 4);
            this.num_Ski_Walker.Name = "num_Ski_Walker";
            this.num_Ski_Walker.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Walker.TabIndex = 25;
            this.num_Ski_Walker.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Walker.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Walker.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Walker.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Digger
            // 
            this.num_Ski_Digger.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Digger.Location = new System.Drawing.Point(84, 341);
            this.num_Ski_Digger.Name = "num_Ski_Digger";
            this.num_Ski_Digger.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Digger.TabIndex = 23;
            this.num_Ski_Digger.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Digger.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Digger.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Digger.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Miner
            // 
            this.num_Ski_Miner.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Miner.Location = new System.Drawing.Point(84, 320);
            this.num_Ski_Miner.Name = "num_Ski_Miner";
            this.num_Ski_Miner.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Miner.TabIndex = 22;
            this.num_Ski_Miner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Miner.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Miner.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Miner.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Basher
            // 
            this.num_Ski_Basher.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Basher.Location = new System.Drawing.Point(84, 278);
            this.num_Ski_Basher.Name = "num_Ski_Basher";
            this.num_Ski_Basher.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Basher.TabIndex = 21;
            this.num_Ski_Basher.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Basher.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Basher.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Basher.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Builder
            // 
            this.num_Ski_Builder.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Builder.Location = new System.Drawing.Point(84, 236);
            this.num_Ski_Builder.Name = "num_Ski_Builder";
            this.num_Ski_Builder.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Builder.TabIndex = 20;
            this.num_Ski_Builder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Builder.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Builder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Builder.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Bomber
            // 
            this.num_Ski_Bomber.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Bomber.Location = new System.Drawing.Point(84, 152);
            this.num_Ski_Bomber.Name = "num_Ski_Bomber";
            this.num_Ski_Bomber.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Bomber.TabIndex = 18;
            this.num_Ski_Bomber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Bomber.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Bomber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Bomber.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Blocker
            // 
            this.num_Ski_Blocker.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Blocker.Location = new System.Drawing.Point(84, 194);
            this.num_Ski_Blocker.Name = "num_Ski_Blocker";
            this.num_Ski_Blocker.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Blocker.TabIndex = 19;
            this.num_Ski_Blocker.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Blocker.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Blocker.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Blocker.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Floater
            // 
            this.num_Ski_Floater.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Floater.Location = new System.Drawing.Point(84, 88);
            this.num_Ski_Floater.Name = "num_Ski_Floater";
            this.num_Ski_Floater.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Floater.TabIndex = 17;
            this.num_Ski_Floater.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Floater.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Floater.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Floater.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // num_Ski_Climber
            // 
            this.num_Ski_Climber.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.num_Ski_Climber.Location = new System.Drawing.Point(84, 46);
            this.num_Ski_Climber.Name = "num_Ski_Climber";
            this.num_Ski_Climber.Size = new System.Drawing.Size(52, 20);
            this.num_Ski_Climber.TabIndex = 16;
            this.num_Ski_Climber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Ski_Climber.ValueChanged += new System.EventHandler(this.num_Skill_ValueChanged);
            this.num_Ski_Climber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_Skill_KeyDown);
            this.num_Ski_Climber.Leave += new System.EventHandler(this.textbox_Leave);
            // 
            // tabMisc
            // 
            this.tabMisc.Controls.Add(this.lblTalismans);
            this.tabMisc.Controls.Add(this.lbTalismans);
            this.tabMisc.Location = new System.Drawing.Point(4, 22);
            this.tabMisc.Name = "tabMisc";
            this.tabMisc.Size = new System.Drawing.Size(174, 396);
            this.tabMisc.TabIndex = 3;
            this.tabMisc.Text = "Misc.";
            this.tabMisc.UseVisualStyleBackColor = true;
            // 
            // lblTalismans
            // 
            this.lblTalismans.AutoSize = true;
            this.lblTalismans.Location = new System.Drawing.Point(8, 9);
            this.lblTalismans.Name = "lblTalismans";
            this.lblTalismans.Size = new System.Drawing.Size(54, 13);
            this.lblTalismans.TabIndex = 1;
            this.lblTalismans.Text = "Talismans";
            // 
            // lbTalismans
            // 
            this.lbTalismans.DisplayMember = "Title";
            this.lbTalismans.FormattingEnabled = true;
            this.lbTalismans.Location = new System.Drawing.Point(8, 25);
            this.lbTalismans.Name = "lbTalismans";
            this.lbTalismans.Size = new System.Drawing.Size(151, 108);
            this.lbTalismans.TabIndex = 0;
            // 
            // txt_Focus
            // 
            this.txt_Focus.Location = new System.Drawing.Point(-100, 1);
            this.txt_Focus.Name = "txt_Focus";
            this.txt_Focus.Size = new System.Drawing.Size(40, 20);
            this.txt_Focus.TabIndex = 37;
            this.txt_Focus.TabStop = false;
            this.txt_Focus.Text = "asdf";
            // 
            // tabLvlPieces
            // 
            this.tabLvlPieces.Enabled = false;
            this.tabLvlPieces.Location = new System.Drawing.Point(415, 50);
            this.tabLvlPieces.Name = "tabLvlPieces";
            this.tabLvlPieces.SelectedIndex = 0;
            this.tabLvlPieces.Size = new System.Drawing.Size(182, 422);
            this.tabLvlPieces.TabIndex = 38;
            this.tabLvlPieces.TabStop = false;
            this.tabLvlPieces.Visible = false;
            this.tabLvlPieces.Click += new System.EventHandler(this.tabLvlProperties_Click);
            // 
            // tabLvlSkills
            // 
            this.tabLvlSkills.Enabled = false;
            this.tabLvlSkills.Location = new System.Drawing.Point(510, 31);
            this.tabLvlSkills.Name = "tabLvlSkills";
            this.tabLvlSkills.SelectedIndex = 0;
            this.tabLvlSkills.Size = new System.Drawing.Size(150, 422);
            this.tabLvlSkills.TabIndex = 39;
            this.tabLvlSkills.TabStop = false;
            this.tabLvlSkills.Visible = false;
            this.tabLvlSkills.Click += new System.EventHandler(this.tabLvlProperties_Click);
            // 
            // scrollPicLevelHoriz
            // 
            this.scrollPicLevelHoriz.LargeChange = 2;
            this.scrollPicLevelHoriz.Location = new System.Drawing.Point(188, 456);
            this.scrollPicLevelHoriz.Maximum = 1;
            this.scrollPicLevelHoriz.Name = "scrollPicLevelHoriz";
            this.scrollPicLevelHoriz.Size = new System.Drawing.Size(598, 16);
            this.scrollPicLevelHoriz.TabIndex = 40;
            this.scrollPicLevelHoriz.Visible = false;
            this.scrollPicLevelHoriz.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollPicLevelHoriz_Scroll);
            // 
            // scrollPicLevelVert
            // 
            this.scrollPicLevelVert.LargeChange = 2;
            this.scrollPicLevelVert.Location = new System.Drawing.Point(770, 27);
            this.scrollPicLevelVert.Maximum = 1;
            this.scrollPicLevelVert.Name = "scrollPicLevelVert";
            this.scrollPicLevelVert.Size = new System.Drawing.Size(16, 444);
            this.scrollPicLevelVert.TabIndex = 41;
            this.scrollPicLevelVert.Visible = false;
            this.scrollPicLevelVert.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollPicLevelVert_Scroll);
            // 
            // pic_DragNewPiece
            // 
            this.pic_DragNewPiece.BackColor = System.Drawing.Color.Black;
            this.pic_DragNewPiece.Location = new System.Drawing.Point(770, 550);
            this.pic_DragNewPiece.Name = "pic_DragNewPiece";
            this.pic_DragNewPiece.Size = new System.Drawing.Size(20, 20);
            this.pic_DragNewPiece.TabIndex = 42;
            this.pic_DragNewPiece.TabStop = false;
            this.pic_DragNewPiece.Visible = false;
            // 
            // toolTipButton
            // 
            this.toolTipButton.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTipButton_Popup);
            // 
            // but_PieceRight
            // 
            this.but_PieceRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_PieceRight.Location = new System.Drawing.Point(756, 478);
            this.but_PieceRight.Name = "but_PieceRight";
            this.but_PieceRight.NoPaddingText = null;
            this.but_PieceRight.Size = new System.Drawing.Size(32, 84);
            this.but_PieceRight.TabIndex = 29;
            this.but_PieceRight.Text = "⇨";
            this.toolTipButton.SetToolTip(this.but_PieceRight, "Right-click for faster scrolling");
            this.but_PieceRight.UseVisualStyleBackColor = true;
            this.but_PieceRight.Click += new System.EventHandler(this.but_PieceRight_Click);
            this.but_PieceRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_PieceRight_MouseUp);
            // 
            // but_PieceLeft
            // 
            this.but_PieceLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_PieceLeft.Location = new System.Drawing.Point(0, 478);
            this.but_PieceLeft.Name = "but_PieceLeft";
            this.but_PieceLeft.NoPaddingText = null;
            this.but_PieceLeft.Size = new System.Drawing.Size(32, 84);
            this.but_PieceLeft.TabIndex = 26;
            this.but_PieceLeft.Text = "⇦";
            this.toolTipButton.SetToolTip(this.but_PieceLeft, "Right-click for faster scrolling");
            this.but_PieceLeft.UseVisualStyleBackColor = true;
            this.but_PieceLeft.Click += new System.EventHandler(this.but_PieceLeft_Click);
            this.but_PieceLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.but_PieceLeft_MouseUp);
            // 
            // NLEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.pic_DragNewPiece);
            this.Controls.Add(this.scrollPicLevelVert);
            this.Controls.Add(this.scrollPicLevelHoriz);
            this.Controls.Add(this.tabLvlSkills);
            this.Controls.Add(this.tabLvlPieces);
            this.Controls.Add(this.txt_Focus);
            this.Controls.Add(this.pic_Level);
            this.Controls.Add(this.but_PieceTerrObj);
            this.Controls.Add(this.picPiece7);
            this.Controls.Add(this.picPiece6);
            this.Controls.Add(this.picPiece5);
            this.Controls.Add(this.picPiece4);
            this.Controls.Add(this.picPiece3);
            this.Controls.Add(this.but_PieceRight);
            this.Controls.Add(this.picPiece2);
            this.Controls.Add(this.picPiece1);
            this.Controls.Add(this.but_PieceLeft);
            this.Controls.Add(this.picPiece0);
            this.Controls.Add(this.combo_PieceStyle);
            this.Controls.Add(this.tabLvlProperties);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "NLEditForm";
            this.Text = "  NeoLemmix Editor";
            this.Activated += new System.EventHandler(this.NLEditForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NLEditForm_FormClosing);
            this.Click += new System.EventHandler(this.NLEditForm_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NLEditForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NLEditForm_KeyUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.NLEditForm_MouseWheel);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiece7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Level)).EndInit();
            this.tabLvlProperties.ResumeLayout(false);
            this.tabGlobalInfo.ResumeLayout(false);
            this.tabGlobalInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_TimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_TimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_Rescue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_Lems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_StartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_StartX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SizeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Lvl_SizeX)).EndInit();
            this.tabPieces.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.num_PickupSkillCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_LemmingLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Resize_Height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Resize_Width)).EndInit();
            this.tabSkills.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Shimmier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Fencer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Cloner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Stacker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Platformer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Stoner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Disarmer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Glider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Swimmer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Walker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Digger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Miner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Basher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Builder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Bomber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Blocker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Floater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Ski_Climber)).EndInit();
            this.tabMisc.ResumeLayout(false);
            this.tabMisc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_DragNewPiece)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPhysicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terrainRenderingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectRenderingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggerAreasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deprecatedPiecesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validateLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.TabPage tabGlobalInfo;
        private System.Windows.Forms.TabPage tabPieces;
        private System.Windows.Forms.TabPage tabSkills;
        private System.Windows.Forms.Label lbl_Skill_Cloner;
        private System.Windows.Forms.Label lbl_Skill_Stacker;
        private System.Windows.Forms.Label lbl_Skill_Platformer;
        private System.Windows.Forms.Label lbl_Skill_Stoner;
        private System.Windows.Forms.Label lbl_Skill_Disarmer;
        private System.Windows.Forms.Label lbl_Skill_Glider;
        private System.Windows.Forms.Label lbl_Skill_Swimmer;
        private System.Windows.Forms.Label lbl_Skill_Walker;
        private System.Windows.Forms.Label lbl_Skill_Digger;
        private System.Windows.Forms.Label lbl_Skill_Miner;
        private System.Windows.Forms.Label lbl_Skill_Basher;
        private System.Windows.Forms.Label lbl_Skill_Builder;
        private System.Windows.Forms.Label lbl_Skill_Bomber;
        private System.Windows.Forms.Label lbl_Skill_Blocker;
        private System.Windows.Forms.Label lbl_Skill_Floater;
        private System.Windows.Forms.Label lbl_Skill_Climber;
        private NLEditor.NumUpDownOverwrite num_Ski_Cloner;
        private NLEditor.NumUpDownOverwrite num_Ski_Stacker;
        private NLEditor.NumUpDownOverwrite num_Ski_Platformer;
        private NLEditor.NumUpDownOverwrite num_Ski_Stoner;
        private NLEditor.NumUpDownOverwrite num_Ski_Disarmer;
        private NLEditor.NumUpDownOverwrite num_Ski_Glider;
        private NLEditor.NumUpDownOverwrite num_Ski_Swimmer;
        private NLEditor.NumUpDownOverwrite num_Ski_Walker;
        private NLEditor.NumUpDownOverwrite num_Ski_Digger;
        private NLEditor.NumUpDownOverwrite num_Ski_Miner;
        private NLEditor.NumUpDownOverwrite num_Ski_Basher;
        private NLEditor.NumUpDownOverwrite num_Ski_Builder;
        private NLEditor.NumUpDownOverwrite num_Ski_Bomber;
        private NLEditor.NumUpDownOverwrite num_Ski_Blocker;
        private NLEditor.NumUpDownOverwrite num_Ski_Floater;
        private NLEditor.NumUpDownOverwrite num_Ski_Climber;
        private NLEditor.NumUpDownOverwrite num_Lvl_Rescue;
        private System.Windows.Forms.Label lbl_Global_Rescue;
        private NLEditor.NumUpDownOverwrite num_Lvl_Lems;
        private System.Windows.Forms.Label lbl_Global_Lemmings;
        private NLEditor.NumUpDownOverwrite num_Lvl_StartY;
        private NLEditor.NumUpDownOverwrite num_Lvl_StartX;
        private System.Windows.Forms.Label lbl_Global_StartPos;
        private NLEditor.NumUpDownOverwrite num_Lvl_SizeY;
        private NLEditor.NumUpDownOverwrite num_Lvl_SizeX;
        private System.Windows.Forms.Label lbl_Global_Size;
        private System.Windows.Forms.ComboBox combo_MainStyle;
        private System.Windows.Forms.Label lbl_Global_Style;
        private System.Windows.Forms.Label lbl_Global_Music;
        private System.Windows.Forms.TextBox txt_LevelAuthor;
        private System.Windows.Forms.Label lbl_Global_Author;
        private System.Windows.Forms.TextBox txt_LevelTitle;
        private System.Windows.Forms.Label lbl_Global_Title;
        private NLEditor.NumUpDownOverwrite num_Lvl_TimeSec;
        private NLEditor.NumUpDownOverwrite num_Lvl_TimeMin;
        private System.Windows.Forms.Label lbl_Global_TimeLimit;
        private System.Windows.Forms.CheckBox check_Lvl_LockSR;
        private NLEditor.NumUpDownOverwrite num_Lvl_SR;
        private System.Windows.Forms.Label lbl_Global_SR;
        private System.Windows.Forms.ComboBox combo_Music;
        private System.Windows.Forms.ComboBox combo_PieceStyle;
        private System.Windows.Forms.PictureBox picPiece0;
        private NLEditor.RepeatButton but_PieceLeft;
        private System.Windows.Forms.PictureBox picPiece1;
        private System.Windows.Forms.PictureBox picPiece2;
        private NLEditor.RepeatButton but_PieceRight;
        private System.Windows.Forms.PictureBox picPiece3;
        private System.Windows.Forms.PictureBox picPiece4;
        private System.Windows.Forms.PictureBox picPiece5;
        private System.Windows.Forms.PictureBox picPiece6;
        private System.Windows.Forms.PictureBox picPiece7;
        private System.Windows.Forms.Button but_PieceTerrObj;
        private System.Windows.Forms.CheckBox check_Lvl_InfTime;
        private System.Windows.Forms.PictureBox pic_Level;
        private System.Windows.Forms.TabControl tabLvlProperties;
        private NLEditor.FocusTextBox txt_Focus;
        private NLEditor.RepeatButton but_FlipPieces;
        private NLEditor.RepeatButton but_InvertPieces;
        private NLEditor.RepeatButton but_RotatePieces;
        private System.Windows.Forms.CheckBox check_Pieces_OneWay;
        private System.Windows.Forms.CheckBox check_Pieces_OnlyOnTerrain;
        private System.Windows.Forms.CheckBox check_Pieces_NoOv;
        private System.Windows.Forms.CheckBox check_Pieces_Erase;
        private System.Windows.Forms.CheckBox check_Piece_Cloner;
        private System.Windows.Forms.CheckBox check_Piece_Stacker;
        private System.Windows.Forms.CheckBox check_Piece_Platformer;
        private System.Windows.Forms.CheckBox check_Piece_Stoner;
        private System.Windows.Forms.CheckBox check_Piece_Disarmer;
        private System.Windows.Forms.CheckBox check_Piece_Glider;
        private System.Windows.Forms.CheckBox check_Piece_Swimmer;
        private System.Windows.Forms.CheckBox check_Piece_Walker;
        private System.Windows.Forms.CheckBox check_Piece_Digger;
        private System.Windows.Forms.CheckBox check_Piece_Miner;
        private System.Windows.Forms.CheckBox check_Piece_Basher;
        private System.Windows.Forms.CheckBox check_Piece_Builder;
        private System.Windows.Forms.CheckBox check_Piece_Bomber;
        private System.Windows.Forms.CheckBox check_Piece_Blocker;
        private System.Windows.Forms.CheckBox check_Piece_Floater;
        private System.Windows.Forms.CheckBox check_Piece_Climber;
        private NLEditor.RepeatButton but_MoveBackOne;
        private NLEditor.RepeatButton but_MoveFrontOne;
        private NLEditor.NoPaddingButton but_MoveBack;
        private NLEditor.NoPaddingButton but_MoveFront;
        private System.Windows.Forms.CheckBox check_Piece_Zombie;
        private System.Windows.Forms.CheckBox check_Piece_Neutral;
        private System.Windows.Forms.ToolStripMenuItem hotkeysToolStripMenuItem;
        private System.Windows.Forms.Label lbl_Resize_Height;
        private NLEditor.NumUpDownOverwrite num_Resize_Height;
        private NLEditor.NumUpDownOverwrite num_Resize_Width;
        private System.Windows.Forms.Label lbl_Resize_Width;
        private System.Windows.Forms.Button but_PairTeleporter;
        private System.Windows.Forms.ComboBox combo_Background;
        private System.Windows.Forms.Label lbl_Global_Background;
        private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTipPieces;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TabControl tabLvlPieces;
        private System.Windows.Forms.TabControl tabLvlSkills;
        private System.Windows.Forms.HScrollBar scrollPicLevelHoriz;
        private System.Windows.Forms.VScrollBar scrollPicLevelVert;
        private System.Windows.Forms.CheckBox check_Piece_Fencer;
        private System.Windows.Forms.Label lbl_Skill_Fencer;
        private NLEditor.NumUpDownOverwrite num_Ski_Fencer;
        private System.Windows.Forms.Button but_UngroupSelection;
        private System.Windows.Forms.Button but_GroupSelection;
        private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ungroupToolStripMenuItem;
        private System.Windows.Forms.PictureBox pic_DragNewPiece;
        private System.Windows.Forms.ToolTip toolTipButton;
        private System.Windows.Forms.ToolStripMenuItem duplicateCToolStripMenuItem;
        private NumUpDownOverwrite num_PickupSkillCount;
        private System.Windows.Forms.Label lbl_PickupSkillCount;
        private NumUpDownOverwrite num_LemmingLimit;
        private System.Windows.Forms.Label lbl_LemmingLimit;
        private System.Windows.Forms.CheckBox check_Piece_Shimmier;
        private System.Windows.Forms.Label lbl_Skill_Shimmier;
        private NumUpDownOverwrite num_Ski_Shimmier;
        private System.Windows.Forms.TabPage tabMisc;
        private System.Windows.Forms.ListBox lbTalismans;
        private System.Windows.Forms.Label lblTalismans;
    }
}

