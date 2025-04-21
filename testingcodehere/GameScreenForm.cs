using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameStartScreen;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class GameScreenForm : Form
{
    private PictureBox jumpingObject;
    private Panel ground;
    private Timer jumpTimer;
    private Timer obstacleTimer;
    private Random random;
    private int jumpSpeed = 20;
    private int gravity = 3;
    private bool isJumping = false;
    private int obstacleSpeed = 15;
    private int points = 0;
    private Label scoreLabel;
    private List<PictureBox> obstacles; // List to manage multiple obstacles
    private Button startButton; // Start button
    private Button exitFullscreenButton; // Exit fullscreen button
    private List<(string Username, int Score)> highScores = new List<(string Username, int Score)>(); // High score list

    public GameScreenForm()
    {
        // Initialize the form
        this.Text = "GOT ANY GRAPES";

        // Enable full-screen mode
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.White; // Sets the form's background color to white

        // Create the ground
        ground = new Panel
        {
            BackColor = Color.Brown,
            Height = 50,
            Dock = DockStyle.Bottom // Ground is fixed at the bottom
        };
        this.Controls.Add(ground);

        // Create the jumping object (player)
        jumpingObject = new PictureBox
        {
            Width = 75, // Fixed width
            Height = 75, // Fixed height
            Left = 100,
            Top = this.ClientSize.Height - ground.Height - 50, // Align dynamically with the ground
            SizeMode = PictureBoxSizeMode.StretchImage // Adjusts the image to fit the PictureBox size
        };

        // Set the image from embedded resources
        jumpingObject.Image = testingcodehere.Properties.Resources.DuckWalkAnimFINALGIF; // Replace "PlayerImage" with the actual resource name

        this.Controls.Add(jumpingObject);

        // Initialize random generator
        random = new Random();

        // Initialize obstacles list
        obstacles = new List<PictureBox>();

        // Set up the score label
        scoreLabel = new Label
        {
            Text = "Score: 0",
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.Black,
            Top = 10,
            Left = 10,
            AutoSize = true,
            Visible = true // Hidden until the game starts
        };
        this.Controls.Add(scoreLabel);

        // Add an Exit Fullscreen button
        exitFullscreenButton = new Button
        {
            Text = "Exit To Menu",
            Font = new Font("Arial", 12, FontStyle.Bold),
            BackColor = Color.Gray,
            ForeColor = Color.White,
            Width = 150,
            Height = 40,
            Top = 10, // Top-right corner
            Left = Screen.PrimaryScreen.Bounds.Width - 160
        };
        exitFullscreenButton.Click += ExitFullscreenButton_Click;
        this.Controls.Add(exitFullscreenButton);

        // Set up the timer for jump mechanics
        jumpTimer = new Timer { Interval = 20 }; // 20ms interval
        jumpTimer.Tick += JumpTimer_Tick;

        // Set up the timer for obstacle movement and spawning
        obstacleTimer = new Timer { Interval = 20 }; // 20ms interval
        obstacleTimer.Tick += ObstacleTimer_Tick;

        // Enable KeyDown and suppress default key behavior
        this.KeyPreview = true;
        this.KeyDown += JumpingGame_KeyDown;
        this.PreviewKeyDown += JumpingGame_PreviewKeyDown;

        // Fix positioning when the form resizes
        this.Resize += OnResize;

        //start game
        obstacleTimer.Start();
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        // Remove the start button and show the score label
        startButton.Visible = false;
        //scoreLabel.Visible = true;

        // Start the game timers
        //obstacleTimer.Start();
    }

    private void ExitFullscreenButton_Click(object sender, EventArgs e)
    {
        // Close the current game screen
        this.Close();

        // Reopen the StartScreenForm
        StartScreenForm startScreen = new StartScreenForm();
        startScreen.Show();
    }

    private void JumpingGame_KeyDown(object sender, KeyEventArgs e)
    {
        // Handle spacebar for jumping
        if (e.KeyCode == Keys.Space && !isJumping)// && startButton.Visible == false)
        {
            isJumping = true;
            jumpSpeed = -35; // Updated jump speed for higher jump
            jumpTimer.Start();
            e.Handled = true; // Prevent further processing of the key
        }
    }

    private void JumpingGame_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        // Suppress default behavior for the space key
        if (e.KeyCode == Keys.Space)
        {
            e.IsInputKey = true;
        }
    }

    private void JumpTimer_Tick(object sender, EventArgs e)
    {
        // Apply gravity and move the jumping object
        jumpSpeed += gravity;
        jumpingObject.Top += jumpSpeed;

        // Check if the object lands on the ground
        if (jumpingObject.Top >= this.ClientSize.Height - ground.Height - jumpingObject.Height)
        {
            jumpingObject.Top = this.ClientSize.Height - ground.Height - jumpingObject.Height;
            isJumping = false;
            jumpTimer.Stop();
        }
    }

    private void ObstacleTimer_Tick(object sender, EventArgs e)
    {
        // Move and update obstacles
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            PictureBox obstacle = obstacles[i];
            obstacle.Left -= obstacleSpeed;

            // Check for collision
            if (jumpingObject.Bounds.IntersectsWith(obstacle.Bounds))
            {
                obstacleTimer.Stop();
                jumpTimer.Stop();
                EndGame(); // Call EndGame method when collision occurs
                return;
            }

            // Remove obstacle if it goes off-screen
            if (obstacle.Right < 0)
            {
                this.Controls.Remove(obstacle);
                obstacles.RemoveAt(i);
                points++; // Increase points when an obstacle is cleared
                scoreLabel.Text = "Score: " + points; // Update score label
            }
        }

        // Randomly spawn new obstacles
        if (random.Next(0, 100) < 5) // 5% chance to spawn a new obstacle each tick
        {
            CreateObstacle();
        }
    }

    private void CreateObstacle()
    {
        // Minimum spacing between obstacles
        int lastObstacleRight = 0;
        if (obstacles.Count > 0)
        {
            lastObstacleRight = obstacles[obstacles.Count - 1].Right;
        }

        // Ensure at least 400 pixels of space between obstacles
        if (lastObstacleRight < this.ClientSize.Width - 400)
        {
            PictureBox newObstacle = new PictureBox
            {
                Width = 100, // Fixed width
                Height = 100, // Fixed height
                Top = this.ClientSize.Height - ground.Height - 100, // Align with ground
                Left = this.ClientSize.Width, // Spawn at the right edge of the screen
                SizeMode = PictureBoxSizeMode.StretchImage // Adjust image to fit the PictureBox size
            };

            // Set the image from embedded resources
            newObstacle.Image = testingcodehere.Properties.Resources.LemonEnemyAnimWIP; // Replace with your image resource name

            // Add the obstacle to the list and display it
            obstacles.Add(newObstacle);
            this.Controls.Add(newObstacle);
        }
    }

    private void OnResize(object sender, EventArgs e)
    {
        // Dynamically reposition the jumping object if not jumping
        if (!isJumping)
        {
            jumpingObject.Top = this.ClientSize.Height - ground.Height - jumpingObject.Height;
        }

        // Adjust obstacle positions if needed
        foreach (var obstacle in obstacles)
        {
            obstacle.Top = this.ClientSize.Height - ground.Height - obstacle.Height;
        }
    }

    private void EndGame()
    {
        // Prompt for username
        string username = PromptForUsername();

        // Add score to high scores
        if (!string.IsNullOrEmpty(username))
        {
            highScores.Add((username, points));
        }

        // Sort high scores and keep only top 5
        highScores = highScores.OrderByDescending(x => x.Score).Take(5).ToList();
        ScoreManager.HighScores.Add((username, points));
        ScoreManager.HighScores = ScoreManager.HighScores.OrderByDescending(x => x.Score).Take(5).ToList();
        // Display high score screen
        DisplayHighScores();
    }

    private string PromptForUsername()
    {
        // Simple input box to ask for username
        using (Form inputForm = new Form())
        {
            inputForm.Width = 300;
            inputForm.Height = 150;
            inputForm.Text = "Enter Username";

            Label promptLabel = new Label { Left = 10, Top = 10, Text = "Enter your username:", AutoSize = true };
            TextBox inputBox = new TextBox { Left = 10, Top = 40, Width = 260 };
            Button submitButton = new Button { Text = "Submit", Left = 10, Top = 80, Width = 80 };
            submitButton.Click += (sender, e) => { inputForm.Close(); };

            inputForm.Controls.Add(promptLabel);
            inputForm.Controls.Add(inputBox);
            inputForm.Controls.Add(submitButton);

            inputForm.ShowDialog();
            return inputBox.Text;
        }
    }

    private void DisplayHighScores()
    {
        using (Form highScoreForm = new Form())
        {
            highScoreForm.Width = 400;
            highScoreForm.Height = 300;
            highScoreForm.Text = "High Scores";

            Label titleLabel = new Label
            {
                Text = "Top 5 High Scores",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Top = 10,
                Left = 10
            };
            highScoreForm.Controls.Add(titleLabel);

            int topOffset = 50;
            foreach (var (Username, Score) in highScores)
            {
                Label scoreLabel = new Label
                {
                    Text = $"{Username}: {Score}",
                    AutoSize = true,
                    Top = topOffset,
                    Left = 10
                };
                highScoreForm.Controls.Add(scoreLabel);
                topOffset += 30;
            }

            Button resetButton = new Button
            {
                Text = "Restart Game",
                Left = 10,
                Top = topOffset,
                Width = 120
            };
            resetButton.Click += (sender, e) =>
            {
                highScoreForm.Close();
                RestartGame();
            };
            highScoreForm.Controls.Add(resetButton);

            highScoreForm.ShowDialog();
        }
    }
    public static class ScoreManager
    {
        public static List<(string Username, int Score)> HighScores = new List<(string, int)>();

        public static string GetHighScores()
        {
            if (HighScores.Count == 0)
                return "No high scores yet!";

            return string.Join("\n", HighScores.Select(x => $"{x.Username}: {x.Score}"));
        }
    }


    private void RestartGame()
    {
        // Reset points and remove obstacles
        points = 0;
        scoreLabel.Text = "Score: 0";
        foreach (var obstacle in obstacles)
        {
            this.Controls.Remove(obstacle);
        }
        obstacles.Clear();

        // Reset the jumping object
        jumpingObject.Top = this.ClientSize.Height - ground.Height - jumpingObject.Height;
        isJumping = false;

        // Restart timers
        jumpTimer.Stop();
        obstacleTimer.Start();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}