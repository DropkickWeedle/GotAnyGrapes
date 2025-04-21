using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using testingcodehere;
using static GameScreenForm;

namespace GameStartScreen
{
    public class StartScreenForm : Form
    {
        public StartScreenForm()
        {
            // Set form properties
            this.Text = "GOT ANY GRAPES";
            this.FormBorderStyle = FormBorderStyle.None; // Fullscreen mode
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.LightBlue;

            // Title Label
            Label titleLabel = new Label
            {
                Text = "GOT ANY GRAPES",
                Font = new Font("Arial", 48, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 150
            };
            this.Controls.Add(titleLabel);

            // Start Button
            Button startButton = new Button
            {
                Text = "Start",
                Font = new Font("Arial", 24),
                Size = new Size(200, 80),
                Location = new Point(this.ClientSize.Width / 2 - 100, 250),
                Anchor = AnchorStyles.None
            };
            startButton.Click += (sender, e) =>
            {
                this.Hide(); // Hide the start screen
                GameScreenForm gameScreen = new GameScreenForm(); // Transition to game screen
                gameScreen.ShowDialog();
                this.Show(); // Show the start screen again when the game screen closes
            };
            this.Controls.Add(startButton);

            // High Score Button
            Button highScoreButton = new Button
            {
                Text = "High Score",
                Font = new Font("Arial", 24),
                Size = new Size(200, 80),
                Location = new Point(this.ClientSize.Width / 2 - 100, 350),
                Anchor = AnchorStyles.None
            };
            highScoreButton.Click += HighScoreButton_Click; // Attach event handler
            this.Controls.Add(highScoreButton);

            // Exit Button
            Button exitButton = new Button
            {
                Text = "Exit",
                Font = new Font("Arial", 24),
                Size = new Size(200, 80),
                Location = new Point(this.ClientSize.Width / 2 - 100, 450),
                Anchor = AnchorStyles.None
            };
            exitButton.Click += (sender, e) => Application.Exit();
            this.Controls.Add(exitButton);
        }

        private void HighScoreButton_Click(object sender, EventArgs e)
        {
            string highScoreText = ScoreManager.GetHighScores();
            MessageBox.Show(highScoreText, "High Scores", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}