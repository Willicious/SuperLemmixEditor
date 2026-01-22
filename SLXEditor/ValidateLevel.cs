using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
{
    /// <summary>
    /// This class can check whether a level has playability issues and display these on a separate form.
    /// </summary>
    class LevelValidator
    {
        /*---------------------------------------------------------
         *  This class controls whether all level data make sense
         *  It presents the result in a new form 
         *    and can fix all issues.
         * -------------------------------------------------------- */
        /// <summary>
        /// Initializes a new instance of a LevelValidator specific to the current level.
        /// </summary>
        /// <param name="level"></param>
        public LevelValidator(Level level)
        {
            System.Diagnostics.Debug.Assert(level != null, "Level passed to LevelValidator is null.");
            this.level = level;
        }

        Level level;
        List<string> issuesList;

        Form validatorForm;
        TextBox txtIssuesList;
        Button butEditLevel;
        Button butDeleteOutsidePieces;
        Button butDeleteDeprecatedPieces;
        Button butClose;

        public static bool validationPassed = true;
        public static bool isCleansing = false;

        /// <summary>
        /// Finds all issues in a level, creates a new form and displays the issues there.
        /// </summary>
        public void Validate(bool reuseValidatorForm, bool openedViaSave = false, bool cleansingLevels = false)
        {
            isCleansing = cleansingLevels;
            FindIssues();

            if (openedViaSave)
            {
                if (issuesList.Count == 0)
                {
                    validationPassed = true;
                    return;
                }
                else
                    validationPassed = false;
            }

            if (!reuseValidatorForm)
                CreateValidatorForm(openedViaSave);
            
            DisplayValidationResult();  
        }

        /// <summary>
        /// Returns a list of descriptions for all issues found in the level.
        /// </summary>
        private void FindIssues()
        {
            issuesList = new List<string>();

            FindIssuesPieceOutsideBoundary();
            FindIssuesTooFewLemmings();
            FindIssuesTimeLimit();
            FindIssuesTooManySkills();
            FindIssuesMissingObjects();
            FindIssuesDeprecation();
        }

        /// <summary>
        /// Returns a list of descriptions for pieces outside the level boundaries.
        /// </summary>
        private void FindIssuesPieceOutsideBoundary()
        {
            foreach (LevelPiece piece in GetPiecesOutsideBoundary())
            {
                issuesList.Add($"Piece outside level boundary: {piece.Name} in style {piece.Style} (Position {piece.PosX}, {piece.PosY})");
            }
        }


        /// <summary>
        /// Gets a list of all pieces completely outside the level area.
        /// </summary>
        private List<LevelPiece> GetPiecesOutsideBoundary()
        {
            System.Drawing.Rectangle levelRect = new System.Drawing.Rectangle(0, 0, level.Width, level.Height);
            var outsidePieceList = new List<LevelPiece>();
            outsidePieceList.AddRange(level.TerrainList.FindAll(ter => !ter.ImageRectangle.IntersectsWith(levelRect)));
            outsidePieceList.AddRange(level.GadgetList.FindAll(gad => !gad.ImageRectangle.IntersectsWith(levelRect)));
            return outsidePieceList;
        }

        /// <summary>
        /// Returns a list of descriptions of issues regarding having too few lemmings available.
        /// </summary>
        private void FindIssuesTooFewLemmings()
        {
            int numPreplacedAll = level.GadgetList.Count(gad => gad.ObjType == C.OBJ.LEMMING);
            int numPreplacedZombie = level.GadgetList.Count(gad => gad.ObjType == C.OBJ.LEMMING && gad.IsZombie);

            // Check whether at least one living lemming exists
            if (level.NumLems <= NumZombies())
            {
                issuesList.Add("Only zombie lemmings available in the level.");
            }

            // Check whether number of lemmings is at least the number of preplaced lemmings.
            if (level.NumLems < numPreplacedAll)
            {
                issuesList.Add("More preplaced lemmings set than the total number of lemmings.");
            }

            // Check whether there are enough lemmings for the save requirement.
            int maxNumSaved = MaxNumSavedLems();
            if (level.SaveReq > maxNumSaved)
            {
                issuesList.Add("Save requirement too high: At most " + maxNumSaved.ToString() + " lemmings can be saved.");
            }
        }

        /// <summary>
        /// Returns the maximal number of lemmings that can be saved theoretically.
        /// </summary>
        private int MaxNumSavedLems()
        {
            return level.NumLems + level.SkillSet[C.Skill.Cloner] - NumZombies();
        }

        /// <summary>
        /// Returns the total number of zombie lemmings in the level.
        /// </summary>
        private int NumZombies()
        {
            int numPreplacedAll = level.GadgetList.Count(gad => gad.ObjType == C.OBJ.LEMMING);
            int numPreplacedZombie = level.GadgetList.Count(gad => gad.ObjType == C.OBJ.LEMMING && gad.IsZombie);
            int numToSpawn = level.NumLems - numPreplacedAll;
            List<bool> isHatchZombieList = level.GadgetList.FindAll(gad => gad.ObjType == C.OBJ.HATCH)
                                                           .ConvertAll(gad => gad.IsZombie);
            int numHatches = Math.Max(isHatchZombieList.Count, 1);
            int numZombieHatch = isHatchZombieList.Count(IsZombie => IsZombie);

            int numZombie = numPreplacedZombie;
            // add number of lemmings that spawn in zombie hatches
            numZombie += numToSpawn / numHatches * numZombieHatch;
            for (int i = 0; i < numToSpawn % numHatches; i++)
            {
                if (isHatchZombieList[i])
                    numZombie++;
            }

            return numZombie;
        }


        /// <summary>
        /// Returns an issue description if the time limit is less than 10 seconds. 
        /// </summary>
        private void FindIssuesTimeLimit()
        {
            if (level.HasTimeLimit && level.TimeLimit < 1)
                issuesList.Add("Time limit must be at least 1 second or set to infinite. " + level.TimeLimit.ToString() + " seconds available.");
        }

        /// <summary>
        /// Returns an issue description if too many different skills are used.
        /// </summary>
        private void FindIssuesTooManySkills()
        {
            int numSkillsUsed = 0;

            foreach (C.Skill skill in C.SkillArray)
            {
                if (level.SkillSet[skill] > 0 ||
                    level.GadgetList.Exists(obj => obj.ObjType == C.OBJ.PICKUP && obj.SkillFlags.Contains(skill)))
                {
                    numSkillsUsed++;
                }
            }

            int editorModeLimit = SLXEditForm.isNeoLemmixOnly ? 10 : 14;

            if (numSkillsUsed > editorModeLimit)
            {
                issuesList.Add(numSkillsUsed.ToString() + $" skill types used. Only {editorModeLimit} allowed.");
            }
        }

        /// <summary>
        /// Returns a list of descriptions of issues regarding missing object types.
        /// </summary>
        private void FindIssuesMissingObjects()
        {
            if (!level.GadgetList.Exists(obj => obj.ObjType == C.OBJ.HATCH && !obj.IsZombie))
            {
                int NumPreplacedLems = level.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING);
                if (level.NumLems > NumPreplacedLems)
                {
                    String preplacedLemWarning = (NumPreplacedLems > 0)
                        ? " (Lemming count is higher than the number of pre-placed lemmings)."
                        : "";
                    issuesList.Add("Missing object: Hatch." + preplacedLemWarning);
                }
            }

            if (!level.GadgetList.Exists(obj => obj.ObjType.In(C.OBJ.EXIT, C.OBJ.EXIT_LOCKED)))
            {
                issuesList.Add("Missing object: Exit.");
            }

            foreach (GadgetPiece pickup in level.GadgetList.FindAll(gad => gad.ObjType == C.OBJ.PICKUP && gad.SkillFlags.Count == 0))
            {
                issuesList.Add("Pickup skill without selected skill " +
                               "(Position " + pickup.PosX.ToString() +
                               ", " + pickup.PosY.ToString() + ").");
            }

        }

        private void FindIssuesDeprecation()
        {
            foreach (GadgetPiece deprecated in level.GadgetList.FindAll(gad => ImageLibrary.GetDeprecated(gad.Key)))
            {
                issuesList.Add($"Deprecated gadget: {deprecated.Name} in style {deprecated.Style} (Position {deprecated.PosX}, {deprecated.PosY})");
            }

            foreach (TerrainPiece deprecated in level.TerrainList.FindAll(gad => ImageLibrary.GetDeprecated(gad.Key)))
            {
                issuesList.Add($"Deprecated terrain piece: {deprecated.Name} in style {deprecated.Style} (Position {deprecated.PosX}, {deprecated.PosY})");
            }
        }


        /// <summary>
        /// Creates a new form to display the validation result.
        /// </summary>
        private void CreateValidatorForm(bool openedViaSave)
        {
            validatorForm = new Form();
            validatorForm.Width = 500;
            validatorForm.Height = 300;
            validatorForm.StartPosition = FormStartPosition.CenterScreen;
            validatorForm.MaximizeBox = false;
            validatorForm.ShowInTaskbar = false;
            validatorForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            validatorForm.Text = "SLXEditor - Level validation";
            validatorForm.FormClosing += new FormClosingEventHandler(ValidatorForm_FormClosing);

            txtIssuesList = new TextBox();
            txtIssuesList.Top = 6;
            txtIssuesList.Left = 6;
            txtIssuesList.Width = 482;
            txtIssuesList.Height = 200;
            txtIssuesList.Multiline = true;
            txtIssuesList.ScrollBars = ScrollBars.Vertical;
            txtIssuesList.Text = "";
            txtIssuesList.ReadOnly = true;
            txtIssuesList.TabStop = false;
            validatorForm.Controls.Add(txtIssuesList);

            butEditLevel = new Button();
            butEditLevel.Top = 212;
            butEditLevel.Left = 6;
            butEditLevel.Width = 220;
            butEditLevel.Height = 40;
            butEditLevel.Text = "Edit Level";
            butEditLevel.Visible = false;
            butEditLevel.Click += new EventHandler(butEditLevel_Click);
            validatorForm.Controls.Add(butEditLevel);

            butDeleteOutsidePieces = new Button();
            butDeleteOutsidePieces.Top = 212;
            butDeleteOutsidePieces.Left = 6;
            butDeleteOutsidePieces.Width = 220;
            butDeleteOutsidePieces.Height = 40;
            butDeleteOutsidePieces.Text = "Delete Pieces Outside Level";
            butDeleteOutsidePieces.Visible = false;
            butDeleteOutsidePieces.Click += new EventHandler(butDeleteOutsidePieces_Click);
            validatorForm.Controls.Add(butDeleteOutsidePieces);

            butDeleteDeprecatedPieces = new Button();
            butDeleteDeprecatedPieces.Top = 212;
            butDeleteDeprecatedPieces.Left = 6;
            butDeleteDeprecatedPieces.Width = 220;
            butDeleteDeprecatedPieces.Height = 40;
            butDeleteDeprecatedPieces.Text = "Delete Deprecated Pieces";
            butDeleteDeprecatedPieces.Visible = false;
            butDeleteDeprecatedPieces.Click += new EventHandler(butDeleteDeprecatedPieces_Click);
            validatorForm.Controls.Add(butDeleteDeprecatedPieces);

            String butCloseCaption = openedViaSave ? "Save Anyway" : "Close";
            butClose = new Button();
            butClose.Top = 212;
            butClose.Left = butEditLevel.Left + butEditLevel.Width + 20;
            butClose.Width = 220;
            butClose.Height = 40;
            butClose.Text = butCloseCaption;
            butClose.Click += new EventHandler(butClose_Click);
            validatorForm.Controls.Add(butClose);

            DisplayValidationResult();
            validatorForm.ShowDialog();
        }

        /// <summary>
        /// Disposes form controls on closing the form.
        /// </summary>
        private void ValidatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtIssuesList.Dispose();
            butEditLevel.Dispose();
            butDeleteOutsidePieces.Dispose();
            butDeleteDeprecatedPieces.Dispose();
            butClose.Dispose();
        }

        /// <summary>
        /// Displays the results of the level validation on the ValidatorForm.
        /// </summary>
        private void DisplayValidationResult()
        {
            txtIssuesList.Text = "";
            butEditLevel.Visible = false;
            butDeleteOutsidePieces.Visible = false;
            butDeleteDeprecatedPieces.Visible = false;

            if (issuesList == null || issuesList.Count == 0)
            {
                txtIssuesList.Text = "No issues found.";
            }
            else
            {
                txtIssuesList.Text = string.Join(C.NewLine, issuesList);

                if (issuesList[0].StartsWith("Piece outside"))
                    butDeleteOutsidePieces.Visible = true;
                else if (issuesList[0].StartsWith("Deprecated"))
                    butDeleteDeprecatedPieces.Visible = true;
                else if (!isCleansing)
                    butEditLevel.Visible = true;
            }
        }

        private void DeletePiecesOutsideBoundary()
        {
            System.Drawing.Rectangle levelRect = new System.Drawing.Rectangle(0, 0, level.Width, level.Height);
            level.TerrainList.RemoveAll(ter => !ter.ImageRectangle.IntersectsWith(levelRect));
            level.GadgetList.RemoveAll(obj => !obj.ImageRectangle.IntersectsWith(levelRect));
        }

        private void DeleteDeprecatedPieces()
        {
            level.TerrainList.RemoveAll(ter => ImageLibrary.GetDeprecated(ter.Key));
            level.GadgetList.RemoveAll(obj => ImageLibrary.GetDeprecated(obj.Key));
        }

        /// <summary>
        /// Ends validation and returns the user to the main form.
        /// </summary>
        private void butEditLevel_Click(object sender, EventArgs e)
        {
            validationPassed = false;
            validatorForm.Close();
        }

        /// <summary>
        /// Deletes pieces outside level and runs the validator again.
        /// </summary>
        private void butDeleteOutsidePieces_Click(object sender, EventArgs e)
        {
            DeletePiecesOutsideBoundary();

            Validate(true, false, isCleansing);

            if (issuesList.Count <= 0)
            {
                validationPassed = true;
                validatorForm.Close();
            }
        }

        /// <summary>
        /// Deletes deprecated pieces and runs the validator again.
        /// </summary>
        private void butDeleteDeprecatedPieces_Click(object sender, EventArgs e)
        {
            DeleteDeprecatedPieces();

            Validate(true, false, isCleansing);

            if (issuesList.Count <= 0)
            {
                validationPassed = true;
                validatorForm.Close();
            }
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            validationPassed = true; // user chose to save anyway, treat as validation passed
            validatorForm.Close();
        }
    }
}
