using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
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
        /// <param name="CurLevel"></param>
        public LevelValidator(Level CurLevel)
        {
            System.Diagnostics.Debug.Assert(CurLevel != null, "Level passed to LevelValidator is null.");
            fCurLevel = CurLevel;
        }

        Level fCurLevel;

        Form ValidatorForm;
        TextBox txtIssuesList;
        Button butResolveIssues;

        /// <summary>
        /// Finds all issues in a level, creates a new form and displays the issues there.
        /// </summary>
        /// <param name="CurLevel"></param>
        public void Validate(bool ReuseValidatorForm = false)
        {
            List<string> IssuesList = FindIssues();
            if (!ReuseValidatorForm) CreateValidatorForm();
            DisplayValidationResult(IssuesList);
        }

        /// <summary>
        /// Returns a list of descriptions for all issues found in the level.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssues()
        {
            List<string> IssuesList = new List<string>();

            IssuesList.AddRange(FindIssuesPieceOutsideBoundary());
            IssuesList.AddRange(FindIssuesTooFewLemmings());
            IssuesList.AddRange(FindIssuesTimeLimit());
            IssuesList.AddRange(FindIssuesTooManySkills());
            IssuesList.AddRange(FindIssuesMissingObjects());

            return IssuesList;
        }

        /// <summary>
        /// Returns a list of descriptions for pieces outside the level boundaries.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssuesPieceOutsideBoundary()
        {
            List<string> IssuesList = new List<string>();

            List<LevelPiece> OutsidePieceList = GetPiecesOutsideBoundary();
            foreach (LevelPiece Piece in OutsidePieceList)
            {
                IssuesList.Add("Piece outside level boundary: " + Piece.Name +
                               " in style " + Piece.Style +
                               "(Position " + Piece.PosX.ToString() +
                               ", " + Piece.PosY.ToString() + ")");
            }

            return IssuesList;
        }


        /// <summary>
        /// Gets a list of all pieces completely outside the level area.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<LevelPiece> GetPiecesOutsideBoundary()
        {
            System.Drawing.Rectangle LevelRect = new System.Drawing.Rectangle(0, 0, fCurLevel.Width, fCurLevel.Height);
            List<LevelPiece> OutsidePieceList = new List<LevelPiece>();
            OutsidePieceList.AddRange(fCurLevel.TerrainList.FindAll(ter => !ter.ImageRectangle.IntersectsWith(LevelRect)));
            OutsidePieceList.AddRange(fCurLevel.GadgetList.FindAll(obj => !obj.ImageRectangle.IntersectsWith(LevelRect)));
            return OutsidePieceList;
        }

        /// <summary>
        /// Returns a list of descriptions of issues regarding having too few lemmings available.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssuesTooFewLemmings()
        {
            List<string> IssuesList = new List<string>();
            
            int NumPreplacedAll = fCurLevel.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING);
            int NumPreplacedZombie = fCurLevel.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING && obj.IsZombie);
            
            // Check whether at least one living lemming exists
            if (fCurLevel.NumLems <= NumZombies())
            {
                IssuesList.Add("Only zombie lemmings available in the level.");
            }
            
            // Check whether number of lemmings is at least the number of preplaced lemmings.
            if (fCurLevel.NumLems < NumPreplacedAll)
            {
                IssuesList.Add("More preplaced lemmings set than the total number of lemmings.");
            }

            // Check whether there are enough lemmings for the save requirement.
            int MaxNumSaved = MaxNumSavedLems();
            if (fCurLevel.SaveReq > MaxNumSaved )
            {
                IssuesList.Add("Save requirement too high: Maximally " + MaxNumSaved.ToString() + " lemmings can be saved.");
            }

            return IssuesList;
        }

        /// <summary>
        /// Returns the maximal number of lemmings that can be saved theoretically.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private int MaxNumSavedLems()
        {
            return fCurLevel.NumLems + fCurLevel.SkillSet[C.Skill.Cloner] - NumZombies();
        }

        /// <summary>
        /// Returns the total number of zombie lemmings in the level.
        /// </summary>
        /// <returns></returns>
        private int NumZombies()
        {
            int NumPreplacedAll = fCurLevel.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING);
            int NumPreplacedZombie = fCurLevel.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING && obj.IsZombie);
            int NumToSpawn = fCurLevel.NumLems - NumPreplacedAll;
            List<bool> IsHatchZombieList = fCurLevel.GadgetList.FindAll(obj => obj.ObjType == C.OBJ.HATCH)
                                                               .ConvertAll(obj => obj.IsZombie);
            int NumHatches = Math.Max(IsHatchZombieList.Count, 1);
            int NumZombieHatch = IsHatchZombieList.Count(IsZombie => IsZombie);

            int NumZombie = NumPreplacedZombie;
            // add number of lemmings that spawn in zombie hatches
            NumZombie += NumToSpawn / NumHatches * NumZombieHatch;
            for (int i = 0; i < NumToSpawn % NumHatches; i++)
            {
                if (IsHatchZombieList[i]) NumZombie++;
            }

            return NumZombie;
        }


        /// <summary>
        /// Returns an issue description if the time limit is less than 10 seconds. 
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssuesTimeLimit()
        {
            List<string> IssuesList = new List<string>();

            if (!fCurLevel.IsNoTimeLimit && fCurLevel.TimeLimit < 10)
            {
                IssuesList.Add("Not enough time: Only " + fCurLevel.TimeLimit.ToString() + "seconds available.");
            }
           
            return IssuesList;
        }

        /// <summary>
        /// Returns an issue description if too many different skills are used.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssuesTooManySkills()
        {
            List<string> IssuesList = new List<string>();
            int NumSkillsUsed = 0;

            foreach (C.Skill skill in C.SkillArray)
            {
                if (fCurLevel.SkillSet[skill] > 0 ||
                    fCurLevel.GadgetList.Exists(obj => obj.ObjType == C.OBJ.PICKUP && obj.SkillFlags.Contains(skill)))
                {
                    NumSkillsUsed++;
                }
            }

            if (NumSkillsUsed > 8)
            {
                IssuesList.Add(NumSkillsUsed.ToString() + " skill types used. Only 8 allowed.");
            }

            return IssuesList;
        }

        /// <summary>
        /// Returns a list of descriptions of issues regarding missing object types.
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <returns></returns>
        private List<string> FindIssuesMissingObjects()
        {
            List<string> IssuesList = new List<string>();

            if (!fCurLevel.GadgetList.Exists(obj => obj.ObjType == C.OBJ.HATCH && !obj.IsZombie))
            {
                int NumPreplacedLems = fCurLevel.GadgetList.Count(obj => obj.ObjType == C.OBJ.LEMMING);
                if (fCurLevel.NumLems > NumPreplacedLems)
                {
                    IssuesList.Add("Missing object: Hatch.");
                }
            }

            if (!fCurLevel.GadgetList.Exists(obj => obj.ObjType.In(C.OBJ.EXIT, C.OBJ.EXIT_LOCKED)))
            {
                IssuesList.Add("Missing object: Exit.");
            }

            return IssuesList;
        }


        /// <summary>
        /// Creates a new form to display the validation result.
        /// </summary>
        private void CreateValidatorForm()
        {
            ValidatorForm = new Form();
            ValidatorForm.Width = 500;
            ValidatorForm.Height = 300;
            ValidatorForm.MaximizeBox = false;
            ValidatorForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ValidatorForm.Text = "NLEditor - Level validation";
            ValidatorForm.FormClosing += new FormClosingEventHandler(ValidatorForm_FormClosing);

            txtIssuesList = new TextBox();
            txtIssuesList.Top = 6;
            txtIssuesList.Left = 6;
            txtIssuesList.Width = 482;
            txtIssuesList.Height = 200;
            txtIssuesList.Multiline = true;
            txtIssuesList.ScrollBars = ScrollBars.Vertical;
            txtIssuesList.Text = "";
            txtIssuesList.ReadOnly = true;
            ValidatorForm.Controls.Add(txtIssuesList);

            butResolveIssues = new Button();
            butResolveIssues.Top = 212;
            butResolveIssues.Left = 6;
            butResolveIssues.Width = 482;
            butResolveIssues.Height = 60;
            butResolveIssues.Text = "Delete pieces outside level";
            butResolveIssues.Click += new EventHandler(butResolveIssues_Click);
            ValidatorForm.Controls.Add(butResolveIssues);

            ValidatorForm.Show();
        }

        /// <summary>
        /// Disposes form controls on closing the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtIssuesList.Dispose();
            butResolveIssues.Dispose();
        }


        /// <summary>
        /// Displays the results of the level validation on the ValidatorForm.
        /// </summary>
        /// <param name="IssuesList"></param>
        private void DisplayValidationResult(List<string> IssuesList)
        {
            txtIssuesList.Text = "";
            
            if (IssuesList == null || IssuesList.Count == 0)
            {
                txtIssuesList.Text = "No issues found.";
                butResolveIssues.Enabled = false;
            }
            else
            {
                foreach (string IssueText in IssuesList)
                {
                    txtIssuesList.Text += IssueText + System.Environment.NewLine;
                }

                butResolveIssues.Enabled = IssuesList[0].StartsWith("Piece outside");
            }

        }

        /// <summary>
        /// Removes all pieces outside of the level and validates the level again afterwards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butResolveIssues_Click(object sender, EventArgs e)
        {
            RemovePiecesOutsideBoundary();
            this.Validate(true);
        }


        /// <summary>
        /// Removes all pieces that do not intersect the level boundaries.
        /// </summary>
        private void RemovePiecesOutsideBoundary()
        {
            System.Drawing.Rectangle LevelRect = new System.Drawing.Rectangle(0, 0, fCurLevel.Width, fCurLevel.Height);
            fCurLevel.TerrainList.RemoveAll(ter => !ter.ImageRectangle.IntersectsWith(LevelRect));
            fCurLevel.GadgetList.RemoveAll(obj => !obj.ImageRectangle.IntersectsWith(LevelRect));
        }

    }
}
