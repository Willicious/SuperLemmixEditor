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

            // Filter TalismanReqArray to include only skills present in skillset
            var filteredRequirements = C.TalismanReqArray.Cast<C.TalismanReq>()
                .Where(req => IsSkillRequirementRelevant(level, req))
                .ToList();

            // Check if the skillset is empty
            bool hasIndividualSkills = filteredRequirements.Any(req => req >= C.TalismanReq.SkillWalker && req <= C.TalismanReq.SkillCloner);

            // Filter out skill-related talismans if the skillset is empty
            if (!hasIndividualSkills)
            {
                filteredRequirements.Remove(C.TalismanReq.SkillTotal);
                filteredRequirements.Remove(C.TalismanReq.SkillTypes);
                filteredRequirements.Remove(C.TalismanReq.SkillEachLimit);
                filteredRequirements.Remove(C.TalismanReq.UseOnlySkill);
            }

            foreach (C.TalismanReq requirement in filteredRequirements)
            {
                cmbRequirementTypes.Items.Add(C.TalismanReqText[requirement]);
            }

            if (cmbRequirementTypes.Items.Count > 0)
            {
                cmbRequirementTypes.Text = cmbRequirementTypes.Items[0].ToString();
            }

            cmbRequirementSkill.Items.Clear(); // Clear any existing items
            foreach (string skill in C.TalismanSkills)
            {
                // Use skill string directly with IsSkillRequired
                if (LevelFile.IsSkillRequired(level, (C.Skill)Enum.Parse(typeof(C.Skill), skill)))
                {
                    cmbRequirementSkill.Items.Add(skill);
                }
            }
        }

        private bool IsSkillRequirementRelevant(Level level, C.TalismanReq requirement)
        {
            // Map TalismanReq to corresponding skills where applicable
            var skillMapping = new Dictionary<C.TalismanReq, C.Skill>
            {
                { C.TalismanReq.SkillWalker, C.Skill.Walker },
                { C.TalismanReq.SkillJumper, C.Skill.Jumper },
                { C.TalismanReq.SkillShimmier, C.Skill.Shimmier },
                { C.TalismanReq.SkillBallooner, C.Skill.Ballooner },
                { C.TalismanReq.SkillSlider, C.Skill.Slider },
                { C.TalismanReq.SkillClimber, C.Skill.Climber },
                { C.TalismanReq.SkillSwimmer, C.Skill.Swimmer },
                { C.TalismanReq.SkillFloater, C.Skill.Floater },
                { C.TalismanReq.SkillGlider, C.Skill.Glider },
                { C.TalismanReq.SkillDisarmer, C.Skill.Disarmer },
                { C.TalismanReq.SkillTimebomber, C.Skill.Timebomber },
                { C.TalismanReq.SkillBomber, C.Skill.Bomber },
                { C.TalismanReq.SkillFreezer, C.Skill.Freezer },
                { C.TalismanReq.SkillStoner, C.Skill.Stoner },
                { C.TalismanReq.SkillBlocker, C.Skill.Blocker },
                { C.TalismanReq.SkillLadderer, C.Skill.Ladderer },
                { C.TalismanReq.SkillPlatformer, C.Skill.Platformer },
                { C.TalismanReq.SkillBuilder, C.Skill.Builder },
                { C.TalismanReq.SkillStacker, C.Skill.Stacker },
                { C.TalismanReq.SkillSpearer, C.Skill.Spearer },
                { C.TalismanReq.SkillGrenader, C.Skill.Grenader },
                { C.TalismanReq.SkillLaserer, C.Skill.Laserer },
                { C.TalismanReq.SkillBasher, C.Skill.Basher },
                { C.TalismanReq.SkillFencer, C.Skill.Fencer },
                { C.TalismanReq.SkillMiner, C.Skill.Miner },
                { C.TalismanReq.SkillDigger, C.Skill.Digger },
                { C.TalismanReq.SkillCloner, C.Skill.Cloner }
            };

            if (skillMapping.ContainsKey(requirement))
            {
                return LevelFile.IsSkillRequired(level, skillMapping[requirement]);
            }

            // Return true for non-skill-related requirements
            return true;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRequirementTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!C.TalismanReqText.ContainsValue(cmbRequirementTypes.Text))
                return;

            C.TalismanReq requirement = C.TalismanReqText.First(pair => pair.Value.Equals(cmbRequirementTypes.Text)).Key;

            // Set visibility
            numReqValue1.Visible = (requirement != C.TalismanReq.UseOnlySkill &&
                                    requirement != C.TalismanReq.KillZombies  &&
                                    requirement != C.TalismanReq.ClassicMode  &&
                                    requirement != C.TalismanReq.NoPause);
            numReqValue2.Visible = (requirement == C.TalismanReq.TimeLimit);

            if (numReqValue2.Visible)
                numReqValue1.Width = numReqValue2.Width;
            else
                numReqValue1.Width = cmbRequirementSkill.Width;      

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
            txtTalismanTitle.Text = "Bronze Talisman";
        }

        private void radSilver_CheckedChanged(object sender, EventArgs e)
        {
            txtTalismanTitle.Text = "Silver Talisman";
        }

        private void radGold_CheckedChanged(object sender, EventArgs e)
        {
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
