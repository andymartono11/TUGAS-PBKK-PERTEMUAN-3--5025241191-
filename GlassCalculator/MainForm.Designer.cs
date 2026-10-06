using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GlassCalculator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Display
        private Panel   displayPanel;
        private Label   lblMode;
        private Label   lblMemoryValue;
        private Label   lblHistoryLine;
        private Label   lblExpression;
        private Label   lblResult;
        private TextBox txtExpression;

        // History
        private Panel   historyPanel;
        private Label   lblHistoryTitle;
        private ListBox historyList;
        private Button  btnClearHistory;
        private Button  btnToggleHistory;

        // Scientific
        private TableLayoutPanel sciTable;
        private TableLayoutPanel trigTable;
        private TableLayoutPanel memTable;
        private TableLayoutPanel keyTable;

        // Header
        private Panel  headerPanel;
        private Label  lblLogo;
        private Label  lblTitle;
        private Label  lblSubtitle;
        private Button btnDegRad;

        // Trig buttons (need refs for 2nd toggle)
        private Button btnSin, btnCos, btnTan;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Form ──────────────────────────────────────────────────────
            Text            = "GlassCalculator — Antonius Andy Martono";
            BackColor       = Color.FromArgb(26, 26, 46);
            ClientSize      = new Size(820, 900);
            MinimumSize     = new Size(660, 700);
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition   = FormStartPosition.CenterScreen;
            AutoScaleMode   = AutoScaleMode.Font;

            // ── Hidden TextBox ────────────────────────────────────────────
            txtExpression         = new TextBox { Visible = false };
            Controls.Add(txtExpression);

            // ── Header ────────────────────────────────────────────────────
            headerPanel           = new Panel();
            headerPanel.Dock      = DockStyle.Top;
            headerPanel.Height    = 78;
            headerPanel.BackColor = Color.FromArgb(40, 40, 70);

            lblLogo            = new Label();
            lblLogo.Text       = "⬡";
            lblLogo.Font       = new Font("Segoe UI", 24f, FontStyle.Bold);
            lblLogo.ForeColor  = Color.FromArgb(79, 195, 247);
            lblLogo.Location   = new Point(18, 12);
            lblLogo.AutoSize   = true;

            lblTitle           = new Label();
            lblTitle.Text      = "GlassCalculator";
            lblTitle.Font      = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location  = new Point(68, 10);
            lblTitle.AutoSize  = true;

            lblSubtitle           = new Label();
            lblSubtitle.Text      = "Scientific  •  History  •  by Antonius Andy Martono";
            lblSubtitle.Font      = new Font("Segoe UI", 8.5f);
            lblSubtitle.ForeColor = Color.FromArgb(180, 200, 220);
            lblSubtitle.Location  = new Point(70, 40);
            lblSubtitle.AutoSize  = true;

            btnDegRad             = MakeBtn("DEG", Color.FromArgb(50, 80, 120), Color.FromArgb(79, 195, 247));
            btnDegRad.Width       = 72;
            btnDegRad.Height      = 36;
            btnDegRad.Dock        = DockStyle.None;
            btnDegRad.Anchor      = AnchorStyles.Top | AnchorStyles.Right;
            btnDegRad.Location    = new Point(headerPanel.Width - 90, 20);
            btnDegRad.Click      += DegRad_Click;

            btnToggleHistory          = MakeBtn("⊟ Hist", Color.FromArgb(30, 60, 100), Color.FromArgb(79, 195, 247));
            btnToggleHistory.Width    = 72;
            btnToggleHistory.Height   = 36;
            btnToggleHistory.Dock     = DockStyle.None;
            btnToggleHistory.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleHistory.Location = new Point(headerPanel.Width - 170, 20);
            btnToggleHistory.Click   += ToggleHistory_Click;

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitle, lblSubtitle, btnDegRad, btnToggleHistory });
            Controls.Add(headerPanel);

            Resize += (s, e) => {
                btnDegRad.Left        = headerPanel.ClientSize.Width - 90;
                btnToggleHistory.Left = headerPanel.ClientSize.Width - 170;
            };

            // ── Root layout ───────────────────────────────────────────────
            var root = new TableLayoutPanel();
            root.Dock        = DockStyle.Fill;
            root.Padding     = new Padding(10);
            root.ColumnCount = 2;
            root.RowCount    = 1;
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            Controls.Add(root);

            // ── Left layout ───────────────────────────────────────────────
            var left = new TableLayoutPanel();
            left.Dock        = DockStyle.Fill;
            left.ColumnCount = 1;
            left.RowCount    = 5;
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.Controls.Add(left, 0, 0);

            // ── Display ───────────────────────────────────────────────────
            displayPanel             = new Panel();
            displayPanel.Dock        = DockStyle.Fill;
            displayPanel.Margin      = new Padding(0, 0, 6, 6);
            displayPanel.BackColor   = Color.FromArgb(20, 30, 55);
            displayPanel.Padding     = new Padding(14, 8, 14, 10);

            lblMode            = new Label();
            lblMode.Text       = "DEG";
            lblMode.Font       = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblMode.ForeColor  = Color.FromArgb(79, 195, 247);
            lblMode.Location   = new Point(14, 10);
            lblMode.AutoSize   = true;

            lblMemoryValue           = new Label();
            lblMemoryValue.Text      = "";
            lblMemoryValue.Font      = new Font("Segoe UI", 9f);
            lblMemoryValue.ForeColor = Color.FromArgb(206, 147, 216);
            lblMemoryValue.AutoSize  = true;
            lblMemoryValue.Location  = new Point(65, 10);

            lblHistoryLine           = new Label();
            lblHistoryLine.Text      = "";
            lblHistoryLine.Font      = new Font("Segoe UI", 9f);
            lblHistoryLine.ForeColor = Color.FromArgb(120, 140, 160);
            lblHistoryLine.Dock      = DockStyle.Top;
            lblHistoryLine.Height    = 22;
            lblHistoryLine.TextAlign = ContentAlignment.MiddleRight;

            lblExpression           = new Label();
            lblExpression.Text      = "";
            lblExpression.Font      = new Font("Segoe UI", 13f);
            lblExpression.ForeColor = Color.FromArgb(180, 200, 220);
            lblExpression.Dock      = DockStyle.Top;
            lblExpression.Height    = 30;
            lblExpression.TextAlign = ContentAlignment.MiddleRight;

            lblResult           = new Label();
            lblResult.Text      = "0";
            lblResult.Font      = new Font("Segoe UI", 30f, FontStyle.Bold);
            lblResult.ForeColor = Color.White;
            lblResult.Dock      = DockStyle.Fill;
            lblResult.TextAlign = ContentAlignment.MiddleRight;

            displayPanel.Controls.Add(lblResult);
            displayPanel.Controls.Add(lblExpression);
            displayPanel.Controls.Add(lblHistoryLine);
            displayPanel.Controls.Add(lblMode);
            displayPanel.Controls.Add(lblMemoryValue);
            left.Controls.Add(displayPanel, 0, 0);

            // ── Scientific ────────────────────────────────────────────────
            var sciPanel = DarkPanel(0, 0, 6, 6);
            sciTable = MakeTable(5, 3);
            sciTable.Dock = DockStyle.Fill;

            AddCell(sciTable, SciBtn("2nd","2nd"), 0,0);
            AddCell(sciTable, SciBtn("π","pi"),   1,0);
            AddCell(sciTable, SciBtn("e","e"),    2,0);
            AddCell(sciTable, SciBtn("x²","sq"),  3,0);
            AddCell(sciTable, SciBtn("x³","cube"),4,0);

            AddCell(sciTable, SciBtn("xʸ","pow"),  0,1);
            AddCell(sciTable, SciBtn("eˣ","exp"),  1,1);
            AddCell(sciTable, SciBtn("10ˣ","ten"), 2,1);
            AddCell(sciTable, SciBtn("1/x","inv"), 3,1);
            AddCell(sciTable, SciBtn("√x","sqrt"), 4,1);

            AddCell(sciTable, SciBtn("³√x","cbrt"),  0,2);
            AddCell(sciTable, SciBtn("log₂","log2"), 1,2);
            AddCell(sciTable, SciBtn("ln","ln"),     2,2);
            AddCell(sciTable, SciBtn("log","log"),   3,2);
            AddCell(sciTable, SciBtn("n!","factorial"),4,2);

            // Wire events
            foreach (Control c in sciTable.Controls)
                if (c is Button b)
                    b.Click += (string)b.Tag == "2nd" ? Second_Click : (EventHandler)Scientific_Click;

            sciPanel.Controls.Add(sciTable);
            left.Controls.Add(sciPanel, 0, 1);

            // ── Trig ──────────────────────────────────────────────────────
            var tPanel = DarkPanel(0, 0, 6, 6);
            trigTable = MakeTable(3, 1);
            trigTable.Dock = DockStyle.Fill;

            btnSin = SciBtn("sin","sin"); btnSin.Click += Trig_Click;
            btnCos = SciBtn("cos","cos"); btnCos.Click += Trig_Click;
            btnTan = SciBtn("tan","tan"); btnTan.Click += Trig_Click;
            AddCell(trigTable, btnSin, 0, 0);
            AddCell(trigTable, btnCos, 1, 0);
            AddCell(trigTable, btnTan, 2, 0);

            tPanel.Controls.Add(trigTable);
            left.Controls.Add(tPanel, 0, 2);

            // ── Memory ────────────────────────────────────────────────────
            var mPanel = DarkPanel(0, 0, 6, 6);
            memTable = MakeTable(5, 1);
            memTable.Dock = DockStyle.Fill;

            string[] mLabels = { "MC","MR","M+","M-","MS" };
            for (int i = 0; i < mLabels.Length; i++)
            {
                var mb = MemBtn(mLabels[i]);
                memTable.Controls.Add(mb, i, 0);
            }

            mPanel.Controls.Add(memTable);
            left.Controls.Add(mPanel, 0, 3);

            // ── Keypad ────────────────────────────────────────────────────
            var kPanel = DarkPanel(0, 0, 6, 0);
            keyTable = MakeTable(5, 6);
            keyTable.Dock = DockStyle.Fill;

            Color cBlue   = Color.FromArgb(30, 60, 100);
            Color cGreen  = Color.FromArgb(20, 70, 40);
            Color cOrange = Color.FromArgb(80, 55, 10);
            Color cRed    = Color.FromArgb(80, 20, 20);
            Color cCyan   = Color.FromArgb(10, 70, 90);

            Color fBlue   = Color.FromArgb(79, 195, 247);
            Color fGreen  = Color.FromArgb(165, 214, 167);
            Color fOrange = Color.FromArgb(255, 204, 128);
            Color fRed    = Color.FromArgb(239, 154, 154);
            Color fCyan   = Color.FromArgb(128, 222, 234);

            // Row 0
            AddKey(keyTable, MakeBtn("(",   cBlue,   fBlue),   0,0, Parenthesis_Click);
            AddKey(keyTable, MakeBtn(")",   cBlue,   fBlue),   1,0, Parenthesis_Click);
            AddKey(keyTable, MakeBtn("Ans", cBlue,   fBlue),   2,0, Ans_Click);
            AddKey(keyTable, MakeBtn("÷",   cOrange, fOrange), 3,0, Operator_Click);
            AddKey(keyTable, MakeBtn("⌫",   cRed,    fRed),    4,0, Backspace_Click);
            // Row 1
            AddKey(keyTable, MakeBtn("7",   cGreen,  fGreen),  0,1, Number_Click);
            AddKey(keyTable, MakeBtn("8",   cGreen,  fGreen),  1,1, Number_Click);
            AddKey(keyTable, MakeBtn("9",   cGreen,  fGreen),  2,1, Number_Click);
            AddKey(keyTable, MakeBtn("×",   cOrange, fOrange), 3,1, Operator_Click);
            AddKey(keyTable, MakeBtn("EXP", cBlue,   fBlue),   4,1, Exp_Click);
            // Row 2
            AddKey(keyTable, MakeBtn("4",   cGreen,  fGreen),  0,2, Number_Click);
            AddKey(keyTable, MakeBtn("5",   cGreen,  fGreen),  1,2, Number_Click);
            AddKey(keyTable, MakeBtn("6",   cGreen,  fGreen),  2,2, Number_Click);
            AddKey(keyTable, MakeBtn("−",   cOrange, fOrange), 3,2, Operator_Click);
            AddKey(keyTable, MakeBtn("RND", cBlue,   fBlue),   4,2, Rnd_Click);
            // Row 3
            AddKey(keyTable, MakeBtn("1",   cGreen,  fGreen),  0,3, Number_Click);
            AddKey(keyTable, MakeBtn("2",   cGreen,  fGreen),  1,3, Number_Click);
            AddKey(keyTable, MakeBtn("3",   cGreen,  fGreen),  2,3, Number_Click);
            AddKey(keyTable, MakeBtn("+",   cOrange, fOrange), 3,3, Operator_Click);
            AddKey(keyTable, MakeBtn("%",   cBlue,   fBlue),   4,3, Percent_Click);
            // Row 4
            AddKey(keyTable, MakeBtn("±",   cBlue,   fBlue),   0,4, Sign_Click);
            AddKey(keyTable, MakeBtn("0",   cGreen,  fGreen),  1,4, Number_Click);
            AddKey(keyTable, MakeBtn(".",   cGreen,  fGreen),  2,4, Decimal_Click);
            AddKey(keyTable, MakeBtn("^",   cOrange, fOrange), 3,4, Power_Click);
            AddKey(keyTable, MakeBtn("M",   cBlue,   fBlue),   4,4, MemRecall_Click);
            // Row 5: AC (1) + = (4)
            var ac = MakeBtn("AC", cRed, fRed);
            ac.Click += Clear_Click;
            keyTable.Controls.Add(ac, 0, 5);

            var eq = MakeBtn("=", cCyan, fCyan);
            eq.Font  = new Font("Segoe UI", 14f, FontStyle.Bold);
            eq.Click += Equals_Click;
            keyTable.SetColumnSpan(eq, 4);
            keyTable.Controls.Add(eq, 1, 5);

            kPanel.Controls.Add(keyTable);
            left.Controls.Add(kPanel, 0, 4);

            // ── History Panel ─────────────────────────────────────────────
            historyPanel           = new Panel();
            historyPanel.Dock      = DockStyle.Fill;
            historyPanel.Margin    = new Padding(6, 0, 0, 0);
            historyPanel.BackColor = Color.FromArgb(18, 25, 45);

            lblHistoryTitle           = new Label();
            lblHistoryTitle.Text      = "📋  History";
            lblHistoryTitle.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.White;
            lblHistoryTitle.Dock      = DockStyle.Top;
            lblHistoryTitle.Height    = 32;
            lblHistoryTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblHistoryTitle.Padding   = new Padding(8, 0, 0, 0);

            historyList               = new ListBox();
            historyList.Dock          = DockStyle.Fill;
            historyList.BackColor     = Color.FromArgb(18, 25, 45);
            historyList.ForeColor     = Color.White;
            historyList.BorderStyle   = BorderStyle.None;
            historyList.Font          = new Font("Segoe UI", 9f);
            historyList.ItemHeight    = 22;
            historyList.DoubleClick  += HistoryList_DoubleClick;

            btnClearHistory           = MakeBtn("Clear History", Color.FromArgb(70, 15, 15), Color.FromArgb(239,154,154));
            btnClearHistory.Dock      = DockStyle.Bottom;
            btnClearHistory.Height    = 32;
            btnClearHistory.Margin    = new Padding(4);
            btnClearHistory.Click    += ClearHistory_Click;

            historyPanel.Controls.Add(historyList);
            historyPanel.Controls.Add(lblHistoryTitle);
            historyPanel.Controls.Add(btnClearHistory);
            root.Controls.Add(historyPanel, 1, 0);
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private Panel DarkPanel(int l, int t, int r, int b)
        {
            var p = new Panel();
            p.Dock      = DockStyle.Fill;
            p.Margin    = new Padding(l, t, r, b);
            p.BackColor = Color.FromArgb(22, 32, 58);
            return p;
        }

        private Button MakeBtn(string text, Color back, Color fore)
        {
            var btn       = new Button();
            btn.Text      = text;
            btn.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.ForeColor = fore;
            btn.BackColor = back;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, fore);
            btn.Cursor    = Cursors.Hand;
            btn.Dock      = DockStyle.Fill;
            btn.Margin    = new Padding(3);
            return btn;
        }

        private Button MakeBtn(string text, Color back) =>
            MakeBtn(text, back, Color.White);

        private Button SciBtn(string text, string tag)
        {
            var btn   = MakeBtn(text, Color.FromArgb(25, 45, 80), Color.FromArgb(160, 190, 220));
            btn.Font  = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Tag   = tag;
            return btn;
        }

        private Button MemBtn(string text)
        {
            var btn  = MakeBtn(text, Color.FromArgb(40, 20, 60), Color.FromArgb(206, 147, 216));
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Tag  = text;
            btn.Click += Memory_Click;
            return btn;
        }

        private TableLayoutPanel MakeTable(int cols, int rows)
        {
            var tbl = new TableLayoutPanel();
            tbl.ColumnCount = cols;
            tbl.RowCount    = rows;
            for (int c = 0; c < cols; c++)
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
            for (int r = 0; r < rows; r++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            tbl.Padding = new Padding(3);
            return tbl;
        }

        private void AddCell(TableLayoutPanel t, Control c, int col, int row)
            => t.Controls.Add(c, col, row);

        private void AddKey(TableLayoutPanel t, Button b, int col, int row, EventHandler h)
        {
            b.Click += h;
            t.Controls.Add(b, col, row);
        }
    }
}
