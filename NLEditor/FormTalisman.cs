using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NLEditor
{
    partial class FormTalisman : Form
    {
        public FormTalisman(Level level)
        {
            InitializeComponent();
            isNewTalisman = true;
            curLevel = level;
            talisman = new Talisman();

            SetFormTexts(level);
        }

        public FormTalisman(Level level, Talisman oldtalisman)
        {
            InitializeComponent();
            isNewTalisman = false;
            curLevel = level;
            oldTalisman = oldtalisman;
            talisman = new Talisman();
            talisman.AwardType = oldTalisman.AwardType;
            talisman.Title = (string)oldTalisman.Title.Clone();
            talisman.Requirements = new Dictionary<C.TalismanReq, int>(oldTalisman.Requirements);

            SetFormTexts(level);
        }

        private void SetFormTexts(Level level)
        {
            lblTalismanTitle.Text = "Talisman for '" + level.Title + "'";

            txtTalismanTitle.Text = talisman.Title;
            radBronze.Checked = (talisman.AwardType == C.TalismanType.Bronze);
            radSilver.Checked = (talisman.AwardType == C.TalismanType.Silver);
            radGold.Checked = (talisman.AwardType == C.TalismanType.Gold);

            WriteRequirementList();

            foreach (C.TalismanReq requirement in C.TalismanReqArray)
            {
                cmbRequirementTypes.Items.Add(C.TalismanReqText[requirement]);
            }
            cmbRequirementTypes.Text = cmbRequirementTypes.Items[0].ToString();

            foreach (string skill in C.TalismanSkills)
            {
                cmbRequirementSkill.Items.Add(skill);
            }
        }

        bool isNewTalisman;
        Level curLevel;
        Talisman talisman;
        Talisman oldTalisman;
        bool isSaved = false;

        /// <summary>
        /// Writes the list of requirements
        /// </summary>
        private void WriteRequirementList()
        {
            listRequirements.Items.Clear();
            if (talisman.Requirements.Count == 0)
            {
                listRequirements.Items.Add("No requirements...");
            }
            else
            {
                foreach (C.TalismanReq requirement in talisman.Requirements.Keys)
                {
                    string text = talisman.GetRequirementText(requirement);
                    listRequirements.Items.Add(text);
                }
            }
        }

        /// <summary>
        /// Adds a new requirement to the talisman.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butRequirementAdd_Click(object sender, EventArgs e)
        {
            string reqText = cmbRequirementTypes.Text;
            if (string.IsNullOrWhiteSpace(reqText))
                return;

            // Add new requirement to talisman
            C.TalismanReq newReq = C.TalismanReqText.First(pair => pair.Value.Equals(reqText)).Key;
            int value = 0;
            if (numReqValue2.Visible)
            {
                value = (int)((numReqValue1.Value * 60 + numReqValue2.Value) * 17);
            }
            else if (numReqValue1.Visible)
            {
                value = (int)numReqValue1.Value;
            }
            else if (cmbRequirementSkill.Visible)
            {
                value = cmbRequirementSkill.SelectedIndex;
            }
            talisman.Requirements[newReq] = value;

            WriteRequirementList();
        }

        /// <summary>
        /// Deltes the selected requirements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butRequirementDelete_Click(object sender, EventArgs e)
        {
            foreach (var listItem in listRequirements.SelectedItems)
            {
                var requirement = talisman.Requirements.First(pair => talisman.GetRequirementText(pair.Key).Equals(listItem.ToString()));
                talisman.Requirements.Remove(requirement.Key);
            }

            WriteRequirementList();
        }

        /// <summary>
        /// Saves the talisman.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butTalismanSave_Click(object sender, EventArgs e)
        {
            SaveTalisman();
        }

        /// <summary>
        /// // Exit talisman creation form without saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butTalismanCancel_Click(object sender, EventArgs e)
        {
            isSaved = true;
            Close();
        }

        /// <summary>
        /// Saves the talisman.
        /// </summary>
        private void SaveTalisman()
        {
            talisman.Title = txtTalismanTitle.Text;
            if (radBronze.Checked)
                talisman.AwardType = C.TalismanType.Bronze;
            else if (radSilver.Checked)
                talisman.AwardType = C.TalismanType.Silver;
            else if (radGold.Checked)
                talisman.AwardType = C.TalismanType.Gold;

            if (isNewTalisman)
            {
                curLevel.Talismans.Add(talisman);
            }
            else
            {
                curLevel.Talismans.Remove(oldTalisman);
                curLevel.Talismans.Add(talisman);
            }

            // Exit talisman creation form
            isSaved = true;
            Close();
        }

        /// <summary>
        /// Update other controls, depending on the selected requirement type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRequirementTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!C.TalismanReqText.ContainsValue(cmbRequirementTypes.Text))
                return;

            C.TalismanReq requirement = C.TalismanReqText.First(pair => pair.Value.Equals(cmbRequirementTypes.Text)).Key;

            // Set visibility
            numReqValue1.Visible = (requirement != C.TalismanReq.UseOnlySkill);
            numReqValue2.Visible = (requirement == C.TalismanReq.TimeLimit);
            cmbRequirementSkill.Visible = (requirement == C.TalismanReq.UseOnlySkill);

            // Set maximums
            switch (requirement)
            {
                case C.TalismanReq.SaveReq:
                    numReqValue1.Maximum = 200;
                    break;
                case C.TalismanReq.TimeLimit:
                    {
                        numReqValue1.Maximum = 99;
                        numReqValue2.Maximum = 59;
                        break;
                    }
                case C.TalismanReq.SkillTotal:
                    numReqValue1.Maximum = 200;
                    break;
                default:
                    numReqValue1.Maximum = 99;
                    break;
            }

            // Set initial values, possible according to existing requirement
            if (talisman.Requirements.ContainsKey(requirement))
            {
                if (requirement == C.TalismanReq.TimeLimit)
                {
                    numReqValue1.Value = talisman.Requirements[requirement] / 17 / 60;
                    numReqValue2.Value = (talisman.Requirements[requirement] / 17) % 60;
                }
                else if (requirement == C.TalismanReq.UseOnlySkill)
                {
                    cmbRequirementSkill.SelectedIndex = talisman.Requirements[requirement];
                }
                else
                {
                    numReqValue1.Value = talisman.Requirements[requirement];
                }
            }
            else
            {
                numReqValue1.Value = 0;
                numReqValue2.Value = 0;
            }
        }

        private void FormTalisman_Leave(object sender, EventArgs e)
        {
            Close();
        }

        private void FormTalisman_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                // Ask user whether to save the talisman
                if (MessageBox.Show("Do you want to save the talisman?", "Save talisman?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveTalisman();
                }
            }

            //(this.Owner as FormMain).WriteTalismanInfo();
        }
    }
}
