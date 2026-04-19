namespace gui;

using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using datamodel;

public class UI : IUI
{
    private string lastMessage = "* The NPC stands before you.\nWhat will you do?";
    private List<string> dialogueHistory = new List<string>();
    private float scrollOffset = 0f;
    private int playerHp = 0;
    private int npcHp = 0;

    public UI()
    {
        Raylib.InitWindow(1280, 720, "Retro RPG UI");
        Raylib.SetTargetFPS(60);
    }

    ~UI()
    {
        if (Raylib.IsWindowReady())
        {
            Raylib.CloseWindow();
        }
    }

    public void UpdateStats(int player, int npc)
    {
        playerHp = player;
        npcHp = npc;
    }

    private void DrawStats()
    {
        Raylib.DrawText($"PLAYER HP: {playerHp}", 50, 520, 30, Color.Green);
        Raylib.DrawText($"NPC HP: {npcHp}", 1000, 520, 30, Color.Red);
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
        
        if (message.StartsWith("NPC: "))
        {
            dialogueHistory.Add(message);
            scrollOffset = float.MaxValue; // Autoscroll to bottom
            return; // Displayed alongside next message, don't block
        }
        else if (message.StartsWith("Round end"))
        {
            return; // Don't block on round end, go straight to menu
        }
        else
        {
            lastMessage = message;
        }

        // Skip input buffer from previous actions
        while (Raylib.GetCharPressed() > 0) { }

        // Dialogue loop
        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                break;
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Rectangle mainBox = new Rectangle(50, 250, 1180, 250);
            Raylib.DrawRectangleLinesEx(mainBox, 5, Color.White);

            DrawDialogueHistory();

            // Draw the current message in the box
            Color msgColor = Color.White;
            if (message.Contains("GAME OVER")) msgColor = Color.Red;
            else if (message.Contains("COMBAT ENDED")) msgColor = Color.Gold;
            
            DrawWrappedText(message, 80, 280, 1100, 30, msgColor);

            DrawStats();

            Raylib.DrawText("[ Press ENTER to continue ]", 80, 460, 20, Color.Gray);
            Raylib.EndDrawing();
        }
    }

    private void DrawWrappedText(string text, int x, int y, int maxWidth, int fontSize, Color color)
    {
        string[] lines = text.Split('\n');
        int yOffset = y;
        foreach (string line in lines)
        {
            string[] words = line.Split(' ');
            string currentLine = "";
            foreach (string word in words)
            {
                if (Raylib.MeasureText(currentLine + word + " ", fontSize) > maxWidth)
                {
                    Raylib.DrawText(currentLine, x, yOffset, fontSize, color);
                    yOffset += fontSize + 5;
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }
            Raylib.DrawText(currentLine, x, yOffset, fontSize, color);
            yOffset += fontSize + 5;
        }
    }

    private List<string> WrapText(string text, int maxWidth, int fontSize)
    {
        List<string> result = new List<string>();
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] words = line.Split(' ');
            string currentLine = "";
            foreach (string word in words)
            {
                if (Raylib.MeasureText(currentLine + word + " ", fontSize) > maxWidth)
                {
                    result.Add(currentLine.TrimEnd());
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }
            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                result.Add(currentLine.TrimEnd());
            }
        }
        return result;
    }

    private void DrawDialogueHistory()
    {
        if (dialogueHistory.Count == 0) return;

        int areaX = 50;
        int areaY = 20;
        int areaWidth = 1180;
        int areaHeight = 210;
        int fontSize = 30;

        List<string> wrappedLines = new List<string>();
        foreach (var msg in dialogueHistory)
        {
            wrappedLines.AddRange(WrapText(msg, areaWidth - 60, fontSize));
            wrappedLines.Add(""); // spacer
        }

        int totalHeight = wrappedLines.Count * (fontSize + 5);

        float wheel = Raylib.GetMouseWheelMove();
        if (wheel != 0)
        {
            scrollOffset -= wheel * 40;
        }

        float maxScroll = Math.Max(0, totalHeight - areaHeight);
        if (scrollOffset > maxScroll) scrollOffset = maxScroll;
        if (scrollOffset < 0) scrollOffset = 0;

        Raylib.BeginScissorMode(areaX, areaY, areaWidth, areaHeight);
        
        int yOff = areaY - (int)scrollOffset;
        foreach (var line in wrappedLines)
        {
            if (yOff > areaY - (fontSize + 5) && yOff < areaY + areaHeight)
            {
                Color c = line.StartsWith("Player:") ? new Color(0, 255, 255, 255) : Color.Yellow;
                Raylib.DrawText(line, areaX + 30, yOff, fontSize, c);
            }
            yOff += fontSize + 5;
        }
        
        Raylib.EndScissorMode();

        if (totalHeight > areaHeight)
        {
            float scrollRatio = areaHeight / (float)totalHeight;
            float scrollBarHeight = areaHeight * scrollRatio;
            float scrollPos = (scrollOffset / maxScroll) * (areaHeight - scrollBarHeight);
            
            Rectangle scrollBar = new Rectangle(areaX + areaWidth - 20, areaY + scrollPos, 15, scrollBarHeight);
            Raylib.DrawRectangleRec(scrollBar, Color.DarkGray);
        }
    }

    public void ShowLoading(Action action)
    {
        var task = System.Threading.Tasks.Task.Run(action);
        
        int frameCounter = 0;
        
        while (!task.IsCompleted && !Raylib.WindowShouldClose())
        {
            frameCounter++;
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            
            Rectangle mainBox = new Rectangle(50, 250, 1180, 250);
            Raylib.DrawRectangleLinesEx(mainBox, 5, Color.White);

            DrawDialogueHistory();
            
            string dots = new string('.', (frameCounter / 15) % 4);
            Raylib.DrawText("* Waiting for the NPC to react" + dots, 80, 280, 30, Color.White);
            
            DrawStats();

            Raylib.EndDrawing();
        }
        
        if (task.IsFaulted && task.Exception != null)
        {
            throw task.Exception.InnerException ?? task.Exception;
        }

        if (Raylib.WindowShouldClose())
        {
            Environment.Exit(0);
        }
    }

    public (ActionType, string?) PerformUserAction()
    {
        bool actionSelected = false;
        ActionType selectedAction = ActionType.ATTACK;
        string? textResult = null;

        bool isTalking = false;
        string inputText = "";

        Rectangle attackBtn = new Rectangle(200, 550, 300, 100);
        Rectangle talkBtn = new Rectangle(780, 550, 300, 100);

        while (!actionSelected && !Raylib.WindowShouldClose())
        {
            Vector2 mousePoint = Raylib.GetMousePosition();

            if (!isTalking)
            {
                if (Raylib.CheckCollisionPointRec(mousePoint, attackBtn) && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    selectedAction = ActionType.ATTACK;
                    actionSelected = true;
                }
                else if (Raylib.CheckCollisionPointRec(mousePoint, talkBtn) && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    isTalking = true;
                    while (Raylib.GetCharPressed() > 0) { }
                }
            }
            else
            {
                int key = Raylib.GetCharPressed();
                while (key > 0)
                {
                    if ((key >= 32) && (key <= 125) && (inputText.Length < 70))
                    {
                        inputText += (char)key;
                    }
                    key = Raylib.GetCharPressed();
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
                {
                    inputText = inputText.Substring(0, inputText.Length - 1);
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    selectedAction = ActionType.TALK;
                    textResult = inputText;
                    dialogueHistory.Add("Player: \"" + inputText + "\"");
                    scrollOffset = float.MaxValue;
                    actionSelected = true;
                }
                
                if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    isTalking = false;
                    inputText = "";
                }
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Rectangle mainBox = new Rectangle(50, 250, 1180, 250);
            Raylib.DrawRectangleLinesEx(mainBox, 5, Color.White);

            DrawDialogueHistory();

            if (!isTalking)
            {
                DrawWrappedText(lastMessage, 80, 280, 1100, 30, Color.White);
                
                bool hoverAttack = Raylib.CheckCollisionPointRec(mousePoint, attackBtn);
                Raylib.DrawRectangleLinesEx(attackBtn, 5, hoverAttack ? Color.Orange : Color.White);
                Raylib.DrawText("ATTACK", (int)attackBtn.X + 80, (int)attackBtn.Y + 35, 40, hoverAttack ? Color.Orange : Color.White);

                bool hoverTalk = Raylib.CheckCollisionPointRec(mousePoint, talkBtn);
                Raylib.DrawRectangleLinesEx(talkBtn, 5, hoverTalk ? Color.Orange : Color.White);
                Raylib.DrawText("TALK", (int)talkBtn.X + 100, (int)talkBtn.Y + 35, 40, hoverTalk ? Color.Orange : Color.White);
            }
            else
            {
                Raylib.DrawText("* What do you want to say?", 80, 280, 30, Color.White);
                
                Raylib.DrawText(inputText + "_", 80, 350, 30, Color.White);
                
                Raylib.DrawText("[ ENTER to confirm ]", 80, 460, 20, Color.Gray);
            }

            DrawStats();

            Raylib.EndDrawing();
        }

        if (Raylib.WindowShouldClose())
        {
            Environment.Exit(0);
        }

        return (selectedAction, textResult);
    }
}
