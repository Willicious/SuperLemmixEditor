using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
{
    partial class FormTalisman : Form
    {
        public FormTalisman(Level level)
        {
            KeyPreview = true;

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
            talisman.SkillMinimum = new Dictionary<C.Skill, int>(oldTalisman.SkillMinimum);
            talisman.SkillMaximum = new Dictionary<C.Skill, int>(oldTalisman.SkillMaximum);

            SetFormTexts(level);
        }

        class TalismanRequirementOption
        {
            public C.TalismanReq Requirement { get; set; }
            public C.Skill? Skill { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void SetFormTexts(Level level)
        {
            lblTalismanTitle.Text = "Talisman for '" + level.Title + "'";

            txtTalismanTitle.Text = talisman.Title;
            radBronze.Checked = (talisman.AwardType == C.TalismanType.Bronze);
            radSilver.Checked = (talisman.AwardType == C.TalismanType.Silver);
            radGold.Checked = (talisman.AwardType == C.TalismanType.Gold);

            WriteRequirementList();

            cmbRequirementTypes.Items.Clear();

            // Add normal requirements
            foreach (C.TalismanReq requirement in C.TalismanReqArray.Cast<C.TalismanReq>())
            {
                if (requirement == C.TalismanReq.IndividualSkillLimits)
                    continue;

                if (SLXEditForm.isNeoLemmixOnly &&
                    (requirement == C.TalismanReq.ClassicMode ||
                     requirement == C.TalismanReq.KillZombies ||
                     requirement == C.TalismanReq.NoPause))
                    continue;

                cmbRequirementTypes.Items.Add(new TalismanRequirementOption
                {
                    Requirement = requirement,
                    Skill = null,
                    Text = C.TalismanReqText[requirement]
                });
            }

            // Add individual skill requirements
            foreach (string skillName in C.TalismanSkills)
            {
                C.Skill skill = (C.Skill)Enum.Parse(typeof(C.Skill), skillName);

                if (LevelFile.IsSkillRequired(level, skill))
                {
                    cmbRequirementTypes.Items.Add(new TalismanRequirementOption
                    {
                        Requirement = C.TalismanReq.IndividualSkillLimits,
                        Skill = skill,
                        Text = "Set " + skillName + " Limits"
                    });
                }
            }

            if (cmbRequirementTypes.Items.Count > 0)
                cmbRequirementTypes.SelectedIndex = 0;

            // Populate the Use Only Skill dropdown
            cmbRequirementSkill.Items.Clear();

            foreach (string skill in C.TalismanSkills)
            {
                if (LevelFile.IsSkillRequired(level, (C.Skill)Enum.Parse(typeof(C.Skill), skill)))
                {
                    cmbRequirementSkill.Items.Add(skill);
                }
            }
        }

        bool isNewTalisman;
        Level curLevel;
        Talisman talisman;
        Talisman oldTalisman;
        bool askToSave = true;

        /// <summary>
        /// Writes the list of requirements
        /// </summary>
        private void WriteRequirementList()
        {
            listRequirements.Items.Clear();

            bool hasRequirements = talisman.Requirements.Count > 0 ||
                                   talisman.SkillMinimum.Count > 0 ||
                                   talisman.SkillMaximum.Count > 0;

            if (!hasRequirements)
            {
                listRequirements.Items.Add("No requirements...");
                return;
            }

            // Write normal requirements
            foreach (C.TalismanReq requirement in talisman.Requirements.Keys)
            {
                string text = talisman.GetRequirementText(requirement);
                listRequirements.Items.Add(text);
            }

            // Write individual skill limits
            foreach (C.Skill skill in talisman.SkillMinimum.Keys
                         .Union(talisman.SkillMaximum.Keys))
            {
                int minimum = talisman.SkillMinimum.ContainsKey(skill)
                    ? talisman.SkillMinimum[skill]
                    : 0;

                int maximum = talisman.SkillMaximum.ContainsKey(skill)
                    ? talisman.SkillMaximum[skill]
                    : 0;

                string skillName = skill.ToString();

                string limitText;

                if (minimum > 0 && maximum > 0)
                    limitText = minimum + "-" + maximum;
                else if (maximum > 0)
                    limitText = "0-" + maximum;
                else
                    limitText = minimum + "+";

                listRequirements.Items.Add("Set " + skillName + " Limits: " + limitText);
            }
        }

        /// <summary>
        /// Adds a new requirement to the talisman.
        /// </summary>
        private void butRequirementAdd_Click(object sender, EventArgs e)
        {
            TalismanRequirementOption option = cmbRequirementTypes.SelectedItem as TalismanRequirementOption;

            if (option == null)
                return;

            C.TalismanReq requirement = option.Requirement;

            // Individual skill minimum/maximum
            if (requirement == C.TalismanReq.IndividualSkillLimits)
            {
                if (!option.Skill.HasValue)
                    return;

                C.Skill skill = option.Skill.Value;

                int minimum = (int)numReqValue1.Value;
                int maximum = (int)numReqValue2.Value;

                if (minimum > 0)
                    talisman.SkillMinimum[skill] = minimum;
                else
                    talisman.SkillMinimum.Remove(skill);

                if (maximum > 0)
                    talisman.SkillMaximum[skill] = maximum;
                else
                    talisman.SkillMaximum.Remove(skill);

                WriteRequirementList();
                return;
            }

            // All other requirements
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
                string selectedSkill = cmbRequirementSkill.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedSkill))
                    value = C.TalismanSkills.IndexOf(selectedSkill);
            }

            talisman.Requirements[requirement] = value;

            WriteRequirementList();
        }

        /// <summary>
        /// Deletes the selected requirements.
        /// </summary>
        private void butRequirementDelete_Click(object sender, EventArgs e)
        {
            foreach (var listItem in listRequirements.SelectedItems)
            {
                string itemText = listItem.ToString();

                // Check for an individual skill limit
                foreach (C.Skill skill in C.TalismanSkills
                             .Select(skillName => (C.Skill)Enum.Parse(typeof(C.Skill), skillName)))
                {
                    string skillName = skill.ToString();

                    if (itemText.StartsWith("Set " + skillName + " Limits:"))
                    {
                        talisman.SkillMinimum.Remove(skill);
                        talisman.SkillMaximum.Remove(skill);
                        break;
                    }
                }

                // Check for a normal requirement
                C.TalismanReq? requirementToRemove = null;

                foreach (C.TalismanReq requirement in talisman.Requirements.Keys)
                {
                    if (talisman.GetRequirementText(requirement) == itemText)
                    {
                        requirementToRemove = requirement;
                        break;
                    }
                }

                if (requirementToRemove.HasValue)
                    talisman.Requirements.Remove(requirementToRemove.Value);
            }

            WriteRequirementList();
        }

        /// <summary>
        /// Saves the talisman.
        /// </summary>
        private void butTalismanSave_Click(object sender, EventArgs e)
        {
            if (talisman.Requirements.Count == 0)
            {
                // Alert user that the talisman has no requirement
                if (MessageBox.Show(
                        "Warning: this talisman has no requirements. Do you want to save anyway?",
                        "Talisman Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveTalisman();
                }
                else
                {
                    // If user clicked No, exit without saving
                    return;
                }
            }
            else
            {
                SaveTalisman();
            }
        }


        /// <summary>
        /// // Exit talisman creation form without saving
        /// </summary>
        private void butTalismanCancel_Click(object sender, EventArgs e)
        {
            askToSave = false;
            Close();
        }

        /// <summary>
        /// Saves the talisman.
        /// </summary>
        private void SaveTalisman()
        {
            talisman.Title = txtTalismanTitle.Text;

            if (radBronze.Checked)
            {
                talisman.AwardType = C.TalismanType.Bronze;

                if(string.IsNullOrEmpty(talisman.Title))
                    talisman.Title = "Bronze Talisman";
            }
            else if (radSilver.Checked)
            {
                talisman.AwardType = C.TalismanType.Silver;

                if (string.IsNullOrEmpty(talisman.Title))
                    talisman.Title = "Silver Talisman";
            }
            else if (radGold.Checked)
            {
                talisman.AwardType = C.TalismanType.Gold;

                if (string.IsNullOrEmpty(talisman.Title))
                    talisman.Title = "Gold Talisman";
            }

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
            askToSave = false;
            Close();
        }

        /// <summary>
        /// Update other controls, depending on the selected requirement type.
        /// </summary>
        private void cmbRequirementTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            TalismanRequirementOption option = cmbRequirementTypes.SelectedItem as TalismanRequirementOption;

            if (option == null)
                return;

            C.TalismanReq requirement = option.Requirement;

            // Set visibility
            numReqValue1.Visible = (requirement != C.TalismanReq.UseOnlySkill &&
                                    requirement != C.TalismanReq.KillZombies  &&
                                    requirement != C.TalismanReq.ClassicMode  &&
                                    requirement != C.TalismanReq.NoPause);
            numReqValue2.Visible = (requirement == C.TalismanReq.TimeLimit) ||
                                   (requirement == C.TalismanReq.IndividualSkillLimits);

            if (numReqValue2.Visible)
                numReqValue1.Width = numReqValue2.Width;
            else
                numReqValue1.Width = cmbRequirementSkill.Width;      

            lblMin.Visible = (requirement == C.TalismanReq.IndividualSkillLimits);
            lblMax.Visible = (requirement == C.TalismanReq.IndividualSkillLimits);

            cmbRequirementSkill.Visible = (requirement == C.TalismanReq.UseOnlySkill);

            // Set maximums
            switch (requirement)
            {
                case C.TalismanReq.SaveReq:
                    numReqValue1.Maximum = 999;
                    break;
                case C.TalismanReq.TimeLimit:
                    {
                        numReqValue1.Maximum = 99;
                        numReqValue2.Maximum = 59;
                        break;
                    }
                case C.TalismanReq.SkillTotal:
                    numReqValue1.Maximum = 999;
                    break;
                case C.TalismanReq.SkillTypes:
                    numReqValue1.Maximum = 10;
                    break;
                case C.TalismanReq.IndividualSkillLimits:
                    numReqValue1.Maximum = 10;
                    numReqValue2.Maximum = 999;
                    break;
                default:
                    numReqValue1.Maximum = 99;
                    break;
            }

            // Set initial values, possible according to existing requirement
            if (requirement == C.TalismanReq.IndividualSkillLimits)
            {
                C.Skill skill = option.Skill.Value;

                numReqValue1.Value = talisman.SkillMinimum.ContainsKey(skill) ? talisman.SkillMinimum[skill] : 0;
                numReqValue2.Value = talisman.SkillMaximum.ContainsKey(skill) ? talisman.SkillMaximum[skill] : 0;
            }
            else if (talisman.Requirements.ContainsKey(requirement))
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
            if (askToSave)
            {
                // Ask user whether to save the talisman
                if (MessageBox.Show("Do you want to save the talisman?", "Save talisman?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveTalisman();
                }
            }
            //(this.Owner as FormMain).WriteTalismanInfo();
        }

        private void radBronze_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(talisman.Title) ||
                talisman.Title == "Silver Talisman" ||
                talisman.Title == "Gold Talisman")
                txtTalismanTitle.Text = "Bronze Talisman";
        }

        private void radSilver_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(talisman.Title) ||
                talisman.Title == "Bronze Talisman" ||
                talisman.Title == "Gold Talisman")
                txtTalismanTitle.Text = "Silver Talisman";
        }

        private void radGold_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(talisman.Title) ||
                talisman.Title == "Bronze Talisman" ||
                talisman.Title == "Silver Talisman")
                txtTalismanTitle.Text = "Gold Talisman";
        }

        private void FormTalisman_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                askToSave = false;
                Close();
            }
        }
    }
}
